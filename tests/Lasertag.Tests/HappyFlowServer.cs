using Admin.Api.Domain.Lasertag;
using Alba;
using FluentAssertions;
using Lasertag.IoT.Simulator;
using Lasertag.Tests.TestInfrastructure;
using Xunit;
using Xunit.Abstractions;
using static Admin.Api.Domain.Lasertag.LasertagEvents;

namespace Lasertag.Tests;

[Collection("integration")]
public class HappyFlowServer : IntegrationContext
{
    readonly ITestOutputHelper _outputHelper;
    readonly IotSimulator _simulator;

    public HappyFlowServer(AppFixture fixture, ITestOutputHelper outputHelper) : base(fixture)
    {
        _outputHelper = outputHelper;
        _simulator = new IotSimulator(MqttClient);
    }

    [Fact]
    public async Task HappyFlow()
    {
        var numberOfGameSets = 2;
        var gameDuration = TimeSpan.FromSeconds(3);

        // Arrange:
        var server = await PrepareServer();

        // Register GameSets:
        var gameSets = await RegisterGameSets(server, numberOfGameSets);
        await EnsureGameSetsRegistered(server, numberOfGameSets);
        await _simulator.ConnectGameSets(gameSets);

        var gamePrepared = await PrepareGame(server, new LobbyConfiguration
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

        await _simulator.ActivateGameSet(gameSets[0], gamePrepared.GameId, 1);
        await _simulator.ActivateGameSet(gameSets[1], gamePrepared.GameId, 2);

        await StartGame(game!, gameDuration);
        await ReloadGame(gamePrepared, g => g.IsGameRunning.Should().BeTrue());

        await _simulator.Shoot(game!.Id, gameSets[0]);

        // await end of game:
        await Task.Delay(gameDuration);
        game = await ReloadGame(gamePrepared, g => g.IsGameRunning.Should().BeFalse());

        await DeleteGame(game!);

        // now as the game got deleted, we shouldn't be able to load it anymore:
        game = await ReloadGame(gamePrepared, mustExist: false);
        game.Should().BeNull();
    }

    async Task EnsureGameSetsRegistered(Server server, int numberOfGameSets)
    {
        await using var session = Store.LightweightSession();

        var persisted = await session.LoadAsync<Server>(server.Id);
        persisted.Should().NotBeNull();
        persisted!.GameSets.Should().HaveCount(numberOfGameSets);
    }

    async Task<Server> PrepareServer()
    {
        var id = Guid.NewGuid();
        _outputHelper.WriteLine($"Created Server with ID: {id}");

        var server = new Server
        {
            Name = "Test Server",
            Id = id
        };

        await using var session = Store.LightweightSession();

        session.Store(server);
        await session.SaveChangesAsync();

        return server;
    }

    async Task<GameSetRegistered[]> RegisterGameSets(Server server, int numberOfGameSets = 2)
    {
        if (numberOfGameSets < 2)
        {
            throw new ArgumentOutOfRangeException(nameof(numberOfGameSets), "There needs to be at least 2 GameSets!");
        }

        var registeredGameSets = new GameSetRegistered[numberOfGameSets];

        for (var i = 0; i < registeredGameSets.Length; i++)
        {
            var gameSetRegistered = await RegisterGameSet(server);
            gameSetRegistered.GameSetId.Should().Be(i + 1);

            registeredGameSets[i] = gameSetRegistered;
        }

        return registeredGameSets;
    }

    async Task<Game?> ReloadGame(GamePrepared gamePrepared, Action<Game>? validateGame = null, bool mustExist = true)
    {
        await using var session = Store.LightweightSession();

        var game = await session.Events.AggregateStreamAsync<Game>(gamePrepared.GameId);
        if (!mustExist)
        {
            return game;
        }

        game.Should().NotBeNull();
        validateGame?.Invoke(game!);

        return game!;
    }

    async Task<GameSetRegistered> RegisterGameSet(Server server)
    {
        var (_, scenarioResult) = await TrackedHttpCall(x =>
        {
            x.Post.Url(ApiRouteBuilder.RegisterGameSet(server));
            x.StatusCodeShouldBeOk();
        });

        scenarioResult.Should().NotBeNull();
        var gameSetRegistered = await scenarioResult!.ReadAsJsonAsync<GameSetRegistered>();
        gameSetRegistered.Should().NotBeNull();
        gameSetRegistered!.ServerId.Should().Be(server.Id);

        return gameSetRegistered;
    }

    async Task<GamePrepared> PrepareGame(Server server, LobbyConfiguration lobbyConfiguration)
    {
        var (tracked, result) = await TrackedHttpCall(x =>
        {
            x.Post.Json(lobbyConfiguration).ToUrl(ApiRouteBuilder.PrepareGame(server));
            x.StatusCodeShouldBeOk();
        });

        result.Should().NotBeNull();
        result!.Context.Response.StatusCode.Should().Be(200);
        return tracked.Sent.SingleMessage<GamePrepared>();
    }

    async Task StartGame(Game game, TimeSpan gameDuration)
    {
        var (tracked, result) = await TrackedHttpCall(x =>
        {
            x.Post.Url(ApiRouteBuilder.StartGame(game, gameDuration));
            x.StatusCodeShouldBeOk();
        });

        result.Should().NotBeNull();
        result!.Context.Response.StatusCode.Should().Be(200);

        var gameStarted = tracked.Sent.SingleMessage<GameStarted>();
        gameStarted.Should().NotBeNull();

        var envelope = tracked.Sent.SingleEnvelope<LasertagCommands.EndGame>();
        envelope.Message.Should().BeAssignableTo<LasertagCommands.EndGame>();
        envelope.ScheduleDelayed(gameDuration);
    }

    async Task DeleteGame(Game game)
    {
        await TrackedHttpCall(x =>
        {
            x.Delete.Url(ApiRouteBuilder.DeleteGame(game));
            x.StatusCodeShouldBeOk();
        });
    }
}