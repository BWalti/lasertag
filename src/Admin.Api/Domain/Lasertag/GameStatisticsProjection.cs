using Marten.Events.Aggregation;

namespace Admin.Api.Domain.Lasertag;

public class GameStatisticsProjection : SingleStreamAggregation<GameStatistics>
{
    public GameStatistics Create(LasertagEvents.GamePrepared @event) =>
        new()
        {
            Id = @event.GameId
        };

    public void Apply(LasertagEvents.GameSetFiredShot @event, GameStatistics statistics)
    {
        statistics.ShotsFired++;
    }
}