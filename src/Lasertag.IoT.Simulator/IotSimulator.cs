using Admin.Api;
using MQTTnet;
using MQTTnet.Client;
using Newtonsoft.Json;
using static Lasertag.Core.Domain.Lasertag.LasertagEvents;

namespace Lasertag.IoT.Simulator;

public class IotSimulator
{
    public IotSimulator(IMqttClient client)
    {
        MqttClient = client;
    }

    public IMqttClient MqttClient { get; }

    public async Task ConnectGameSets(RegisterGameSetResponse[] gameSets)
    {
        foreach (var gameSet in gameSets)
        {
            var gameSetActivated = new GameSetConnected(gameSet.ServerId, gameSet.Id);
            var serialized = JsonConvert.SerializeObject(gameSetActivated);

            var message = new MqttApplicationMessageBuilder()
                .WithTopic(MqttTopics.GameSetConnected)
                .WithPayload(serialized)
                .Build();

            await MqttClient.PublishAsync(message);
        }
    }

    public async Task ActivateGameSet(RegisterGameSetResponse gameSet, Guid gameId, int playerId)
    {
        var gameSetActivated = new GameSetActivated(gameId, gameSet.Id, playerId);
        var serialized = JsonConvert.SerializeObject(gameSetActivated);

        var message = new MqttApplicationMessageBuilder()
            .WithTopic(MqttTopics.GameSetActivated)
            .WithPayload(serialized)
            .Build();

        await MqttClient.PublishAsync(message);
    }

    public async Task Shoot(Guid gameId, RegisterGameSetResponse gameSet)
    {
        var shot = new GameSetFiredShot(gameId, gameSet.Id);
        var serialized = JsonConvert.SerializeObject(shot);

        var message = new MqttApplicationMessageBuilder()
            .WithTopic(MqttTopics.ShotFired)
            .WithPayload(serialized)
            .Build();

        await MqttClient.PublishAsync(message);
    }
}