using Lasertag.DomainModel;
using Lasertag.DomainModel.DomainEvents;
using Orleans;
using Orleans.EventSourcing.CustomStorage.Marten;

namespace Lasertag.Manager;

public class GameManager : EventSourcedGrain<Game, GameState, IDomainEventBase>, IGameManager
{
    readonly MartenJournaledGrainAdapter<GameState, IDomainEventBase> _martenAdapter;

    public GameManager(MartenJournaledGrainAdapter<GameState, IDomainEventBase> martenAdapter)
    {
        _martenAdapter = martenAdapter;
    }

    public Task<KeyValuePair<int, GameState>> ReadStateFromStorage() =>
        _martenAdapter.ReadStateFromStorage(this.GetPrimaryKey());

    public Task<bool> ApplyUpdatesToStorage(IReadOnlyList<IDomainEventBase> updates, int expectedversion) =>
        _martenAdapter.ApplyUpdatesToStorage(this.GetPrimaryKey(), updates, expectedversion);
}