using Lasertag.DomainModel;
using Lasertag.DomainModel.DomainEvents;
using Lasertag.Manager;
using Lasertag.Manager.Game;
using Lasertag.Manager.GameRound;
using Marten;
using Marten.Events.Daemon.Resiliency;
using Marten.Events.Projections;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Orleans.EventSourcing.CustomStorage.Marten;
using Orleans.Providers;
using Orleans.Runtime;
using Orleans.Storage;
using Weasel.Core;

namespace Server;

public static class LasertagServerServiceCollectionExtensions
{
    public static void AddLasertagServer(this IServiceCollection services)
    {
        services.AddSingleton<MartenJournaledGrainAdapter<GameState, IDomainEventBase>>();
        services.AddSingleton<MartenJournaledGrainAdapter<GameRoundState, IDomainEventBase>>();
        services.AddSingleton(provider =>
            provider.GetServiceByName<IGrainStorage>(ProviderConstants.DEFAULT_STORAGE_PROVIDER_NAME));
        services.AddSingletonNamedService<IGrainStorage>(ProviderConstants.DEFAULT_STORAGE_PROVIDER_NAME,
            (provider, _) => new MartenGrainStorage(provider.GetRequiredService<IDocumentSession>()));
    }

    public static void AddMartenBackend(this IServiceCollection services, string? connectionString, bool isDevelopment)
    {
        services.AddMarten(o =>
            {
                if (string.IsNullOrEmpty(connectionString))
                {
                    throw new InvalidOperationException("Need to configure Marten connection string!");
                }

                o.Connection(connectionString);
                if (isDevelopment)
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
    }
}