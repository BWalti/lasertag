using Lasertag.DomainModel;
using Orleans;

namespace Lasertag.Api;

public interface IGameCommands : IGrainWithIntegerKey
{
    Task<ApiResult<Game>> InitializeGame(Guid gameId);
    Task<ApiResult<Game>> ConnectGameSet(Guid gameId, Guid gameSetId);
    Task<ApiResult<Game>> ActivateGameSet(Guid gameId, Guid gameSetId, Guid playerId);
    Task<ApiResult<Game>> CreateLobby(Guid gameId, int numberOfGroups);
    Task<ApiResult<Game>> StartGame(Guid gameId);
}