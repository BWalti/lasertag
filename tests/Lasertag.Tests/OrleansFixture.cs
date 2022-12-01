using Marten;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Weasel.Core;

namespace Lasertag.Tests;

public class OrleansFixture : IDisposable
{
    readonly IHost _host;

    public OrleansFixture()
    {
        _host = BuildAndStartSiloAsync();
        ClusterClient = _host.Services.GetRequiredService<IClusterClient>();
    }

    public IClusterClient ClusterClient { get; }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        _host.Dispose();
    }

    static IHost BuildAndStartSiloAsync()
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

#pragma warning disable S125
            // services.AddSingleton<IGrainStorage, CustomGrainStorage>();
#pragma warning restore S125

            services.AddMarten(options =>
            {
                var connectionString = context.Configuration.GetConnectionString("Marten");
                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new InvalidOperationException("Need to configure Marten connection string!");
                }

                options.Connection(connectionString);
                if (context.HostingEnvironment.IsDevelopment())
                {
                    options.AutoCreateSchemaObjects = AutoCreate.All;
                }
            });
        });

        var host = hostBuilder.Build();
        host.Start();

        return host;
    }
}