namespace Lasertag.DomainModel;

[GenerateSerializer]
public class ScoreBoard
{
    [Id(0)] public Guid Id { get; set; }
    [Id(1)] public int ShotsFired { get; set; }
    [Id(2)] public int ShotsHit { get; set; }
}