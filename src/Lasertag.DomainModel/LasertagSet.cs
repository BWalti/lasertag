using Orleans;

namespace Lasertag.DomainModel;

[GenerateSerializer]
public record LasertagSet(Guid Id, string Name);