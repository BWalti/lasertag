using Admin.Api;
using MQTTnet;
using MQTTnet.Client;
using Newtonsoft.Json;
using static Lasertag.Core.Domain.Lasertag.LasertagCommands;
using static Admin.Api.Models.Response;

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
            var gameSetActivated = new ConnectGameSet(gameSet.ServerId, gameSet.Id);
            var serialized = JsonConvert.SerializeObject(gameSetActivated);

            var message = new MqttApplicationMessageBuilder()
                .WithTopic(MqttTopics.ConnectGameSet)
                .WithPayload(serialized)
                .Build();

            await MqttClient.PublishAsync(message);
        }
    }

    public async Task ActivateGameSet(RegisterGameSetResponse gameSet, Guid gameId, int playerId)
    {
        var gameSetActivated = new ActivateGameSet(gameId, gameSet.Id, playerId);
        var serialized = JsonConvert.SerializeObject(gameSetActivated);

        var message = new MqttApplicationMessageBuilder()
            .WithTopic(MqttTopics.ActivateGameSet)
            .WithPayload(serialized)
            .Build();

        await MqttClient.PublishAsync(message);
    }

    public async Task Shoot(Guid gameId, RegisterGameSetResponse gameSet)
    {
        var shot = new FireShot(gameId, gameSet.Id);
        var serialized = JsonConvert.SerializeObject(shot);

        var message = new MqttApplicationMessageBuilder()
            .WithTopic(MqttTopics.FireShot)
            .WithPayload(serialized)
            .Build();

        await MqttClient.PublishAsync(message);
    }

    public async Task GotHit(Guid gameId, RegisterGameSetResponse gameSetHitReceiver,
        RegisterGameSetResponse gameSetHitSender, int shotCounter)
    {
        var shot = new RegisterHit(gameId, gameSetHitReceiver.Id, gameSetHitSender.Id, shotCounter);
        var serialized = JsonConvert.SerializeObject(shot);

        var message = new MqttApplicationMessageBuilder()
            .WithTopic(MqttTopics.GameSetGotHit)
            .WithPayload(serialized)
            .Build();

        await MqttClient.PublishAsync(message);
    }
}