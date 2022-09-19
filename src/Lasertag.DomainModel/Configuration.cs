using Orleans;

namespace Lasertag.DomainModel;

[GenerateSerializer]
public class Configuration
{
    public List<LasertagSet> RegisteredLasertagSets { get; set; } = new();
}