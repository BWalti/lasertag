using JetBrains.Annotations;
using Lasertag.DomainModel;
using Lasertag.DomainModel.DomainEvents.GameEvents;
using Orleans;

namespace Lasertag.Manager.Game;

[GenerateSerializer]
public class GameState : DomainModel.Game
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
        ConnectedGameSets.Add(new LasertagSet(e.GameSetId, "Name"));
    }

    [UsedImplicitly]
    public void Apply(GameSetDisconnected e)
    {
        var lasertagSet = ConnectedGameSets.First(set => set.Id == e.GameSetId);
        ConnectedGameSets.Remove(lasertagSet);
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
    public void Apply(GameRoundStarted e)
    {
        ActiveRoundId = e.GameRoundId;
        Status = GameStatus.GameStarted;
    }
}