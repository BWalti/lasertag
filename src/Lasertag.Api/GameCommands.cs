using Lasertag.DomainModel;
using Lasertag.DomainModel.DomainEvents;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Concurrency;

namespace Lasertag.Api;

[Reentrant]
[StatelessWorker]
public class GameCommands : Grain, IGameCommands
{
    public GameCommands(ILogger<GameCommands> logger)
    {
        EventRaiser = new EventRaiser(logger, GrainFactory);
    }

    public EventRaiser EventRaiser { get; }

    public async Task<ApiResult<Game>> InitializeGame(Guid gameId)
    {
        return await EventRaiser.RaiseEventWithChecks(gameId, game =>
        {
            if (game.State != GameStateEnum.None)
                throw new InvalidStateException("Can only Initialize game once after application start!");

            return new GameInitialized(gameId);
        });
    }

    public async Task<ApiResult<Game>> ConnectGameSet(Guid gameId, Guid gameSetId)
    {
        return await EventRaiser.RaiseEventWithChecks(gameId, game =>
        {
            if (game.State != GameStateEnum.Initialized)
                throw new InvalidStateException("Connecting GameSets is only possible when Game is initialized!");

            return new GameSetConnected(gameSetId);
        });
    }

    public async Task<ApiResult<Game>> ActivateGameSet(Guid gameId, Guid gameSetId, Guid playerId)
    {
        return await EventRaiser.RaiseEventWithChecks(gameId, game =>
        {
            if (game.State != GameStateEnum.LobyOpened)
                throw new InvalidStateException("Game is not ready for players!");

            if (game.ConnectedGameSets.All(gs => gs.GameSetId != gameSetId))
                throw new InvalidOperationException("This GameSet is unknown!");

            if (game.ActiveGameSets.Any(gs => gs.GameSetId == gameSetId))
                throw new InvalidOperationException("This GameSet is already active!");

            if (game.ActiveGameSets.Any(gs => gs.PlayerId == playerId))
                throw new InvalidOperationException("This Player is already active!");

            return new GameSetActivated(playerId, gameSetId);
        });
    }

    public async Task<ApiResult<Game>> CreateLobby(Guid gameId, int numberOfGroups)
    {
        return await EventRaiser.RaiseEventWithChecks(gameId, game =>
        {
            if (game.State != GameStateEnum.Initialized && game.State != GameStateEnum.GameFinished)
                throw new InvalidStateException("Lobby cannot be created currently!");

            var groups = game.ConnectedGameSets
                .Select((s, i) => new { s, i })
                .GroupBy(x => x.i % numberOfGroups)
                .Select((g, i) => new GameSetGroup(
                    Guid.NewGuid(),
                    g.Select(x => x.s).ToArray(),
                    GetGameColor(i)
                ))
                .ToArray();

            return new GameLobbyCreated(groups);
        });
    }

    public Task<ApiResult<Game>> StartGame(Guid gameId)
    {
        return EventRaiser.RaiseEventWithChecks(gameId, _ => new GameStarted());
    }

    private static GroupColor GetGameColor(int index)
    {
        if (index > 8) throw new InvalidOperationException("We do only support up to 8 groups (colors)");

        return (GroupColor)(index - 1);
    }
}