using Orleans;

namespace Lasertag.DomainModel.DomainEvents.RoundEvents;

[GenerateSerializer]
public record PlayerFiredShot(Guid SourceGameSetId, Guid SourcePlayerId, Guid SourceGroupId) : DomainEventBase, ISourcePlayerEvents;