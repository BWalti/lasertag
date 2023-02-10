using Wolverine;
using Wolverine.Persistence.Sagas;

namespace Admin.Api.Domain.Lasertag;

public static class LasertagEvents
{
    public interface IServerEvents
    {
        public int ServerId { get; }
    }

    public interface IGameEvents
    {
        [SagaIdentity]
        public int GameId { get; }
    }

    public record GameSetRegistered(int ServerId, int GameSetId) : IServerEvents;
    public record GameSetPinged(int ServerId, int GameSetId) : IServerEvents;

    public record GamePrepared([property: SagaIdentity] int GameId, Lobby Lobby) : IGameEvents;
    public record GameSetActivated([property: SagaIdentity] int GameId, int PlayerId) : IGameEvents;
    public record GameStarted([property: SagaIdentity] int GameId) : IGameEvents;
    public record PlayerFiredShot([property: SagaIdentity] int GameId, int PlayerId) : IGameEvents;
    public record PlayerGotHit([property: SagaIdentity] int GameId, int ShotSourcePlayerId, int PlayerId) : IGameEvents;
    public record GameFinished([property: SagaIdentity] int GameId, TimeSpan DelayTime) : TimeoutMessage(DelayTime), IGameEvents;
}