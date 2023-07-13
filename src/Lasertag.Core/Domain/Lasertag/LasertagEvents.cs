using Wolverine.Persistence.Sagas;

namespace Lasertag.Core.Domain.Lasertag;

public static class LasertagEvents
{
    public interface IServerEvents : IHasServerId
    {
    }

    public interface IGameEvents
    {
        [SagaIdentity] public Guid GameId { get; }
    }

    public record ServerCreated(string Name);
    public record GameSetRegistered(Guid ServerId, int GameSetId) : IServerEvents;
    public record GameSetConnected(Guid ServerId, int GameSetId) : IServerEvents;
    public record GameSetPinged(Guid ServerId, int GameSetId) : IServerEvents;

    public record GamePrepared(Guid ServerId, Guid GameId, Lobby Lobby) : IServerEvents, IGameEvents;
    public record GameSetActivated(Guid GameId, int GameSetId, int PlayerId) : IGameEvents;
    public record GameStarted(Guid ServerId, Guid GameId) : IServerEvents, IGameEvents;
    public record GameSetFiredShot(Guid GameId, int GameSetId) : IGameEvents;
    public record GameSetGotHit(Guid GameId, int ShotSourceGameSetId, int GameSetId) : IGameEvents;
    public record GameFinished(Guid ServerId, Guid GameId) : IServerEvents, IGameEvents;
}