using JetBrains.Annotations;
using Lasertag.DomainModel;
using static Lasertag.DomainModel.DomainEvents.GameRoundEvents;

namespace Lasertag.Manager.GameRound;

[GenerateSerializer]
public class GameRoundState : DomainModel.GameRound
{
    [UsedImplicitly]
    public void Apply(LobbyCreated e)
    {
        Id = e.GameRoundId;
        GameSetGroups = e.GameSetGroups;

        Status = GameRoundStatus.Created;
    }

    [UsedImplicitly]
    public void Apply(GameSetActivated e)
    {
        ActiveGameSets.Add(new ActiveGameSet(e.PlayerId, e.GameSetId));
    }

    [UsedImplicitly]
    public void Apply(Started _)
    {
        Status = GameRoundStatus.Started;

        ////PlayerStats.AddRange(ActiveGameSets.Select(ags => new PlayerStats
        ////{
        ////    GameSetId = ags.GameSetId,
        ////    PlayerId = ags.PlayerId
        ////}));
    }

    [UsedImplicitly]
    public void Apply(PlayerFiredShot _)
    {
        ////ShotsFired += 1;
        ////var playerStatistics = PlayerStats.First(p => p.PlayerId == e.SourcePlayerId);
        ////playerStatistics.ShotsFired += 1;
    }

    [UsedImplicitly]
    public void Apply(PlayerGotHitBy _)
    {
        ////ShotsHit += 1;
        ////var target = PlayerStats.First(p => p.PlayerId == e.TargetPlayerId);
        ////target.GotHit += 1;

        ////var source = PlayerStats.First(p => p.PlayerId == e.SourcePlayerId);
        ////source.ShotsHit += 1;
    }
}