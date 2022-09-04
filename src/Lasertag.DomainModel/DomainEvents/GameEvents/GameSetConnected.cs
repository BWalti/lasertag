using Orleans;

namespace Lasertag.DomainModel.DomainEvents.GameEvents;

[GenerateSerializer]
public record GameSetConnected(Guid GameSetId) : DomainEventBase;