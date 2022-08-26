using Orleans;

namespace Lasertag.DomainModel.DomainEvents;

[GenerateSerializer]
public record GameFinished : GameEventBase;