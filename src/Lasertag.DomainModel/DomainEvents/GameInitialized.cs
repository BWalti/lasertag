using Orleans;

namespace Lasertag.DomainModel.DomainEvents;

[GenerateSerializer]
public record GameInitialized(Guid GameId) : GameEventBase;