﻿using Lasertag.DomainModel;
using Lasertag.DomainModel.DomainEvents;
using Lasertag.Manager;
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
using Orleans.Providers;
using Orleans.Runtime;
using Orleans.Storage;
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
    services.AddSingleton<MartenJournaledGrainAdapter<GameState, IDomainEventBase>>();
    services.AddSingleton<MartenJournaledGrainAdapter<GameRoundState, IDomainEventBase>>();

    services.AddHostedService<InitializeStuff>();

    services.AddSingleton(provider =>
        provider.GetServiceByName<IGrainStorage>(ProviderConstants.DEFAULT_STORAGE_PROVIDER_NAME));
    services.AddSingletonNamedService<IGrainStorage>(ProviderConstants.DEFAULT_STORAGE_PROVIDER_NAME,
        (provider, _) => new MartenGrainStorage(provider.GetRequiredService<IDocumentSession>()));

    services.AddMarten(o =>
        {
            var connectionString = context.Configuration.GetConnectionString("Marten");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException("Need to configure Marten connection string!");
            }

            o.Connection(connectionString);
            if (context.HostingEnvironment.IsDevelopment())
            {
                o.AutoCreateSchemaObjects = AutoCreate.All;
            }

            o.Schema.For<Game>().Identity(g => g.GameId);

            o.Projections.Add<ScoreBoardProjection>(ProjectionLifecycle.Async);

#pragma warning disable S125
            // o.Projections.SelfAggregate<GameRoundState>(ProjectionLifecycle.Async);
#pragma warning restore S125
        })
        .AddAsyncDaemon(DaemonMode.Solo);
});

await host.RunConsoleAsync();