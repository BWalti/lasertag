﻿using Wolverine.Persistence.Sagas;

namespace Lasertag.Core.Domain.Lasertag;

public static class LasertagCommands
{
    public interface IServerCommands : IHasServerId
    {
    }

    public interface IGameCommands
    {
        public Guid GameId { get; }
    }

    public record CreateServer(string Name);

    public record RegisterGameSet(Guid ServerId) : IServerCommands;
    public record PrepareGame(Guid ServerId, LobbyConfiguration Configuration) : IServerCommands;

    public record StartGame(Guid ServerId, [property: SagaIdentity] Guid GameId, TimeSpan GameDuration) : IServerCommands, IGameCommands;
    public record EndGame(Guid ServerId, [property: SagaIdentity] Guid GameId) : IServerCommands, IGameCommands;
    public record DeleteGame([property: SagaIdentity] Guid GameId) : IGameCommands;
}