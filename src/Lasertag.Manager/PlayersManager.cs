using Lasertag.DomainModel;
using Marten;
using Orleans;

namespace Lasertag.Manager;

public class PlayersManager : Grain, IPlayersManager
{
    readonly IQuerySession _querySession;

    public PlayersManager(IQuerySession querySession)
    {
        _querySession = querySession;
    }

    public async Task<GameRound?> GetResult()
    {
        var gameId = this.GetPrimaryKey();
        var playersStatistics = await _querySession.Events.AggregateStreamAsync<GameRoundState>(gameId);

        return playersStatistics;
    }
}