using Orleans;

namespace Lasertag.DomainModel;

[GenerateSerializer]
public class Game
{
    [Id(0)] public Guid GameId { get; set; }

    [Id(1)] public List<GameSet> GameSets { get; set; } = new();

    [Id(5)] public GameStatus Status { get; set; } = GameStatus.None;

    [Id(6)] public Guid ActiveRoundId { get; set; }
}