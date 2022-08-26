using GrainInterfaces;
using GrainInterfaces.Models;
using Marten;
using Orleans.Concurrency;
using Orleans.EventSourcing.CustomStorage.Marten;

namespace Grains;

public class GameManagerGrain : MartenJournaledGrain<GameManagerState, IGameEvents>, IGameManagerGrain
{
    public GameManagerGrain(IDocumentStore store) : base(store)
    {
    }

    public async Task<IGameGrain> PrepareGame(GameConfiguration config)
    {
        var gameSets = State.OnlineGameSets.ToList();

        if (gameSets.Count < config.NumberOfGroups)
        {
            throw new InvalidOperationException(
                $"Cannot play a game with more groups ({config.NumberOfGroups}) than game sets ({gameSets.Count})");
        }

        var gameId = Guid.NewGuid();
        var gameGrain = GrainFactory.GetGrain<IGameGrain>(gameId);

        var groups = gameSets.Select((s, i) => new { s, i })
            .GroupBy(x => x.i % config.NumberOfGroups)
            .Select(g => new GameGroup(Guid.NewGuid(), g.Select(x => x.s).ToArray()))
            .ToArray();

        await gameGrain.SetPreparedGameSetGroups(new Immutable<GameGroup[]>(groups));
        return gameGrain;
    }

    public Task<int> GetOnlineGameSets()
    {
        return Task.FromResult(State.OnlineGameSets.Count);
    }

    public async Task GameSetOnline(Guid id)
    {
        RaiseEvent(new GameSetOnline(id));
        await ConfirmEvents();
    }

    public async Task GameSetOffline(Guid id)
    {
        RaiseEvent(new GameSetOffline(id));
        await ConfirmEvents();
    }
}