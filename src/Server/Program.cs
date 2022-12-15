using Lasertag.DomainModel;
using Lasertag.Manager;
using Marten;
using Marten.Events.Daemon.Resiliency;
using Marten.Events.Projections;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Exporter;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using Server;
using Weasel.Core;

// docker run --name martendb -p 5432:5432 -e POSTGRES_USER=demo -e POSTGRES_PASSWORD=demo -e POSTGRES_DATABASE=demo postgres
// docker rm -f martendb

var host = Host.CreateDefaultBuilder(args);

host.UseOrleans((_, builder) =>
{
    builder.UseLocalhostClustering();

    builder.AddLogStorageBasedLogConsistencyProviderAsDefault();
    builder.AddCustomStorageBasedLogConsistencyProviderAsDefault();

    builder.AddActivityPropagation();
});

host.ConfigureLogging((context, builder) =>
{
    var resourceBuilder = ResourceBuilder.CreateDefault().AddService(context.HostingEnvironment.ApplicationName);
    builder.AddOpenTelemetry(options =>
    {
        options
            .SetResourceBuilder(resourceBuilder)
            .AddOtlpExporter();
    });

    builder
        .AddFilter("Microsoft", LogLevel.Warning) // generic host lifecycle messages
        .AddFilter("Orleans", LogLevel.Information) // suppress status dumps
        .AddFilter("Runtime", LogLevel.Warning) // also an Orleans prefix
        .AddDebug() // VS Debug window
        .AddConsole();
});

host.ConfigureServices((context, services) =>
{
    services.AddLogging();
    services.AddOptions();
    services.Configure<OtlpExporterOptions>(context.Configuration.GetSection("OtlpExporter"));

    var resourceBuilder = ResourceBuilder.CreateDefault().AddService(context.HostingEnvironment.ApplicationName);
    services.AddOpenTelemetry(resourceBuilder);
    services.AddLasertagServer();
    services.AddMartenBackend(context.Configuration.GetConnectionString("Marten"), context.HostingEnvironment.IsDevelopment());

    services.AddHostedService<InitializeStuff>();
});

await host.RunConsoleAsync();