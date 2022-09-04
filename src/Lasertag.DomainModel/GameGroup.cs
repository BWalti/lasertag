using Orleans;

namespace Lasertag.DomainModel;

[GenerateSerializer]
public record GameGroup(Guid GroupId, LasertagSet[] GameSets, GroupColor Color)
{
}