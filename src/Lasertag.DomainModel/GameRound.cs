using JetBrains.Annotations;
using Orleans;

namespace Lasertag.DomainModel;

[GenerateSerializer]
public class GameRound
{
    [Id(0)] public Guid Id { get; set; }

    [Id(1)] public int ShotsFired { get; set; }

    [Id(2)] public List<PlayerStats> PlayerStats { get; set; } = new();

    [Id(3)] public int ShotsHit { get; set; }

    [UsedImplicitly]
    [Id(4)] public int Version { get; set; }

    [Id(5)] public GameRoundStatus Status { get; set; }

    [Id(6)] public List<ActiveGameSet> ActiveGameSets { get; set; } = new();

    [Id(7)] public GameGroup[] GameSetGroups { get; set; } = { };
}