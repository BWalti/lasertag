using Lasertag.DomainModel;
using Lasertag.DomainModel.DomainEvents;
using Orleans;
using Orleans.EventSourcing.CustomStorage;
using Orleans.EventSourcing.CustomStorage.Marten;

namespace Lasertag.Manager;

public interface IGameManager : IGrainWithGuidKey,
    IEventSourcedGrain<Game, IGameEventBase>,
    ICustomStorageInterface<GameState, IGameEventBase>
{
}