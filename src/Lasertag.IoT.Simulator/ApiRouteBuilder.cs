using Admin.Api.Domain.Lasertag;

namespace Lasertag.IoT.Simulator;

public static class ApiRouteBuilder
{
    public static string RegisterGameSet(Server server) =>
        $"/api/lasertag/server/{server.Id}/registerGameSet";

    public static string PrepareGame(Server server) =>
        $"/api/lasertag/server/{server.Id}/prepareGame";

    public static string StartGame(Game game, TimeSpan gameDuration) =>
        $"/api/lasertag/game/{game.Id}/start?gameDuration={gameDuration}";

    public static string DeleteGame(Game game) =>
        $"/api/lasertag/game/{game.Id}";
}