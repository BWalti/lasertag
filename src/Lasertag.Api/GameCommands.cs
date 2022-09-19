using Lasertag.DomainModel;
using Lasertag.DomainModel.DomainEvents;
using Lasertag.DomainModel.DomainEvents.GameEvents;
using Lasertag.Manager.Game;
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
        EventRaiser = new EventRaiser<IGameManager, Game, GameState, IDomainEventBase>(logger, GrainFactory);
    }

    public EventRaiser<IGameManager, Game, GameState, IDomainEventBase> EventRaiser { get; }

    public async Task<ApiResult<Game>> InitializeGame(Guid gameId)
    {
        return await EventRaiser.RaiseEventWithChecks(gameId, game =>
        {
            if (game.Status != GameStatus.None)
            {
                throw new InvalidStateException("Can only Initialize game once after application start!");
            }

            return new GameInitialized(gameId);
        });
    }

    public async Task<ApiResult<Game>> ConnectGameSet(Guid gameId, Guid gameSetId)
    {
        return await EventRaiser.RaiseEventWithChecks(gameId, game =>
        {
            if (game.Status != GameStatus.Initialized)
            {
                throw new InvalidStateException("Connecting GameSets is only possible when Game is initialized!");
            }

            return new GameSetConnected(gameSetId);
        });
    }

    public async Task<ApiResult<Game>> ActivateGameSet(Guid gameId, Guid gameSetId, Guid playerId)
    {
        return await EventRaiser.RaiseEventWithChecks(gameId, game =>
        {
            if (game.Status != GameStatus.LobyOpened)
            {
                throw new InvalidStateException("Game is not ready for players!");
            }

            if (game.ConnectedGameSets.All(gs => gs.Id != gameSetId))
            {
                throw new InvalidOperationException("This LasertagSet is unknown!");
            }

            if (game.ActiveGameSets.Any(gs => gs.GameSetId == gameSetId))
            {
                throw new InvalidOperationException("This LasertagSet is already active!");
            }

            if (game.ActiveGameSets.Any(gs => gs.PlayerId == playerId))
            {
                throw new InvalidOperationException("This Player is already active!");
            }

            return new GameSetActivated(playerId, gameSetId);
        });
    }

    public async Task<ApiResult<Game>> CreateLobby(Guid gameId, int numberOfGroups)
    {
        return await EventRaiser.RaiseEventWithChecks(gameId, game =>
        {
            if (game.Status != GameStatus.Initialized && game.Status != GameStatus.GameFinished)
            {
                throw new InvalidStateException("Lobby cannot be created currently!");
            }

            var groups = game.ConnectedGameSets
                .Select((s, i) => new { s, i })
                .GroupBy(x => x.i % numberOfGroups)
                .Select((g, i) => new GameGroup(
                    Guid.NewGuid(),
                    g.Select(x => x.s).ToArray(),
                    GetGameColor(i)
                ))
                .ToArray();

            return new GameLobbyCreated(groups);
        });
    }

    public async Task<(ApiResult<Game>, ApiResult<GameRound>)> StartGameRound(Guid gameId)
    {
        var game = await EventRaiser.RaiseEventWithChecks(gameId, game => new GameRoundStarted(Guid.NewGuid(), game.ActiveGameSets, game.GameSetGroups));
        var gameRoundCommands = GrainFactory.GetGrain<IGameRoundCommands>(0);

        try
        {
            if (game.Output == null)
            {
                throw new InvalidOperationException("GameRound ID not found!");
            }

            var gameRound = await gameRoundCommands.StartGameRound(
                game.Output.ActiveRoundId,
                game.Output.ActiveGameSets,
                game.Output.GameSetGroups);

            if (gameRound.Output == null)
            {
                throw new InvalidOperationException("GameRound not correctly initialized");
            }

            return (game, gameRound);
        }
        catch (Exception e)
        {
            return (game, new ApiResult<GameRound>(e));
        }
    }

    static GroupColor GetGameColor(int index)
    {
        if (index > 7)
        {
            throw new InvalidOperationException("We do only support up to 8 groups (colors)");
        }

        return (GroupColor)index;
    }
}