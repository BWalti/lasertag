namespace Lasertag.DomainModel.DomainEvents;

public static class GameRoundEvents
{
    [GenerateSerializer]
    public record LobbyCreated(Guid GameRoundId, GameGroup[] GameSetGroups) : DomainEventBase;

    [GenerateSerializer]
    public record GameSetActivated(Guid GameRoundId, Guid PlayerId, Guid GameSetId) : DomainEventBase;

    [GenerateSerializer]
    public record Started(Guid GameRoundId) : DomainEventBase;

    [GenerateSerializer]
    public record PlayerFiredShot
        (Guid GameRoundId, Guid SourceGameSetId, Guid SourcePlayerId, Guid SourceGroupId) : DomainEventBase,
            ISourcePlayerEvents;

    [GenerateSerializer]
    public record PlayerGotHitBy(Guid GameRoundId, Guid SourceGameSetId, Guid TargetGameSetId, Guid SourcePlayerId,
        Guid TargetPlayerId,
        Guid SourceGroupId, Guid TargetGroupId) : DomainEventBase, ISourcePlayerEvents, ITargetPlayerEvents;
}