using JetBrains.Annotations;
using Lasertag.DomainModel;
using Lasertag.DomainModel.DomainEvents.GameEvents;
using Lasertag.DomainModel.DomainEvents.RoundEvents;
using Marten.Events.Aggregation;

namespace Lasertag.Manager;

public class ScoreBoardProjection : SingleStreamAggregation<ScoreBoard>
{
    [UsedImplicitly]
    public void Apply(ScoreBoard snapshot, GameRoundStarted e)
    {
        snapshot.Id = e.GameRoundId;
    }

    [UsedImplicitly]
    public void Apply(ScoreBoard snapshot, PlayerFiredShot e)
    {
        snapshot.ShotsFired += 1;
    }

    [UsedImplicitly]
    public void Apply(ScoreBoard snapshot, PlayerGotHitBy e)
    {
        snapshot.ShotsHit += 1;
    }
}