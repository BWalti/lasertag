namespace Lasertag.Core.Domain.Lasertag;

public static class LasertagCommands
{
    public interface IServerCommands : IHasServerId
    {
    }

    public interface IGameCommands : IHasGameId
    {
    }

    public record CreateServer(string Name);
    public record RegisterGameSet(Guid ServerId) : IServerCommands;
    public record PrepareGame(Guid ServerId, LobbyConfiguration Configuration) : IServerCommands;
    public record StartGame(Guid ServerId, Guid GameId, TimeSpan GameDuration) : IServerCommands, IGameCommands;
    public record EndGame(Guid ServerId, Guid GameId) : IServerCommands, IGameCommands;
    public record DeleteGame(Guid GameId) : IGameCommands;
}
