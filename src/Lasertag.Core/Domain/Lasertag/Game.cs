﻿namespace Lasertag.Core.Domain.Lasertag;

public class Game
{
    public Guid Id { get; set; }
    public GameStatus Status { get; set; }
    public Guid Version { get; set; }

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
#pragma warning disable S6602
        var team = Lobby.Teams.FirstOrDefault(t => t.GameSets.Any(gs => gs.Id == @event.GameSetId));
#pragma warning restore S6602
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