using Orleans;

namespace Lasertag.DomainModel;

[GenerateSerializer]
public class Game
{
    [Id(0)] public Guid GameId { get; set; }

    [Id(1)] public List<LasertagSet> ConnectedGameSets { get; set; } = new();

    [Id(2)] public List<ActiveGameSet> ActiveGameSets { get; set; } = new();

    [Id(4)] public GameGroup[] GameSetGroups { get; set; } = { };

    [Id(5)] public GameStatus Status { get; set; } = GameStatus.None;

    [Id(6)] public Guid ActiveRoundId { get; set; }
}