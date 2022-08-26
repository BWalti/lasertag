using Orleans;

namespace Lasertag.DomainModel.DomainEvents;

[GenerateSerializer]
public record GameSetActivated(Guid PlayerId, Guid GameSetId) : GameEventBase;