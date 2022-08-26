using Orleans;

namespace Lasertag.DomainModel.DomainEvents;

[GenerateSerializer]
public record GameSetConnected(Guid GameSetId) : GameEventBase;