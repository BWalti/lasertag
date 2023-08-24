namespace Lasertag.Core.Domain.Lasertag;

public class Server
{
    public Guid Id { get; set; }
    public int Version { get; set; }
    public ServerStatus Status { get; set; }
    public Guid? CurrentGameId { get; set; }

    public string Name { get; set; } = "Lasertag Server";
    public List<GameSet> GameSets { get; set; } = new();

    public static Server Create(LasertagEvents.ServerCreated @event) =>
        new()
        {
            Name = @event.Name
        };

    public void Apply(LasertagEvents.GameSetRegistered @event)
    {
        GameSets.Add(new GameSet(@event.GameSetId));
        if (Status == ServerStatus.Created && GameSets.Count > 1)
        {
            Status = ServerStatus.ReadyForLobby;
        }
    }

    public void Apply(LasertagEvents.GameSetConnected @event)
    {
        var gameSet = GameSets.Find(gs => gs.Id == @event.GameSetId);
        if (gameSet == null)
        {
            throw new ArgumentOutOfRangeException(nameof(@event),
                $"GameSet with ID {@event.GameSetId} is unknown to server {@event.ServerId}");
        }

        gameSet.IsConnected = true;
    }

    public void Apply(LasertagEvents.GamePrepared @event)
    {
        Status = ServerStatus.GamePrepared;
        CurrentGameId = @event.GameId;
    }

    public void Apply(LasertagEvents.GameStarted @event)
    {
        Status = ServerStatus.GameRunning;
    }

    public void Apply(LasertagEvents.GameFinished @event)
    {
        Status = ServerStatus.ReadyForLobby;
        CurrentGameId = null;
    }
}