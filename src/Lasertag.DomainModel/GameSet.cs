namespace Lasertag.DomainModel;

[GenerateSerializer]
public class GameSet
{
    [Id(0)] public Guid Id { get; set; }

    [Id(1)] public bool IsOnline { get; set; }

    [Id(2)] public GameSetConfiguration? Configuration { get; set; }

    [Id(3)] public string Name { get; set; } = string.Empty;
}