namespace Lasertag.IoT.Simulator;

public interface IHandleMessages
{
    void ProcessMessage(string topic, string payload);
}