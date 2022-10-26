using Orleans;

namespace Lasertag.DomainModel;

[GenerateSerializer]
public record GameGroup(Guid GroupId, GameSet[] GameSets, GroupColor Color);