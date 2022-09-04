using Orleans;

namespace Lasertag.DomainModel;

[GenerateSerializer]
public record PlayerStats
{
    [Id(0)] public Guid PlayerId { get; set; }

    [Id(1)] public Guid GameSetId { get; set; }

    [Id(2)] public int ShotsFired { get; set; }

    [Id(3)] public int GotHit { get; set; }

    [Id(4)] public int ShotsHit { get; set; }

    public double HitRatio => ShotsFired > 0 ? ShotsHit / ShotsFired : 0;
}