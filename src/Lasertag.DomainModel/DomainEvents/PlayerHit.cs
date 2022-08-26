using Orleans;

namespace Lasertag.DomainModel.DomainEvents;

[GenerateSerializer]
public record PlayerHit : GameEventBase;