using System.Text;
using Lasertag.Core.Domain.Lasertag;
using MQTTnet;
using MQTTnet.Client;
using Newtonsoft.Json;
using Wolverine;

namespace Admin.Api;

public class MqttAdapterService : IHostedService
{
    readonly IMqttClient _client;
    readonly ILogger<MqttAdapterService> _logger;
    readonly IMessageBus _bus;
    readonly MqttClientOptions _options;

    public MqttAdapterService(IMqttClient client, MqttClientOptions options, ILogger<MqttAdapterService> logger,
        IMessageBus bus)
    {
        _client = client;
        _options = options;
        _logger = logger;
        _bus = bus;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _client.ApplicationMessageReceivedAsync += ClientOnApplicationMessageReceivedAsync;
        _client.ConnectedAsync += ClientOnConnectedAsync;
        _client.DisconnectedAsync += ClientOnDisconnectedAsync;

        await _client.ConnectAsync(_options, cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _client.ApplicationMessageReceivedAsync -= ClientOnApplicationMessageReceivedAsync;
        _client.ConnectedAsync -= ClientOnConnectedAsync;
        _client.DisconnectedAsync -= ClientOnDisconnectedAsync;

        await _client.DisconnectAsync();
    }

    public async Task Handle(LasertagEvents.ServerCreated @event)
    {
        var serialized = JsonConvert.SerializeObject(@event);

        _logger.LogInformation("Going to publish a message...");
        var message = new MqttApplicationMessageBuilder()
            .WithTopic(MqttTopics.ServerMessages)
            .WithPayload(serialized)
            .Build();

        await _client.PublishAsync(message);
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
                .WithTopic(MqttTopics.EverythingFromAnyClient))
            .Build();

        await _client.SubscribeAsync(subscribeOptions);
    }

    async Task ClientOnApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs arg)
    {
        T ExtractMessage<T>(string serializedContent)
        {
            var message = JsonConvert.DeserializeObject<T>(serializedContent);
            _logger.LogInformation("Got something: {Message}", message);

            return message!;
        }

        var content = arg.ApplicationMessage.PayloadSegment.Count > 0
            ? Encoding.UTF8.GetString(arg.ApplicationMessage.PayloadSegment)
            : string.Empty;

        _logger.LogInformation("Got a message from MQTT: {Topic} -> '{Content}'", arg.ApplicationMessage.Topic, content);

        switch (arg.ApplicationMessage.Topic)
        {
            case MqttTopics.ConnectGameSet:
                var gameSetConnected = ExtractMessage<LasertagCommands.ConnectGameSet>(content);
                await _bus.SendAsync(gameSetConnected);
                break;

            case MqttTopics.ActivateGameSet:
                var gameSetActivated = ExtractMessage<LasertagCommands.ActivateGameSet>(content);
                await _bus.SendAsync(gameSetActivated);
                break;

            case MqttTopics.FireShot:
                var shotFired = ExtractMessage<LasertagCommands.FireShot>(content);
                await _bus.SendAsync(shotFired);
                break;

            case MqttTopics.GameSetGotHit:
                var registerHit = ExtractMessage<LasertagCommands.RegisterHit>(content);
                await _bus.SendAsync(registerHit);
                break;
        }

        await arg.AcknowledgeAsync(CancellationToken.None);
    }
}