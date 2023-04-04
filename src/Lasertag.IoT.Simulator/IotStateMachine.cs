using Admin.Api;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Lasertag.IoT.Simulator;

public class IotStateMachine : IHandleMessages
{
    readonly IMessageBus _bus;
    readonly ILogger<IotStateMachine> _logger;
    readonly IOptions<SimulatorOptions> _simulatorOptions;

    public IotStateMachine(IMessageBus bus, ILogger<IotStateMachine> logger,
        IOptions<SimulatorOptions> simulatorOptions)
    {
        _bus = bus;
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

        await _bus.RegisterListener(this, new[] { $"server/{clientId}/shotHit", "allClients" });
        await _bus.SendMessageAsync(MqttTopics.GameSetConnected, string.Empty);
    }
}