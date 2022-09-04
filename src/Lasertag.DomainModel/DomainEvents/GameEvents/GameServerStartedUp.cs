using Orleans;

namespace Lasertag.DomainModel.DomainEvents.GameEvents;

[GenerateSerializer]
public record GameServerStartedUp : DomainEventBase;