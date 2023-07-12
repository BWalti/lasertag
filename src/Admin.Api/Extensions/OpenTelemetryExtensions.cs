using System.Diagnostics;
using System.Reflection;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Admin.Api.Extensions;

public static class OpenTelemetryExtensions
{
    internal static readonly ActivitySource ActivitySource = new("Admin.Api");

    /// <summary>
    ///     Configures logging, distributed tracing, and metrics
    ///     <list type="bullet">
    ///         <item><term>Distributed tracing</term> uses the OTLP Exporter, which can be viewed with Jaeger</item>
    ///         <item><term>Metrics</term> uses the Prometheus Exporter</item>
    ///         <item>
    ///             <term>Logging</term> can use the OTLP Exporter, but due to limited vendor support it is not enabled by
    ///             default
    ///         </item>
    ///     </list>
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static WebApplicationBuilder AddOpenTelemetry(this WebApplicationBuilder builder)
    {
        var resourceBuilder = ResourceBuilder
            .CreateDefault()
            .AddService(
                builder.Environment.ApplicationName,
                serviceVersion: Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "unknown",
                serviceInstanceId: Environment.MachineName);

        builder.Services.AddOptions();

        var otlpExporterConfig = builder.Configuration.GetSection("OtlpExporter");
        builder.Services.Configure<OtlpExporterOptions>(otlpExporterConfig);

        builder.Logging.ClearProviders();
        builder.Logging
            .AddFilter("Microsoft", LogLevel.Warning) // generic host lifecycle messages
            .AddFilter("Orleans", LogLevel.Information) // suppress status dumps
            .AddFilter("Runtime", LogLevel.Warning) // also an Orleans prefix
            .AddFilter("Wolverine", LogLevel.Warning)
            .AddSimpleConsole(configure =>
            {
                configure.IncludeScopes = true;
                configure.SingleLine = true;
                configure.TimestampFormat = "HH:mm:ss.fffffff";
            })
            .AddOpenTelemetry(options =>
            {
                options.SetResourceBuilder(resourceBuilder);
                options.AddOtlpExporter(configure =>
                {
                    var uriString = otlpExporterConfig["Endpoint"];
                    if (string.IsNullOrEmpty(uriString))
                    {
                        return;
                    }

                    configure.Endpoint = new Uri(uriString);
                });
            });

        builder.Services.AddOpenTelemetry()
            .WithMetrics(metrics =>
                {
                    metrics.SetResourceBuilder(resourceBuilder)
                        .AddPrometheusExporter()
                        .AddMeter("Microsoft.Orleans")
                        .AddMeter("Wolverine")
                        .AddMeter("Marten")
                        .AddAspNetCoreInstrumentation()
                        .AddRuntimeInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddEventCountersInstrumentation(c =>
                        {
                            // https://learn.microsoft.com/en-us/dotnet/core/diagnostics/available-counters
                            c.AddEventSources(
                                "Microsoft.AspNetCore.Hosting",
                                "Microsoft-AspNetCore-Server-Kestrel",
                                "System.Net.Http",
                                "System.Net.Sockets",
                                "System.Net.NameResolution",
                                "System.Net.Security",
                                "Wolverine");
                        })
                        .AddOtlpExporter();
                }
            )
            .WithTracing(tracing =>
            {
                tracing.SetResourceBuilder(resourceBuilder)
                    .AddSource("Microsoft.Orleans.Runtime")
                    .AddSource("Microsoft.Orleans.Application")
                    .AddSource("Wolverine")
                    .AddSource(ActivitySource.Name)
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddOtlpExporter();
            });

        return builder;
    }
}