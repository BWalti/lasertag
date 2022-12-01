namespace Lasertag.DomainModel;

[GenerateSerializer]
public class Player
{
    [Id(0)] public Guid Id { get; set; }

    [Id(1)] public string Nickname { get; set; } = string.Empty;
}