using GrainInterfaces;
using GrainInterfaces.Models;
using Marten;
using Orleans.Concurrency;
using Orleans.EventSourcing.CustomStorage.Marten;

namespace Grains;

public class GameGrain : MartenJournaledGrain<GameGrainState, IGameEvents>, IGameGrain
{
    public GameGrain(IDocumentStore store) : base(store)
    {
    }

    public Task<Immutable<GameGroup[]>> GetPreparedGameSetGroups()
    {
        return Task.FromResult(new Immutable<GameGroup[]>(State.Groups));
    }

    public async Task OnGameSetActivated(Guid gameSetId, Guid playerId)
    {
        if (!State.AllPreparedVests.Contains(gameSetId))
        {
            throw new InvalidOperationException("This GameSet is unknown!");
        }

        RaiseEvent(new PlayerActivatedGameSet(playerId, gameSetId));
        await ConfirmEvents();
    }

    public async Task StartGame()
    {
        // Check which groups are actually playing (at least one player):
        var bla = (from pwgs in State.PlayersWithGameSets
            let groupId = State.Groups.Single(g => g.GameSetIds.Contains(pwgs.GameSetId))
            select (pwgs.PlayerId, pwgs.GameSetId, groupId));

        var numberOfActiveGroups = bla.Select(x => x.groupId).Distinct().Count();
        if (numberOfActiveGroups < 2)
        {
            throw new InvalidOperationException("There need to be at least two groups to play!");
        }
        
        await ConfirmEvents();
    }

    public async Task SetPreparedGameSetGroups(Immutable<GameGroup[]> groups)
    {
        RaiseEvent(new GameGroupsDefined(groups.Value));
        await ConfirmEvents();
    }
}

public record PlayerActivatedGameSet(Guid PlayerId, Guid GameSetId) : IGameEvents;

public record GameGroupsDefined(GameGroup[] Groups) : IGameEvents;

public class GameGrainState
{
    public Guid Id { get; set; }

    public Guid[] AllPreparedVests { get; set; } = {};
    public GameGroup[] Groups { get; set; } = {};
    public List<(Guid PlayerId, Guid GameSetId)> PlayersWithGameSets { get; set; } = new();

    public void Apply(GameGroupsDefined @event)
    {
        Groups = @event.Groups;
        AllPreparedVests = Groups.SelectMany(g => g.GameSetIds).ToArray();
    }

    public void Apply(PlayerActivatedGameSet @event)
    {
        if (PlayersWithGameSets.Any(pv => pv.PlayerId == @event.PlayerId || pv.GameSetId == @event.GameSetId))
        {
            throw new InvalidOperationException("Either Player OR GameSet is already in use!");
        }

        PlayersWithGameSets.Add((@event.PlayerId, @event.GameSetId));
    }
}