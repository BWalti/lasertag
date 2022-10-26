using Orleans;

namespace Lasertag.DomainModel;

[GenerateSerializer]
public class GameSetConfiguration
{
    [Id(0)] public Guid Id { get; set; }

    [Id(1)] public bool IsTargetOnly { get; set; }
}