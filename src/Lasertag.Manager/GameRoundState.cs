﻿using JetBrains.Annotations;
using Lasertag.DomainModel;
using Lasertag.DomainModel.DomainEvents.GameEvents;
using Lasertag.DomainModel.DomainEvents.RoundEvents;
using Orleans;

namespace Lasertag.Manager;

[GenerateSerializer]
public class GameRoundState : GameRound
{
    [UsedImplicitly]
    public void Apply(GameRoundStarted e)
    {
        Id = e.GameRoundId;
        Status = GameRoundStatus.Started;
        ActiveGameSets = e.ActiveLasertagSets.ToList();
        GameSetGroups = e.Groups.ToArray();

        PlayerStats.AddRange(ActiveGameSets.Select(ags => new PlayerStats
        {
            GameSetId = ags.GameSetId,
            PlayerId = ags.PlayerId
        }));
    }

    [UsedImplicitly]
    public void Apply(PlayerFiredShot e)
    {
        ShotsFired += 1;
        var playerStatistics = PlayerStats.First(p => p.PlayerId == e.SourcePlayerId);
        playerStatistics.ShotsFired += 1;
    }

    [UsedImplicitly]
    public void Apply(PlayerGotHit e)
    {
        ShotsHit += 1;
        var sourcePlayerStatistics = PlayerStats.First(p => p.PlayerId == e.SourcePlayerId);
        sourcePlayerStatistics.ShotsHit += 1;

        var targetPlayerStatistics = PlayerStats.First(p => p.PlayerId == e.TargetPlayerId);
        targetPlayerStatistics.GotHit += 1;
    }
}