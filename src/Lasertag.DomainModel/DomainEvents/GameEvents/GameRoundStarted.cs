using Orleans;

namespace Lasertag.DomainModel.DomainEvents.GameEvents;

[GenerateSerializer]
public record GameRoundStarted(Guid GameRoundId, IEnumerable<ActiveGameSet> ActiveLasertagSets, IEnumerable<GameGroup> Groups) : DomainEventBase;