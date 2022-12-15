namespace Lasertag.DomainModel;

[GenerateSerializer]
public class GameConfiguration
{
    [Id(0)] public Guid Id { get; set; }
}