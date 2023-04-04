using Wolverine.Persistence.Sagas;

namespace Admin.Api.Domain.Lasertag;

public static class LasertagCommands
{
    public interface IServerCommands
    {
        public Guid ServerId { get; }
    }

    public interface IGameCommands
    {
        public Guid GameId { get; }
    }

    public record CreateServer;
    public record RegisterGameSet(Guid ServerId) : IServerCommands;
    public record PrepareGame(Guid ServerId, LobbyConfiguration Configuration) : IServerCommands;

    public record StartGame([property: SagaIdentity] Guid GameId, TimeSpan GameDuration) : IGameCommands;
    public record EndGame([property: SagaIdentity] Guid GameId) : IGameCommands;
    public record DeleteGame([property: SagaIdentity] Guid GameId) : IGameCommands;
}
