namespace Admin.Api.Domain.Lasertag;

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