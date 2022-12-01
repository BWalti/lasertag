namespace Lasertag.DomainModel.DomainEvents;

public static class InfrastructureEvents
{
    [GenerateSerializer]
    public record GameServerStarting : DomainEventBase;

    [GenerateSerializer]
    public record GameServerStarted : DomainEventBase;

    [GenerateSerializer]
    public record GameServerInitialized(Guid GameId) : DomainEventBase;

    [GenerateSerializer]
    public record GameSetRegistered(GameSetConfiguration Configuration) : DomainEventBase;

    [GenerateSerializer]
    public record GameSetConnected(Guid GameSetId) : DomainEventBase;

    [GenerateSerializer]
    public record GameSetDisconnected(Guid GameSetId) : DomainEventBase;

    [GenerateSerializer]
    public record GameSetUnregistered(Guid GameSetId) : DomainEventBase;
}