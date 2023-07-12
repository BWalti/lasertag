using Marten.Metadata;

namespace Admin.Api.Domain.Lasertag;

public enum ServerStatus
{
    /// <summary>
    /// Server created
    /// </summary>
    Created,

    /// <summary>
    /// Marks that this server would be ready for games,
    /// meaning it has at least two GameSets registered
    /// and currently no game is prepared / running.
    /// </summary>
    ReadyForLobby,

    /// <summary>
    /// Lobby created.
    /// </summary>
    GamePrepared,

    /// <summary>
    /// After the game is finished, the server state switches back to ReadyForLobby.
    /// </summary>
    GameRunning,
}

public class Server : IVersioned, ITracked
{
    public Guid Id { get; set; }
    public Guid Version { get; set; }
    public string CorrelationId { get; set; } = string.Empty;
    public string CausationId { get; set; } = string.Empty;
    public string LastModifiedBy { get; set; } = string.Empty;
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