namespace Lasertag.IoT.Simulator;

public static class ApiRouteBuilder
{

    public const string GetServerByIdPathTemplate = "/server/{id}";
    public const string GetGameByIdPathTemplate = "/game/{id}";

    public const string CreateServerPath = "/server";
    public const string RegisterGameSetPath = "/server/registerGameSet";
    public const string PrepareGamePath = "/server/prepareGame";
    public const string DeleteGamePath = "/game/delete";
    public const string StartGamePath = "/game/start";
    public const string StopGamePath = "/game/stop";

    public static string GetServerById(string id) =>
        GetServerByIdPathTemplate.Replace("{id}", id);

    public static string GetGameById(string id) =>
        GetGameByIdPathTemplate.Replace("{id}", id);
}