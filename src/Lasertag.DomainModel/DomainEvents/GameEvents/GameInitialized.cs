using Orleans;

namespace Lasertag.DomainModel.DomainEvents.GameEvents;

[GenerateSerializer]
public record GameInitialized(Guid GameId) : DomainEventBase;