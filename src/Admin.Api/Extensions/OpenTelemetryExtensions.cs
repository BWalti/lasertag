using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Reflection;
using OpenTelemetry;

namespace Admin.Api.Extensions;

#pragma warning disable S125 // Sections of code should not be commented out

public static class OpenTelemetryExtensions
{
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
                serviceName: builder.Environment.ApplicationName,
                serviceVersion: Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "unknown",
                serviceInstanceId: Environment.MachineName);

        builder.Services.AddOptions();
        builder.Services.Configure<OtlpExporterOptions>(builder.Configuration.GetSection("OtlpExporter"));

        var endpoint = builder.Configuration.GetSection("OtlpExporter")["Endpoint"]!;

        builder.Logging.ClearProviders();
        builder.Logging
            //.AddFilter("Microsoft", LogLevel.Warning) // generic host lifecycle messages
            //.AddFilter("Orleans", LogLevel.Information) // suppress status dumps
            //.AddFilter("Runtime", LogLevel.Warning) // also an Orleans prefix
            .AddOpenTelemetry(logging =>
            {
                logging.AddProcessor(new SimpleLogRecordExportProcessor(new ConsoleLogRecordExporter(
                    new ConsoleExporterOptions
                    {
                        Targets = ConsoleExporterOutputTargets.Console
                    })));

                logging
                    .SetResourceBuilder(resourceBuilder)
                    .AddConsoleExporter()
                    .AddOtlpExporter(options =>
                    {
                        options.Endpoint = new Uri(endpoint);
                    });
            });

        builder.Services.AddOpenTelemetryMetrics(metrics =>
        {
            metrics.SetResourceBuilder(resourceBuilder)
                .AddPrometheusExporter()
                .AddMeter("Microsoft.Orleans")
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
                });
            //.AddConsoleExporter();
            //.AddOtlpExporter();
        }
);

        builder.Services.AddOpenTelemetryTracing(tracing =>
        {
            tracing.SetResourceBuilder(resourceBuilder)
                .AddSource("Microsoft.Orleans.Runtime")
                .AddSource("Microsoft.Orleans.Application")
                .AddSource("Wolverine")
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation();
            //.AddConsoleExporter();
            //.AddOtlpExporter();
        });

        return builder;
    }
}

#pragma warning restore S125 // Sections of code should not be commented out
