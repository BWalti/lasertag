namespace Grains;

public class GameManagerState
{
    public Guid Id { get; set; }

    public GameManagerState()
    {
        OnlineGameSets = new();
    }

    public HashSet<Guid> OnlineGameSets { get; }
    
    public void Apply(GameSetOnline @event)
    {
        OnlineGameSets.Add(@event.Id);
    }

    public void Apply(GameSetOffline @event)
    {
        OnlineGameSets.Remove(@event.Id);
    }
}