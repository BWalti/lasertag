using Lasertag.DomainModel;
using Lasertag.DomainModel.DomainEvents;
using Lasertag.Manager.Game;
using Microsoft.Extensions.Logging;
using static Lasertag.DomainModel.DomainEvents.GameRoundEvents;
using static Lasertag.DomainModel.DomainEvents.InfrastructureEvents;

namespace Lasertag.Api;

//public class GameCommands : IGameCommands
//{
//    public async Task<ApiResult<Game>> InitializeGame(Guid gameId)
//    {
//        var result = await EventRaiser.RaiseEventWithChecks(gameId, game =>
//        {
//            if (game.Status != GameStatus.None)
//            {
//                throw new InvalidStateException("Can only Initialize game once after application start!");
//            }

//            return new GameServerInitialized(gameId);
//        });

//        return result;
//    }

//    public async Task<ApiResult<Game>> RegisterGameSet(Guid gameId, GameSetConfiguration configuration)
//    {
//        return await EventRaiser.RaiseEvent(gameId, () => new GameSetRegistered(configuration));
//    }

//    public async Task<ApiResult<Game>> UnregisterGameSet(Guid gameId, Guid gameSetId)
//    {
//        return await EventRaiser.RaiseEvent(gameId, () => new GameSetUnregistered(gameSetId));
//    }

//    public async Task<ApiResult<Game>> ConnectGameSet(Guid gameId, Guid gameSetId)
//    {
//        return await EventRaiser.RaiseEventWithChecks(gameId, game =>
//        {
//            if (game.Status != GameStatus.Initialized)
//            {
//                throw new InvalidStateException("Connecting GameSets is only possible when Game is initialized!");
//            }

//            if (game.GameSets.All(gs => gs.Id != gameSetId))
//            {
//#pragma warning disable S3928
//                throw new ArgumentException("Invalid GameSetId provided!", nameof(gameSetId));
//#pragma warning restore S3928
//            }

//            return new GameSetConnected(gameSetId);
//        });
//    }

//    public async Task<ApiResult<Game>> DisconnectGameSet(Guid gameId, Guid gameSetId)
//    {
//        return await EventRaiser.RaiseEventWithChecks(gameId, game =>
//        {
//            if (game.Status != GameStatus.Initialized)
//            {
//                throw new InvalidStateException("Disconnecting GameSets is only possible when Game is initialized!");
//            }

//            if (game.GameSets.All(gs => gs.Id != gameSetId))
//            {
//#pragma warning disable S3928
//                throw new ArgumentException("Invalid GameSetId provided!", nameof(gameSetId));
//#pragma warning restore S3928
//            }

//            return new GameSetDisconnected(gameSetId);
//        });
//    }

//    public async Task<ApiResult<Game>> CreateLobby(Guid gameId, int numberOfGroups)
//    {
//        var gameRoundId = Guid.NewGuid();
//        GameGroup[]? groups = null;

//        var task = await EventRaiser.RaiseEventWithChecks(gameId, game =>
//        {
//            if (game.Status != GameStatus.Initialized && game.Status != GameStatus.GameFinished)
//            {
//                throw new InvalidStateException("Lobby cannot be created currently!");
//            }

//            groups = game.GameSets
//                .Where(set => set.IsOnline)
//                .Select((s, i) => new { s, i })
//                .GroupBy(x => x.i % numberOfGroups)
//                .Select((g, i) => new GameGroup(
//                    Guid.NewGuid(),
//                    g.Select(x => x.s).ToArray(),
//                    GetGameColor(i)
//                ))
//                .ToArray();

//            return new LobbyCreated(gameRoundId, groups);
//        });

//        if (groups != null)
//        {
//            var gameRoundCommands = GrainFactory.GetGrain<IGameRoundCommands>(0);
//            await gameRoundCommands.CreateLobby(gameRoundId, groups);
//        }

//        return task;
//    }

//    public async Task<(ApiResult<Game>, ApiResult<GameRound>)> StartGameRound(Guid gameId)
//    {
//        var gameRoundId = Guid.NewGuid();

//        var game = await EventRaiser.RaiseEventWithChecks(gameId, game => new Started(gameRoundId))
//            ;
//        var gameRoundCommands = GrainFactory.GetGrain<IGameRoundCommands>(0);

//        try
//        {
//            if (game.Output == null)
//            {
//                throw new InvalidOperationException("GameRound ID not found!");
//            }

//            var gameRound = await gameRoundCommands.Start(gameRoundId);

//            if (gameRound.Output == null)
//            {
//                throw new InvalidOperationException("GameRound not correctly initialized");
//            }

//            return (game, gameRound);
//        }
//        catch (Exception e)
//        {
//            return (game, new ApiResult<GameRound>(e));
//        }
//    }

//    static GroupColor GetGameColor(int index)
//    {
//        if (index > 7)
//        {
//            throw new InvalidOperationException("We do only support up to 8 groups (colors)");
//        }

//        return (GroupColor)index;
//    }
//}