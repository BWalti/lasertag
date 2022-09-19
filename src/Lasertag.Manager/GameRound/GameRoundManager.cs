using Lasertag.DomainModel.DomainEvents;
using Orleans;
using Orleans.EventSourcing.CustomStorage.Marten;

namespace Lasertag.Manager.GameRound;

public class GameRoundManager : EventSourcedGrain<DomainModel.GameRound, GameRoundState, IDomainEventBase>,
    IGameRoundManager
{
    readonly MartenJournaledGrainAdapter<GameRoundState, IDomainEventBase> _martenAdapter;

    public GameRoundManager(MartenJournaledGrainAdapter<GameRoundState, IDomainEventBase> martenAdapter)
    {
        _martenAdapter = martenAdapter;
    }

    public Task<KeyValuePair<int, GameRoundState>> ReadStateFromStorage() =>
        _martenAdapter.ReadStateFromStorage(this.GetPrimaryKey());

    public Task<bool> ApplyUpdatesToStorage(IReadOnlyList<IDomainEventBase> updates, int expectedversion) =>
        _martenAdapter.ApplyUpdatesToStorage(this.GetPrimaryKey(), updates, expectedversion);
}