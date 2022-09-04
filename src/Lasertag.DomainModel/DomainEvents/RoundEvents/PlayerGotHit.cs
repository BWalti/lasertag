using Orleans;

namespace Lasertag.DomainModel.DomainEvents.RoundEvents;

[GenerateSerializer]
public record PlayerGotHit(Guid SourceGameSetId, Guid TargetGameSetId, Guid SourcePlayerId, Guid TargetPlayerId,
    Guid SourceGroupId, Guid TargetGroupId) : DomainEventBase, ISourcePlayerEvents, ITargetPlayerEvents;