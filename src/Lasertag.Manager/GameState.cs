using JetBrains.Annotations;
using Lasertag.DomainModel;
using Lasertag.DomainModel.DomainEvents;
using Orleans;

namespace Lasertag.Manager;

[GenerateSerializer]
public class GameState : Game
{
    [UsedImplicitly]
    public void Apply(GameInitialized e)
    {
        GameId = e.GameId;
        Status = GameStatus.Initialized;
    }

    [UsedImplicitly]
    public void Apply(GameSetConnected e)
    {
        ConnectedGameSets.Add(new GameSet(e.GameSetId));
    }

    [UsedImplicitly]
    public void Apply(GameSetDisconnected e)
    {
        ConnectedGameSets.Remove(new GameSet(e.GameSetId));
    }

    [UsedImplicitly]
    public void Apply(GameLobbyCreated e)
    {
        GameSetGroups = e.GameSetGroups;
        Status = GameStatus.LobyOpened;
    }

    [UsedImplicitly]
    public void Apply(GameSetActivated e)
    {
        ActiveGameSets.Add(new ActiveGameSet(e.PlayerId, e.GameSetId));
    }

    [UsedImplicitly]
    public void Apply(GameStarted _)
    {
        Status = GameStatus.GameStarted;
    }
}