using Lasertag.DomainModel;

namespace Lasertag.Api;

public interface IGameCommands
{
    Task<ApiResult<Game>> InitializeGame(Guid gameId);
    Task<ApiResult<Game>> RegisterGameSet(Guid gameId, GameSetConfiguration configuration);
    Task<ApiResult<Game>> UnregisterGameSet(Guid gameId, Guid gameSetId);
    Task<ApiResult<Game>> ConnectGameSet(Guid gameId, Guid gameSetId);
    Task<ApiResult<Game>> DisconnectGameSet(Guid gameId, Guid gameSetId);
    Task<ApiResult<Game>> CreateLobby(Guid gameId, int numberOfGroups);
    Task<(ApiResult<Game>, ApiResult<GameRound>)> StartGameRound(Guid gameId);
}

public interface IServerCommands
{
    Task<ApiResult<Server>> Configure(ServerConfiguration configuration);
    Task<ApiResult<Server>> RegisterGameSet(GameSetConfiguration gameSetConfiguration);
}