using Wolverine.Persistence.Sagas;

namespace Admin.Api.Domain.Lasertag;

public static class LasertagCommands
{
    public interface IServerCommands
    {
        public int ServerId { get; }
    }

    public interface IGameCommands
    {
        public int GameId { get; }
    }

    public record RegisterGameSet(int ServerId) : IServerCommands;
    public record PrepareGame(int ServerId, LobbyConfiguration Configuration) : IServerCommands;

    public record StartGame([property: SagaIdentity] int GameId, TimeSpan GameDuration) : IGameCommands;
    public record EndGame([property: SagaIdentity] int GameId) : IGameCommands;
    public record DeleteGame([property: SagaIdentity] int GameId) : IGameCommands;
}
