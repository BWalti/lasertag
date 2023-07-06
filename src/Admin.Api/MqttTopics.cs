namespace Admin.Api;

public static class MqttTopics
{
    public const string GameSetConnected = "client/connected";
    public const string GameSetActivated = "client/activated";
    public const string ShotFired = "client/shotFired";
    public const string GameSetGotHit = "client/hit";

    public const string EverythingFromAnyClient = "client/#";
    public const string ToAllClients = "allClient";

    public const string ServerMessages = "server";

    public static string AllMessagesForClient(int clientId)
    {
        return $"server/{clientId}/#";
    }
}