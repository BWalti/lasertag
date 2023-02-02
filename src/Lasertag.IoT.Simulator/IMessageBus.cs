namespace Lasertag.IoT.Simulator;

public interface IMessageBus
{
    public Task SendMessageAsync(string topic, string payload);
    public Task RegisterListener(IHandleMessages messageHandler, string[] topics);
}