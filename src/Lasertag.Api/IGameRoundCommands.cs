using Lasertag.DomainModel;
using Orleans;

namespace Lasertag.Api;

public interface IGameRoundCommands : IGrainWithIntegerKey
{
    Task<ApiResult<GameRound>> CreateLobby(Guid gameRoundId, GameGroup[] gameSetGroups);

    Task<ApiResult<GameRound>> ActivateGameSet(Guid gameRoundId, Guid gameSetId, Guid playerId);

    Task<ApiResult<GameRound>> Start(Guid gameRoundId);

    Task<ApiResult<GameRound>> Fire(Guid gameRoundId, Guid sourceLasertagSetId);
    Task<ApiResult<GameRound>> Hit(Guid gameRoundId, Guid sourceLasertagSetId, Guid targetGameSetId);
}