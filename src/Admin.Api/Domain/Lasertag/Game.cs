using Marten.Events.Aggregation;

namespace Admin.Api.Domain.Lasertag;

public class GameStatistics
{
    public Guid Id { get; set; }
    public int Version { get; set; }

    public int ShotsFired { get; set; } = 0;
}

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

public class DummyGameEventsHandler
{
    public void Handle(LasertagEvents.GamePrepared prepared)
    {
        // do nothing
    }

    public void Handle(LasertagEvents.GameSetActivated @event)
    {
        // do nothing
    }

    public void Handle(LasertagEvents.GameStarted @event)
    {
        // do nothing
    }

    public void Handle(LasertagEvents.GameSetFiredShot @event)
    {
        // do nothing
    }

    public void Handle(LasertagEvents.GameSetGotHit @event)
    {
        // do nothing
    }

    public void Handle(LasertagEvents.GameFinished @event)
    {
        // do nothing
    }
}
public class Game
{
    public Guid Id { get; set; }
    public bool IsGameRunning { get; set; }

    public Lobby Lobby { get; set; } = new();

    public static Game Create(LasertagEvents.GamePrepared prepared) =>
        new()
        {
            Id = prepared.GameId,
            Lobby = prepared.Lobby
        };

    public void Apply(LasertagEvents.GameSetActivated @event)
    {
        // multiple times for each player
    }

    public void Apply(LasertagEvents.GameStarted @event)
    {
        IsGameRunning = true;
    }

    public void Apply(LasertagEvents.GameSetFiredShot @event)
    {
        // do something with this!
    }

    public void Apply(LasertagEvents.GameSetGotHit @event)
    {
        // do something with this!
    }

    public void Apply(LasertagEvents.GameFinished @event)
    {
        IsGameRunning = false;
    }
}