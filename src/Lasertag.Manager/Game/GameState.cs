using JetBrains.Annotations;
using Lasertag.DomainModel;
using Lasertag.DomainModel.Data;
using Orleans;
using static Lasertag.DomainModel.DomainEvents.InfrastructureEvents;
using static Lasertag.DomainModel.DomainEvents.GameRoundEvents;

namespace Lasertag.Manager.Game;

[GenerateSerializer]
public class GameState : DomainModel.Game
{
    [UsedImplicitly]
    public void Apply(GameServerInitialized e)
    {
        GameId = e.GameId;
        Status = GameStatus.Initialized;
    }

    [UsedImplicitly]
    public void Apply(GameSetRegistered e)
    {
        var name = PokemonNames.GetRandomName();

        // there probably need to be two different kind of configurations
        // one with ID, one without.
        GameSets.Add(new GameSet
        {
            Id = e.Configuration.Id,
            Configuration = e.Configuration,
            Name = name
        });
    }

    [UsedImplicitly]
    public void Apply(GameSetConnected e)
    {
        var set = GameSets.Single(gameSet => gameSet.Id == e.GameSetId);
        set.IsOnline = true;
    }

    [UsedImplicitly]
    public void Apply(GameSetDisconnected e)
    {
        var set = GameSets.Single(gameSet => gameSet.Id == e.GameSetId);
        set.IsOnline = false;
    }

    [UsedImplicitly]
    public void Apply(GameSetUnregistered e)
    {
        GameSets.RemoveAll(gameSet => gameSet.Id == e.GameSetId);
    }

    [UsedImplicitly]
    public void Apply(LobbyCreated _)
    {
        Status = GameStatus.LobyOpened;
    }

    [UsedImplicitly]
    public void Apply(Started e)
    {
        ActiveRoundId = e.GameRoundId;
        Status = GameStatus.GameStarted;
    }
}