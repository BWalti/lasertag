using Lasertag.DomainModel.DomainEvents;
using Lasertag.Manager.Game;
using Lasertag.Manager.GameRound;
using Marten;
using Marten.Events.Daemon.Resiliency;
using Marten.Events.Projections;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans.EventSourcing.CustomStorage.Marten;
using Orleans.Hosting;
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

host.ConfigureLogging(builder =>
{
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
    services.AddTransient<MartenJournaledGrainAdapter<GameState, IDomainEventBase>>();
    services.AddTransient<MartenJournaledGrainAdapter<GameRoundState, IDomainEventBase>>();

#pragma warning disable S125
    // services.AddSingleton<IGrainStorage, CustomGrainStorage>();
#pragma warning restore S125

    services.AddMarten(o =>
        {
            o.Connection(context.Configuration.GetConnectionString("Marten"));
            if (context.HostingEnvironment.IsDevelopment())
            {
                o.AutoCreateSchemaObjects = AutoCreate.All;
            }

#pragma warning disable S125
            // o.Projections.SelfAggregate<GameRoundState>(ProjectionLifecycle.Async);
#pragma warning restore S125
        })
        .AddAsyncDaemon(DaemonMode.Solo);
});

await host.RunConsoleAsync();