using Admin.Api;
using Alba;
using FluentAssertions;
using Lasertag.Core.Domain.Lasertag;
using Lasertag.IoT.Simulator;
using Lasertag.Tests.TestInfrastructure;
using Xunit;
using static Lasertag.Core.Domain.Lasertag.LasertagEvents;
using static Admin.Api.Models.Response;

namespace Lasertag.Tests;

[Collection("integration")]
public class HappyFlowServer : IntegrationContext
{
    readonly IotSimulator _simulator;

    public HappyFlowServer(AppFixture fixture) : base(fixture)
    {
        _simulator = new IotSimulator(MqttClient);
    }

    [Fact]
    public async Task HappyFlow()
    {
        var numberOfGameSets = 2;
        var gameDuration = TimeSpan.FromSeconds(3);

        // Create server:
        var serverId = await PrepareServer();

        // Register GameSets:
        var gameSets = await RegisterGameSets(serverId, numberOfGameSets);
        await EnsureGameSetsRegistered(numberOfGameSets, serverId);
        await _simulator.ConnectGameSets(gameSets);

        var gamePrepared = await PrepareGame(serverId, new LobbyConfiguration
        {
            NumberOfTeams = 2
        });

        var game = await ReloadGame(gamePrepared, g =>
        {
            g.Lobby.Teams.Should().HaveCount(2);
            g.Lobby.Teams[0].Should().NotBeNull();
            g.Lobby.Teams[1].Should().NotBeNull();
            g.Lobby.Teams[0].GameSets.Should().HaveCount(1);
            g.Lobby.Teams[1].GameSets.Should().HaveCount(1);
        });

        await _simulator.ActivateGameSet(gameSets[0], gamePrepared.Id, 1);
        await _simulator.ActivateGameSet(gameSets[1], gamePrepared.Id, 2);
        await Task.Delay(TimeSpan.FromSeconds(1));

        await StartGame(serverId, game!, gameDuration);
        await ReloadGame(gamePrepared, g => g.Status.Should().Be(GameStatus.Started));

        await _simulator.Shoot(game!.Id, gameSets[0]);
        await _simulator.Shoot(game.Id, gameSets[0]);
        await _simulator.Shoot(game.Id, gameSets[1]);
        await Task.Delay(TimeSpan.FromSeconds(1));
        _ = await ReloadGame(gamePrepared, g => g.Statistics.ShotsFired.Should().Be(3));

        // await end of game:
        await Task.Delay(1.2 * gameDuration - TimeSpan.FromSeconds(1));
        game = await ReloadGame(gamePrepared, g => g.Status.Should().Be(GameStatus.Finished));

        await DeleteGame(game!.Id);

        // now as the game got deleted, we shouldn't be able to load it anymore:
        game = await ReloadGame(gamePrepared, mustExist: false);
        game.Should().BeNull();
    }

    async Task EnsureGameSetsRegistered(int numberOfGameSets, Guid serverId)
    {
        await using var session = Store.LightweightSession();

        var persisted = await session.LoadAsync<Server>(serverId);
        persisted.Should().NotBeNull();
        persisted!.GameSets.Should().HaveCount(numberOfGameSets);
    }

    async Task<Guid> PrepareServer()
    {
        var (_, scenarioResult) = await TrackedHttpCall(x =>
        {
            x.Post
                .Json(new LasertagCommands.CreateServer("TestServer"))
                .ToUrl(ApiRouteBuilder.CreateServerPath);

            x.StatusCodeShouldBe(201);
        });

        scenarioResult.Should().NotBeNull();
        var serverCreated = await scenarioResult!.ReadAsJsonAsync<ServerCreatedResponse>();
        serverCreated.Should().NotBeNull();
        serverCreated!.Id.Should().NotBe(Guid.Empty);

        return serverCreated.Id;
    }

    async Task<RegisterGameSetResponse[]> RegisterGameSets(Guid serverId, int numberOfGameSets = 2)
    {
        if (numberOfGameSets < 2)
        {
            throw new ArgumentOutOfRangeException(nameof(numberOfGameSets), "There needs to be at least 2 GameSets!");
        }

        var registeredGameSets = new RegisterGameSetResponse[numberOfGameSets];

        for (var i = 0; i < registeredGameSets.Length; i++)
        {
            var gameSetRegistered = await RegisterGameSet(serverId);
            gameSetRegistered.Id.Should().Be(i + 1);

            registeredGameSets[i] = gameSetRegistered;
        }

        return registeredGameSets;
    }

    async Task<Game?> ReloadGame(GameCreatedResponse gamePrepared, Action<Game>? validateGame = null, bool mustExist = true)
    {
        await using var session = Store.LightweightSession();

        var game = await session.Events.AggregateStreamAsync<Game>(gamePrepared.Id);
        if (!mustExist)
        {
            return game;
        }

        game.Should().NotBeNull();
        validateGame?.Invoke(game!);

        return game!;
    }

    async Task<RegisterGameSetResponse> RegisterGameSet(Guid serverId)
    {
        var response = Host.PostJson(new LasertagCommands.RegisterGameSet(serverId), ApiRouteBuilder.RegisterGameSetPath);

        response.Should().NotBeNull();
        var registeredGameSet = await response.Receive<RegisterGameSetResponse>();
        registeredGameSet.Should().NotBeNull();

        return registeredGameSet!;
    }

    async Task<GameCreatedResponse> PrepareGame(Guid serverId, LobbyConfiguration lobbyConfiguration)
    {
        var (_, scenarioResult) = await TrackedHttpCall(x =>
        {
            x.Post
                .Json(new LasertagCommands.PrepareGame(serverId, lobbyConfiguration))
                .ToUrl(ApiRouteBuilder.PrepareGamePath);
            x.StatusCodeShouldBe(201);
        });

        var gameCreated = await scenarioResult!.ReadAsJsonAsync<GameCreatedResponse>();
        gameCreated.Should().NotBeNull();

        return gameCreated!;
    }

    async Task StartGame(Guid serverId, Game game, TimeSpan gameDuration)
    {
        var (tracked, scenarioResult) = await TrackedHttpCall(x =>
        {
            x.Post
                .Json(new LasertagCommands.StartGame(serverId, game.Id, gameDuration))
                .ToUrl(ApiRouteBuilder.StartGamePath);
            x.StatusCodeShouldBeOk();
        });

        scenarioResult.Should().NotBeNull();
        var response = scenarioResult!.ReadAsJson<GameStartedResponse>();
        response.Should().NotBeNull();

        var envelope = tracked.Sent.SingleEnvelope<LasertagCommands.EndGame>();
        envelope.Message.Should().BeAssignableTo<LasertagCommands.EndGame>();
        envelope.ScheduleDelayed(gameDuration);
    }

    async Task DeleteGame(Guid gameId)
    {
        await TrackedHttpCall(x =>
        {
            x.Delete
                .Json(new LasertagCommands.DeleteGame(gameId))
                .ToUrl(ApiRouteBuilder.DeleteGamePath);
            x.StatusCodeShouldBeOk();
        });
    }
}