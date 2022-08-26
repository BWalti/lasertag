using Marten;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Hosting;
using Weasel.Core;

namespace Lasertag.Grains.Tests;

public class OrleansFixture : IDisposable
{
    private readonly IHost _host;

    public OrleansFixture()
    {
        _host = BuildAndStartSiloAsync();
        ClusterClient = _host.Services.GetRequiredService<IClusterClient>();
    }

    public IClusterClient ClusterClient { get; }

    public void Dispose()
    {
        _host.Dispose();
    }

    private static IHost BuildAndStartSiloAsync()
    {
        var hostBuilder = new HostBuilder()
            .ConfigureHostConfiguration(builder => builder.AddJsonFile("appsettings.json"))
            .UseOrleans((_, builder) =>
            {
                builder.UseLocalhostClustering();
                builder.AddLogStorageBasedLogConsistencyProviderAsDefault();
                builder.AddCustomStorageBasedLogConsistencyProviderAsDefault();

                builder.AddActivityPropagation();
                builder.ConfigureLogging(logging => logging.AddConsole());
            });


        hostBuilder.ConfigureLogging(builder =>
        {
            builder
                .AddFilter("Microsoft", LogLevel.Warning) // generic host lifecycle messages
                .AddFilter("Orleans", LogLevel.Information) // suppress status dumps
                .AddFilter("Runtime", LogLevel.Warning) // also an Orleans prefix
                .AddDebug() // VS Debug window
                .AddConsole();
        });

        hostBuilder.ConfigureServices((context, services) =>
        {
            services.AddLogging();
            //services.AddSingleton<IGrainStorage, CustomGrainStorage>();

            services.AddMarten(options =>
            {
                options.Connection(context.Configuration.GetConnectionString("Marten"));
                if (context.HostingEnvironment.IsDevelopment()) options.AutoCreateSchemaObjects = AutoCreate.All;
            });
        });

        var host = hostBuilder.Build();
        host.Start();

        return host;
    }
}