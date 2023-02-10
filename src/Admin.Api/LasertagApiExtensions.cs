using Admin.Api.Domain.Lasertag;
using Marten;
using Microsoft.AspNetCore.Mvc;
using Wolverine;
using static Admin.Api.Domain.Lasertag.LasertagEvents;

namespace Admin.Api;

public static class LasertagApiExtensions
{
    public static RouteGroupBuilder MapLasertagApi(this RouteGroupBuilder group)
    {
        group.MapPost("/server",
            async (IDocumentSession session) =>
            {
                var account = new Server();

                session.Store(account);
                await session.SaveChangesAsync();

                return account;
            });

        group.MapPost("/server/{serverId}/registerGameSet",
            (IMessageBus bus, [FromRoute] int serverId) => bus.InvokeAsync<GameSetRegistered>(new LasertagCommands.RegisterGameSet(serverId)));

        group.MapPost("/server/{serverId}/prepareGame",
            (IMessageBus bus, [FromRoute] int serverId, [FromBody] LobbyConfiguration lobbyConfiguration) => bus.PublishAsync(new LasertagCommands.PrepareGame(serverId, lobbyConfiguration)));

        group.MapPost("/game/{gameId}/start",
            (IMessageBus bus, [FromRoute] int gameId, [FromQuery] TimeSpan ? gameDuration) => bus.PublishAsync(new LasertagCommands.StartGame(gameId, gameDuration ?? TimeSpan.FromMinutes(10))));

        group.MapPost("/game/{gameId}/end",
            (IMessageBus bus, [FromRoute] int gameId) => bus.PublishAsync(new LasertagCommands.EndGame(gameId)));

        group.MapDelete("/game/{gameId}",
            (IMessageBus bus, [FromRoute] int gameId) => bus.PublishAsync(new LasertagCommands.DeleteGame(gameId)));


        return group;
    }
}