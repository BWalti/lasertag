using Orleans;

namespace Lasertag.DomainModel.DomainEvents.GameEvents;

[GenerateSerializer]
public record GameSetActivated(Guid PlayerId, Guid GameSetId) : DomainEventBase;