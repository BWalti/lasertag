﻿using JasperFx.Core;

namespace Lasertag.Core.Domain.Lasertag;

public class Game
{
    public Guid Id { get; set; }
    public GameStatistics Statistics { get; } = new();
    public GameStatus Status { get; set; }
    public Lobby Lobby { get; set; } = new();

    public static Game Create(LasertagEvents.GamePrepared prepared)
    {
        var game = new Game
        {
            Id = prepared.GameId,
            Lobby = prepared.Lobby,
            Status = GameStatus.Created
        };

        game.Statistics.Apply(prepared);

        return game;
    }

    public void Apply(LasertagEvents.GameSetActivated @event)
    {
        var team = Lobby.Teams.FindFirst(t => t.Value.GameSets.Any(gs => gs.Id == @event.GameSetId));
        if (team.Value != null)
        {
            var gameSet = team.Value.GameSets.First(gs => gs.Id == @event.GameSetId);
            gameSet.IsActive = true;
        }

        if (Status == GameStatus.Created)
        {
            var atLeastTwoTeamsHaveActivePlayers = Lobby.Teams.Count(t => t.Value.GameSets.Any(gs => gs.IsActive)) >= 2;
            if (atLeastTwoTeamsHaveActivePlayers)
            {
                Status = GameStatus.ReadyToStart;
            }
        }
    }

    public void Apply(LasertagEvents.GameSetFiredShot @event)
    {
        Statistics.Apply(@event);
    }

    public void Apply(LasertagEvents.GameSetGotHit @event)
    {
        Statistics.Apply(@event);
    }

    public void Apply(LasertagEvents.GameStarted @event)
    {
        Status = GameStatus.Started;
    }

    public void Apply(LasertagEvents.GameFinished @event)
    {
        Status = GameStatus.Finished;
    }
}