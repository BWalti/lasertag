using Lasertag.DomainModel;
using Lasertag.DomainModel.DomainEvents;
using Orleans;

namespace Lasertag.Manager;

[GenerateSerializer]
public class GameState : Game
{
    public void Apply(GameInitialized e)
    {
        GameId = e.GameId;
        Status = GameStatus.Initialized;
    }

    public void Apply(GameSetConnected e)
    {
        ConnectedGameSets.Add(new GameSet(e.GameSetId));
    }

    public void Apply(GameSetDisconnected e)
    {
        ConnectedGameSets.Remove(new GameSet(e.GameSetId));
    }

    public void Apply(GameLobbyCreated e)
    {
        GameSetGroups = e.GameSetGroups;
        Status = GameStatus.LobyOpened;
    }

    public void Apply(GameSetActivated e)
    {
        ActiveGameSets.Add(new ActiveGameSet(e.PlayerId, e.GameSetId));
    }

    public void Apply(GameStarted e)
    {
        Status = GameStatus.GameStarted;
    }
}
