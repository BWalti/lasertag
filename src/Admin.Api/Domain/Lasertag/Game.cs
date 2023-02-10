using Wolverine;

namespace Admin.Api.Domain.Lasertag;

public class Game : Saga
{
    public int Id { get; set; }
    public bool IsGameRunning { get; set; }

    public Lobby Lobby { get; set; } = new();

    public void Start(LasertagEvents.GamePrepared prepared)
    {
        Id = prepared.GameId;
        Lobby = prepared.Lobby;
    }

    public async Task<LasertagEvents.GameStarted> Handle(LasertagCommands.StartGame @event, IMessageBus bus)
    {
        IsGameRunning = true;

        var finished = new LasertagEvents.GameFinished(Id, @event.GameDuration);
        await bus.PublishAsync(finished);

        return new LasertagEvents.GameStarted(Id);
    }

    public void Handle(LasertagEvents.GameSetActivated @event)
    {
        // multiple times for each player
    }

    public void Handle(LasertagEvents.GameStarted @event)
    {
        // do something with this!
    }

    public void Handle(LasertagEvents.PlayerFiredShot @event)
    {
        // do something with this!
    }

    public void Handle(LasertagEvents.PlayerGotHit @event)
    {
        // do something with this!
    }

    public void Handle(LasertagEvents.GameFinished @event)
    {
        IsGameRunning = false;
    }

    public void Handle(LasertagCommands.DeleteGame _)
    {
        MarkCompleted();
    }
}