using System.Text;
using Admin.Api.Domain.Lasertag;
using MQTTnet;
using MQTTnet.Client;
using Newtonsoft.Json;
using Wolverine;

namespace Admin.Api;

public class MqttAdapterService : IHostedService
{
    readonly IMessageContext _bus;
    readonly IMqttClient _client;
    readonly ILogger<MqttAdapterService> _logger;
    readonly MqttClientOptions _options;

    public MqttAdapterService(IMqttClient client, MqttClientOptions options, ILogger<MqttAdapterService> logger,
        IMessageContext bus)
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
        async Task ProcessMessage<T>(string serializedContent)
        {
            var message = JsonConvert.DeserializeObject<T>(serializedContent);
            _logger.LogInformation($"Got something: {message}");

            await _bus.SendAsync(message);
        }

        var content = arg.ApplicationMessage.Payload != null
            ? Encoding.UTF8.GetString(arg.ApplicationMessage.Payload)
            : string.Empty;

        _logger.LogInformation($"Got a message from Mqtt: {arg.ApplicationMessage.Topic} -> '{content}'");

        switch (arg.ApplicationMessage.Topic)
        {
            case MqttTopics.GameSetConnected:
                await ProcessMessage<LasertagEvents.GameSetConnected>(content);
                break;

            case MqttTopics.GameSetActivated:
                await ProcessMessage<LasertagEvents.GameSetActivated>(content);
                break;

            case MqttTopics.ShotFired:
                await ProcessMessage<LasertagEvents.GameSetFiredShot>(content);
                break;
        }
    }
}