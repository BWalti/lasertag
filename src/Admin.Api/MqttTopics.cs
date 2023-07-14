namespace Admin.Api;

public static class MqttTopics
{
    public const string ConnectGameSet = "client/connected";
    public const string ActivateGameSet = "client/activated";
    public const string FireShot = "client/shotFired";
    public const string GameSetGotHit = "client/hit";

    public const string EverythingFromAnyClient = "client/#";
    public const string ToAllClients = "allClient";

    public const string ServerMessages = "server";

    public static string AllMessagesForClient(int clientId)
    {
        return $"server/{clientId}/#";
    }
}