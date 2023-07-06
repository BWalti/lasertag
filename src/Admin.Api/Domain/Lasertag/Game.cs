using Marten.Metadata;

namespace Admin.Api.Domain.Lasertag;

public class Game : IVersioned, ITracked
{
    public Guid Id { get; set; }
    public bool IsGameRunning { get; set; }

    public Guid Version { get; set; }
    public string CorrelationId { get; set; } = string.Empty;
    public string CausationId { get; set; } = string.Empty;
    public string LastModifiedBy { get; set; } = string.Empty;

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