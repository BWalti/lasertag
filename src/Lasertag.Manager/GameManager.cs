using Lasertag.DomainModel;
using Lasertag.DomainModel.DomainEvents;
using Orleans;
using Orleans.EventSourcing.CustomStorage.Marten;

namespace Lasertag.Manager;

public class GameManager : EventSourcedGrain<Game, GameState, IGameEventBase>, IGameManager
{
    private readonly MartenJournaledGrainAdapter<GameState, IGameEventBase> _martenAdapter;

    public GameManager(MartenJournaledGrainAdapter<GameState, IGameEventBase> martenAdapter)
    {
        _martenAdapter = martenAdapter;
    }

    public Task<KeyValuePair<int, GameState>> ReadStateFromStorage()
    {
        return _martenAdapter.ReadStateFromStorage(this.GetPrimaryKey());
    }

    public Task<bool> ApplyUpdatesToStorage(IReadOnlyList<IGameEventBase> updates, int expectedversion)
    {
        return _martenAdapter.ApplyUpdatesToStorage(this.GetPrimaryKey(), updates, expectedversion);
    }
}