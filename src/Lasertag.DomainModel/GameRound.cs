using JetBrains.Annotations;

namespace Lasertag.DomainModel;

[GenerateSerializer]
public class GameRound
{
    [Id(0)] public Guid Id { get; set; }

    [Id(1)] public GameGroup[] GameSetGroups { get; set; } = Array.Empty<GameGroup>();

    [UsedImplicitly] [Id(2)] public int Version { get; set; }

    [Id(3)] public GameRoundStatus Status { get; set; }

#pragma warning disable MA0016
    [Id(4)] public List<ActiveGameSet> ActiveGameSets { get; set; } = new();
#pragma warning restore MA0016
}