using Lasertag.DomainModel;
using Lasertag.Manager.GameRound;
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
        var scoreBoard = await querySession.Events.AggregateStreamAsync<ScoreBoard>(gameRoundId);
        if (scoreBoard != null)
        {
            return new ApiResult<ScoreBoard>(scoreBoard);
        }

        return new ApiResult<ScoreBoard>("not found");
    }

    public async Task<ApiResult<GameRound>> GetGameRound(Guid gameRoundId)
    {
        var querySession = ServiceProvider.GetRequiredService<IQuerySession>();
        var gameRound = await querySession.Events.AggregateStreamAsync<GameRoundState>(gameRoundId);
        if (gameRound != null)
        {
            return new ApiResult<GameRound>(gameRound);
        }

        return new ApiResult<GameRound>("not found");
    }
}