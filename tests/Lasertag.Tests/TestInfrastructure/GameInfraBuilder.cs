using Admin.Api;
using Alba;
using FluentAssertions;
using Lasertag.Core.Domain.Lasertag;
using Lasertag.IoT.Simulator;
using Marten;
using static Admin.Api.Models.Response;

namespace Lasertag.Tests.TestInfrastructure;

public class GameInfraBuilder
{
    readonly IotSimulator _simulator;

    GameInfraBuilder(IntegrationContext integrationContext)
    {
        IntegrationContext = integrationContext;
        _simulator = new IotSimulator(IntegrationContext.MqttClient);
    }

    internal IAlbaHost AlbaHost => IntegrationContext.Host;
    internal IntegrationContext IntegrationContext { get; }

    public TimeSpan GameDuration { get; private set; }
    public int RegisteredGameSetCount { get; private set; } = 2;
    public int ConnectedGameSetCount { get; private set; } = 2;

    public int NumberOfTeams { get; private set; }

    public bool ShouldPrepareGame { get; private set; }

    IDocumentStore Store => IntegrationContext.Store;

    public static GameInfraBuilder Create(IntegrationContext integrationContext) =>
        new(integrationContext);

    public GameInfraBuilder WithGameDuration(TimeSpan gameDuration)
    {
        GameDuration = gameDuration;
        return this;
    }

    public GameInfraBuilder WithRegisteredGameSetCount(int registeredGameSetCount)
    {
        RegisteredGameSetCount = registeredGameSetCount;
        return this;
    }

    public GameInfraBuilder WithConnectedGameSetCount(int connectedGameSetCount)
    {
        ConnectedGameSetCount = connectedGameSetCount;
        return this;
    }

    public GameInfraBuilder WithPreparedGame(int numberOfTeams = 2)
    {
        ShouldPrepareGame = true;
        NumberOfTeams = numberOfTeams;

        return this;
    }

    public async Task<GameInfra> Build()
    {
        var (_, scenarioResult) = await IntegrationContext.TrackedHttpCall(x =>
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
        var serverId = serverCreated.Id;

        var gameSets = await RegisterGameSets(serverId, RegisteredGameSetCount);
        await EnsureGameSetsRegistered(RegisteredGameSetCount, serverId);
        await _simulator.ConnectGameSets(gameSets.Take(ConnectedGameSetCount).ToArray());

        var gameId = Guid.Empty;

        if (ShouldPrepareGame)
        {
            var gamePrepared = await PrepareGame(serverId, new LobbyConfiguration
            {
                NumberOfTeams = NumberOfTeams
            });

            gameId = gamePrepared.Id;
        }

        return new GameInfra(
            serverId,
            gameId,
            gameSets,
            GameDuration,
            RegisteredGameSetCount,
            ConnectedGameSetCount,
            NumberOfTeams,
            IntegrationContext,
            _simulator);
    }

    async Task<GameCreatedResponse> PrepareGame(Guid serverId, LobbyConfiguration lobbyConfiguration)
    {
        var (_, scenarioResult) = await IntegrationContext.TrackedHttpCall(x =>
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

    async Task<RegisterGameSetResponse> RegisterGameSet(Guid serverId)
    {
        var response = AlbaHost.PostJson(new LasertagCommands.RegisterGameSet(serverId),
            ApiRouteBuilder.RegisterGameSetPath);

        response.Should().NotBeNull();
        var registeredGameSet = await response.Receive<RegisterGameSetResponse>();
        registeredGameSet.Should().NotBeNull();

        return registeredGameSet!;
    }

    async Task EnsureGameSetsRegistered(int numberOfGameSets, Guid serverId)
    {
        await using var session = Store.LightweightSession();

        var persisted = await session.LoadAsync<Server>(serverId);
        persisted.Should().NotBeNull();
        persisted!.GameSets.Should().HaveCount(numberOfGameSets);
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
}

public class GameInfra
{
    readonly IntegrationContext _integrationContext;
    readonly IotSimulator _iotSimulator;

    public GameInfra(Guid serverId, Guid gameId, RegisterGameSetResponse[] gameSets, TimeSpan gameDuration,
        int registeredGameSetCount, int connectedGameSetCount, int numberOfTeams, IntegrationContext integrationContext,
        IotSimulator iotSimulator)
    {
        _integrationContext = integrationContext;
        _iotSimulator = iotSimulator;

        ServerId = serverId;
        GameId = gameId;
        GameSets = gameSets;
        GameDuration = gameDuration;
        RegisteredGameSetCount = registeredGameSetCount;
        ConnectedGameSetCount = connectedGameSetCount;
        NumberOfTeams = numberOfTeams;
    }

    public Guid ServerId { get; }
    public Guid GameId { get; }
    public RegisterGameSetResponse[] GameSets { get; }
    public TimeSpan GameDuration { get; }
    public int RegisteredGameSetCount { get; }
    public int ConnectedGameSetCount { get; }
    public int NumberOfTeams { get; }

    public async Task StartGame()
    {
        var (tracked, scenarioResult) = await _integrationContext.TrackedHttpCall(x =>
        {
            x.Post
                .Json(new LasertagCommands.StartGame(ServerId, GameId, GameDuration))
                .ToUrl(ApiRouteBuilder.StartGamePath);
            x.StatusCodeShouldBeOk();
        });

        scenarioResult.Should().NotBeNull();
        var response = scenarioResult!.ReadAsJson<GameStartedResponse>();
        response.Should().NotBeNull();

        var envelope = tracked.Sent.SingleEnvelope<LasertagCommands.EndGame>();
        envelope.Message.Should().BeAssignableTo<LasertagCommands.EndGame>();
        envelope.ScheduleDelayed(GameDuration);
    }

    public async Task<Game?> ReloadGame(Action<Game>? validateGame = null, bool mustExist = true)
    {
        await using var session = _integrationContext.Store.LightweightSession();

        var game = await session.Events.AggregateStreamAsync<Game>(GameId);
        if (!mustExist)
        {
            return game;
        }

        game.Should().NotBeNull();
        validateGame?.Invoke(game!);

        return game!;
    }

    public Task ActivateGameSet(int index, int playerId)
    {
        if (index >= RegisteredGameSetCount)
        {
            throw new ArgumentOutOfRangeException("index", "There are not enough registered GameSets!");
        }

        return _iotSimulator.ActivateGameSet(GameSets[index], GameId, playerId);
    }

    public Task Shoot(int gameSetIndex)
    {
        if (gameSetIndex >= ConnectedGameSetCount)
        {
            throw new ArgumentOutOfRangeException(
                "gameSetIndex",
                "There are not enough connected GameSets!");
        }

        return _iotSimulator.Shoot(GameId, GameSets[gameSetIndex]);
    }

    public Task GotHit(int hitReceiverIndex, int originalSenderIndex, int shotCounter)
    {
        if (hitReceiverIndex >= ConnectedGameSetCount)
        {
            throw new ArgumentOutOfRangeException(
                "hitReceiverIndex",
                "There are not enough connected GameSets!");
        }

        if (originalSenderIndex >= ConnectedGameSetCount)
        {
            throw new ArgumentOutOfRangeException(
                "originalSenderIndex",
                "There are not enough connected GameSets!");
        }

        return _iotSimulator.GotHit(GameId, GameSets[hitReceiverIndex], GameSets[originalSenderIndex], shotCounter);
    }

    public async Task DeleteGame()
    {
        await _integrationContext.TrackedHttpCall(x =>
        {
            x.Delete
                .Json(new LasertagCommands.DeleteGame(GameId))
                .ToUrl(ApiRouteBuilder.DeleteGamePath);
            x.StatusCodeShouldBeOk();
        });
    }
}