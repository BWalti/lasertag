using Orleans;

namespace Lasertag.DomainModel.DomainEvents;

[GenerateSerializer]
public record GameSetDisconnected(Guid GameSetId) : GameEventBase;