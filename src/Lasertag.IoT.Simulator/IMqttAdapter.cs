namespace Lasertag.IoT.Simulator;

public interface IMqttAdapter
{
    public Task SendMessageAsync(string topic, string payload);
    public Task RegisterListener(IHandleMessages messageHandler, string[] topics);
}