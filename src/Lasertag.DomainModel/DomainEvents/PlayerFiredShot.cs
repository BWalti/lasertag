using Orleans;

namespace Lasertag.DomainModel.DomainEvents;

[GenerateSerializer]
public record PlayerFiredShot : GameEventBase;