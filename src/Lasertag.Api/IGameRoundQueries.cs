using Lasertag.DomainModel;

namespace Lasertag.Api;

public interface IGameRoundQueries : IGrainWithIntegerKey
{
    Task<ApiResult<ScoreBoard>> GetStats(Guid gameRoundId);
}