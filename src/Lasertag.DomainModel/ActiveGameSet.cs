using Orleans;

namespace Lasertag.DomainModel;

[GenerateSerializer]
public record ActiveGameSet(Guid PlayerId, Guid GameSetId);