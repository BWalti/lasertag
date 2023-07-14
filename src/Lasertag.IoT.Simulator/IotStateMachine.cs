using Admin.Api;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Lasertag.IoT.Simulator;

public class IotStateMachine : IHandleMessages
{
    readonly IMqttAdapter _mqttAdapter;
    readonly ILogger<IotStateMachine> _logger;
    readonly IOptions<SimulatorOptions> _simulatorOptions;

    public IotStateMachine(
        IMqttAdapter mqttAdapter,
        ILogger<IotStateMachine> logger,
        IOptions<SimulatorOptions> simulatorOptions)
    {
        _mqttAdapter = mqttAdapter;
        _logger = logger;
        _simulatorOptions = simulatorOptions;
    }

    public void ProcessMessage(string topic, string payload)
    {
        _logger.LogInformation("Got message from Mqtt: {topic}, payload: {payload}", topic, payload);
        // nothing to do for now!
    }

    public async Task Initialize()
    {
        var clientId = _simulatorOptions.Value.ClientId;

        await _mqttAdapter.RegisterListener(this, new[] { MqttTopics.AllMessagesForClient(clientId), MqttTopics.ToAllClients });
        await _mqttAdapter.SendMessageAsync(MqttTopics.ConnectGameSet, string.Empty);
    }
}