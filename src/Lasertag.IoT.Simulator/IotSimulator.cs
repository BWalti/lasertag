using Admin.Api;
using MQTTnet;
using MQTTnet.Client;
using Newtonsoft.Json;
using static Admin.Api.Domain.Lasertag.LasertagEvents;

namespace Lasertag.IoT.Simulator;

public class IotSimulator
{
    public IotSimulator(IMqttClient client)
    {
        MqttClient = client;
    }

    public IMqttClient MqttClient { get; }

    public async Task ConnectGameSets(GameSetRegistered[] gameSets)
    {
        foreach (var gameSet in gameSets)
        {
            var gameSetActivated = new GameSetConnected(gameSet.ServerId, gameSet.GameSetId);
            var serialized = JsonConvert.SerializeObject(gameSetActivated);

            var message = new MqttApplicationMessageBuilder()
                .WithTopic(MqttTopics.GameSetConnected)
                .WithPayload(serialized)
                .Build();

            await MqttClient.PublishAsync(message);
        }
    }

    public async Task ActivateGameSet(GameSetRegistered gameSet, Guid gameId, int playerId)
    {
        var gameSetActivated = new GameSetActivated(gameId, gameSet.GameSetId, playerId);
        var serialized = JsonConvert.SerializeObject(gameSetActivated);

        var message = new MqttApplicationMessageBuilder()
            .WithTopic(MqttTopics.GameSetActivated)
            .WithPayload(serialized)
            .Build();

        await MqttClient.PublishAsync(message);
    }

    public async Task Shoot(Guid gameId, GameSetRegistered gameSet)
    {
        var shot = new GameSetFiredShot(gameId, gameSet.GameSetId);
        var serialized = JsonConvert.SerializeObject(shot);

        var message = new MqttApplicationMessageBuilder()
            .WithTopic(MqttTopics.ShotFired)
            .WithPayload(serialized)
            .Build();

        await MqttClient.PublishAsync(message);
    }
}