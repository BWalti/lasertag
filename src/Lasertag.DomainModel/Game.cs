using Orleans;

namespace Lasertag.DomainModel;

[GenerateSerializer]
public class Game
{
    [Id(0)] public Guid GameId { get; set; }

    [Id(1)] public List<GameSet> ConnectedGameSets { get; set; } = new();
    
    [Id(2)] public List<ActiveGameSet> ActiveGameSets { get; set; } = new();

    [Id(4)] public GameSetGroup[] GameSetGroups { get; set; } = { };

    [Id(5)] public GameStateEnum State { get; set; } = GameStateEnum.None;
}

public enum GameStateEnum : short
{
    None = -1,

    Initialized = 0,

    LobyOpened = 1,
    GameStarted = 2,
    GameFinished = 3
}