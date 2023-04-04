namespace Admin.Api;

public static class MqttTopics
{
    public const string GameSetConnected = "client/connected";
    public const string GameSetActivated = "client/activated";
    public const string ShotFired = "client/shotFired";
}