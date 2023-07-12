using System.Text;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Client;

namespace Lasertag.IoT.Simulator;

public class MqttAdapter : IMqttAdapter, IAsyncDisposable
{
    readonly IMqttClient _client;
    readonly ILogger<MqttAdapter> _logger;
    readonly MqttClientOptions _options;
    IHandleMessages? _messageHandler;
    string[]? _topics;

    public MqttAdapter(MqttClientOptions options, IMqttClient client, ILogger<MqttAdapter> logger)
    {
        _options = options;
        _client = client;
        _logger = logger;

        _client.DisconnectedAsync += ClientOnDisconnectedAsync;
        _client.ApplicationMessageReceivedAsync += ClientOnApplicationMessageReceivedAsync;
        _client.ConnectedAsync += ClientOnConnectedAsync;
    }

    public async ValueTask DisposeAsync()
    {
        await _client.DisconnectAsync();
        _logger.LogInformation("Mqtt Connection closed...");

        await _client.DisconnectAsync();
        _logger.LogInformation("Mqtt connection disposed...");
    }

    public async Task SendMessageAsync(string topic, string payload)
    {
        await EnsureConnection();

        _logger.LogInformation("Going to publish a message...");
        var message = new MqttApplicationMessageBuilder()
            .WithTopic(topic)
            .WithPayload(payload)
            .Build();

        await _client.PublishAsync(message);
        _logger.LogInformation("Successfully published the message");
    }

    public async Task RegisterListener(IHandleMessages messageHandler, string[] topics)
    {
        _messageHandler = messageHandler;
        _topics = topics;

        await EnsureConnection();
        await EnsureTopicSubscription();
    }

    async Task ClientOnConnectedAsync(MqttClientConnectedEventArgs arg)
    {
        _logger.LogInformation("Connected to Mqtt!");
        await EnsureTopicSubscription();
    }

    async Task EnsureTopicSubscription()
    {
        if (_topics == null)
        {
            return;
        }

        foreach (var topic in _topics)
        {
            var subscribeOptions = new MqttClientSubscribeOptionsBuilder()
                .WithTopicFilter(builder => builder
                    .WithExactlyOnceQoS()
                    .WithTopic(topic))
                .Build();

            await _client.SubscribeAsync(subscribeOptions);
        }
    }

    async Task ClientOnApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs arg)
    {
        _messageHandler?.ProcessMessage(arg.ApplicationMessage.Topic,
            Encoding.UTF8.GetString(arg.ApplicationMessage.PayloadSegment));
        await arg.AcknowledgeAsync(CancellationToken.None);
    }

    async Task EnsureConnection()
    {
        if (_client.IsConnected)
        {
            return;
        }

        _logger.LogInformation("Going to connect to MQTT...");
        await _client.ConnectAsync(_options);
    }

    async Task ClientOnDisconnectedAsync(MqttClientDisconnectedEventArgs arg)
    {
        _logger.LogWarning("Got disconnected, going to re-connect!");
        await EnsureConnection();
    }
}