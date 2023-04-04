using Admin.Api.Domain.Lasertag;
using Microsoft.AspNetCore.Mvc;
using Wolverine;
using static Admin.Api.Domain.Lasertag.LasertagEvents;

namespace Admin.Api;

public static class LasertagApiExtensions
{
    public static RouteGroupBuilder MapLasertagApi(this RouteGroupBuilder group)
    {
        group.MapPost("/server",
            (IMessageBus bus) => { return bus.InvokeAsync<Server>(new LasertagCommands.CreateServer()); });

        group.MapPost("/server/{serverId}/registerGameSet",
            (IMessageBus bus, [FromRoute] Guid serverId) =>
                bus.InvokeAsync<GameSetRegistered>(new LasertagCommands.RegisterGameSet(serverId)));

        group.MapPost("/server/{serverId}/prepareGame",
            (IMessageBus bus, [FromRoute] Guid serverId, [FromBody] LobbyConfiguration lobbyConfiguration) =>
                bus.InvokeAsync(new LasertagCommands.PrepareGame(serverId, lobbyConfiguration)));

        group.MapPost("/game/{gameId}/start",
            (IMessageBus bus, [FromRoute] Guid gameId, [FromQuery] TimeSpan ? gameDuration) =>
                bus.InvokeAsync(new LasertagCommands.StartGame(gameId, gameDuration ?? TimeSpan.FromMinutes(10))));

        group.MapPost("/game/{gameId}/end",
            (IMessageBus bus, [FromRoute] Guid gameId) =>
                bus.InvokeAsync(new LasertagCommands.EndGame(gameId)));

        group.MapDelete("/game/{gameId}",
            (IMessageBus bus, [FromRoute] Guid gameId) => bus.InvokeAsync(new LasertagCommands.DeleteGame(gameId)));

        return group;
    }
}