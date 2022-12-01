using Lasertag.DomainModel;
using Marten;
using Microsoft.Extensions.DependencyInjection;
using Orleans.Concurrency;

namespace Lasertag.Api;

[Reentrant]
[StatelessWorker]
public class GameRoundQueries : Grain, IGameRoundQueries
{
    public async Task<ApiResult<ScoreBoard>> GetStats(Guid gameRoundId)
    {
        var querySession = ServiceProvider.GetRequiredService<IQuerySession>();
        var scoreBoard = await querySession.Events.AggregateStreamAsync<ScoreBoard>(gameRoundId).ConfigureAwait(false);
        if (scoreBoard != null)
        {
            return new ApiResult<ScoreBoard>(scoreBoard);
        }

        return new ApiResult<ScoreBoard>("not found");
    }
}