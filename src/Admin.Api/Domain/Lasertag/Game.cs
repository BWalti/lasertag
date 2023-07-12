using JasperFx.Core;
using Marten.Metadata;

namespace Admin.Api.Domain.Lasertag;

public enum GameStatus
{
    Created,
    ReadyToStart,
    Started,
    Finished
}

public class Game : IVersioned, ITracked
{
    public Guid Id { get; set; }
    public GameStatus Status { get; set; }

    public Guid Version { get; set; }
    public string CorrelationId { get; set; } = string.Empty;
    public string CausationId { get; set; } = string.Empty;
    public string LastModifiedBy { get; set; } = string.Empty;

    public Lobby Lobby { get; set; } = new();

    public static Game Create(LasertagEvents.GamePrepared prepared) =>
        new()
        {
            Id = prepared.GameId,
            Lobby = prepared.Lobby,
            Status = GameStatus.Created
        };

    public void Apply(LasertagEvents.GameSetActivated @event)
    {
        var team = Lobby.Teams.FindFirst(t => t.GameSets.Any(gs => gs.Id == @event.GameSetId));
        if (team != null)
        {
            var gameSet = team.GameSets.First(gs => gs.Id == @event.GameSetId);
            gameSet.IsActive = true;
        }

        if (Status != GameStatus.ReadyToStart)
        {
            var atLeastTwoTeamsHaveActivePlayers = Lobby.Teams.Count(t => t.GameSets.Any(gs => gs.IsActive)) >= 2;
            if (atLeastTwoTeamsHaveActivePlayers)
            {
                Status = GameStatus.ReadyToStart;
            }
        }
    }

    public void Apply(LasertagEvents.GameStarted @event)
    {
        Status = GameStatus.Started;
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
        Status = GameStatus.Finished;
    }
}