using Marten.Events.Aggregation;

namespace Lasertag.Core.Domain.Lasertag;

public class GameStatisticsProjection : SingleStreamProjection<GameStatistics>
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

    public void Apply(LasertagEvents.GameSetGotHit @event, GameStatistics statistics)
    {
        // do something with this!
        statistics.GotHit++;
    }
}