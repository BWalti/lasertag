using Wolverine.Persistence.Sagas;

namespace Admin.Api.Domain.Lasertag;

public static class LasertagEvents
{
    public interface IServerEvents
    {
        public Guid ServerId { get; }
    }

    public interface IGameEvents
    {
        [SagaIdentity] public Guid GameId { get; }
    }

    public record ServerCreated(Guid ServerId) : IServerEvents;
    public record GameSetRegistered(Guid ServerId, int GameSetId) : IServerEvents;
    public record GameSetConnected(Guid ServerId, int GameSetId) : IServerEvents;
    public record GameSetPinged(Guid ServerId, int GameSetId) : IServerEvents;

    public record GamePrepared(Guid GameId, Lobby Lobby) : IGameEvents;
    public record GameSetActivated(Guid GameId, int GameSetId, int PlayerId) : IGameEvents;
    public record GameStarted(Guid GameId) : IGameEvents;
    public record GameSetFiredShot(Guid GameId, int GameSetId) : IGameEvents;
    public record GameSetGotHit(Guid GameId, int ShotSourceGameSetId, int GameSetId) : IGameEvents;
    public record GameFinished(Guid GameId) : IGameEvents;
}