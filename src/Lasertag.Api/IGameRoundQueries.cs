using Lasertag.DomainModel;

namespace Lasertag.Api;

public interface IGameRoundQueries
{
    Task<ApiResult<ScoreBoard>> GetStats(Guid gameRoundId);
    Task<ApiResult<GameRound>> GetGameRound(Guid gameRoundId);
}