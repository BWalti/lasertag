using Orleans;

namespace Lasertag.DomainModel.DomainEvents.GameEvents;

[GenerateSerializer]
public record GameSetDisconnected(Guid GameSetId) : DomainEventBase;