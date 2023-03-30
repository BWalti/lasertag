using System.Text;
using MQTTnet.Client;

namespace Admin.Api;

public class MqttAdapterService : IHostedService
{
    readonly IMqttClient _client;
    readonly MqttClientOptions _options;
    readonly ILogger<MqttAdapterService> _logger;

    public MqttAdapterService(IMqttClient client, MqttClientOptions options, ILogger<MqttAdapterService> logger)
    {
        _client = client;
        _options = options;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _client.ApplicationMessageReceivedAsync += ClientOnApplicationMessageReceivedAsync;
        _client.ConnectedAsync += ClientOnConnectedAsync;
        _client.DisconnectedAsync += ClientOnDisconnectedAsync;
        await _client.ConnectAsync(_options, cancellationToken);
    }

    async Task ClientOnDisconnectedAsync(MqttClientDisconnectedEventArgs arg)
    {
        _logger.LogInformation("Got disconnected, going to re-connect!");
        _client.ConnectedAsync += ClientOnConnectedAsync;
        await _client.ConnectAsync(_options);
    }

    async Task ClientOnConnectedAsync(MqttClientConnectedEventArgs arg)
    {
        _client.ConnectedAsync -= ClientOnConnectedAsync;

        var subscribeOptions = new MqttClientSubscribeOptionsBuilder()
            .WithTopicFilter(builder => builder
                .WithExactlyOnceQoS()
                .WithTopic("client/*"))
            .Build();

        await _client.SubscribeAsync(subscribeOptions);
    }

    Task ClientOnApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs arg)
    {
        var content = arg.ApplicationMessage.Payload != null
            ? Encoding.UTF8.GetString(arg.ApplicationMessage.Payload)
            : string.Empty;

        _logger.LogInformation($"Got a message from Mqtt: {arg.ApplicationMessage.Topic} -> {content}");
        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _client.ApplicationMessageReceivedAsync -= ClientOnApplicationMessageReceivedAsync;
        _client.ConnectedAsync -= ClientOnConnectedAsync;
        _client.DisconnectedAsync -= ClientOnDisconnectedAsync;

        await _client.DisconnectAsync();
    }
}