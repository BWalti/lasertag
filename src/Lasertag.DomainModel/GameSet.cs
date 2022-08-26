using Orleans;

namespace Lasertag.DomainModel;

[GenerateSerializer]
public record GameSet(Guid GameSetId);