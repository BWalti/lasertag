using Orleans;

namespace Lasertag.DomainModel;

[GenerateSerializer]
public class Server
{
    [Id(0)] public int Id { get; set; } = 0;

    [Id(1)] public string Name { get; set; } = string.Empty;
}