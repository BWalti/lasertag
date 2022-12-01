using JetBrains.Annotations;
using Lasertag.DomainModel;
using Marten.Events.Aggregation;
using static Lasertag.DomainModel.DomainEvents.GameRoundEvents;

namespace Lasertag.Manager;

public class ScoreBoardProjection : SingleStreamAggregation<ScoreBoard>
{
    [UsedImplicitly]
    public void Apply(ScoreBoard snapshot, Started e)
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