using JetBrains.Annotations;
using Lasertag.Core.Domain.Lasertag;
using Marten;
using Marten.Events;
using Microsoft.AspNetCore.Mvc;
using Wolverine;
using Wolverine.Attributes;
using Wolverine.Http;
using Wolverine.Marten;
using static Lasertag.Core.Domain.Lasertag.LasertagEvents;
using static Admin.Api.Models.Response;

namespace Admin.Api.Endpoints;

[UsedImplicitly]
public class LasertagEndpoints
{
    [WolverineGet(ApiRouteBuilder.GetServerByIdPathTemplate)]
    public async Task<Server?> Get([FromRoute] Guid id, IDocumentSession session) =>
        await session.LoadAsync<Server>(id);

    [Transactional]
    [WolverinePost(ApiRouteBuilder.CreateServerPath)]
    public (ServerCreatedResponse, IStartStream) CreateServer(LasertagCommands.CreateServer command)
    {
        var id = Guid.NewGuid();
        var serverCreated = new ServerCreated(command.Name);
        var stream = MartenOps.StartStream<Server>(id, serverCreated);

        return (
            new ServerCreatedResponse(stream.StreamId, command.Name),
            stream
        );
    }

    [AggregateHandler(AggregateType = typeof(Server))]
    [WolverinePost(ApiRouteBuilder.RegisterGameSetPath)]
    public (RegisterGameSetResponse, GameSetRegistered) RegisterGameSet(LasertagCommands.RegisterGameSet command,
        Server server)
    {
        if (server.Status is not ServerStatus.Created and ServerStatus.ReadyForLobby)
        {
            throw new InvalidOperationException($"Server is not in the right state: {server.Status}");
        }

        var id = server.GameSets.Count + 1;
        var @event = new GameSetRegistered(server.Id, id);

        return (new RegisterGameSetResponse(server.Id, id), @event);
    }

    [AggregateHandler(AggregateType = typeof(Server))]
    [WolverinePost(ApiRouteBuilder.PrepareGamePath)]
    public (GameCreatedResponse, IStartStream) PrepareGame(
        LasertagCommands.PrepareGame command,
        IEventStream<Server> serverStream)
    {
        var server = serverStream.Aggregate;
        if (server.Status != ServerStatus.ReadyForLobby)
        {
            throw new InvalidOperationException($"Server is not in the right state: {server.Status}");
        }

        var inputConfig = command.Configuration;

        var lobby = new Lobby
        {
            Configuration = inputConfig,
            Teams = server.GameSets
                .Select((gameSet, index) => (gameSet, index))
                .GroupBy(tuple => tuple.index % inputConfig.NumberOfTeams)
                .Select(ConvertGroupingToTeam)
                .ToArray()
        };

        var gameId = Guid.NewGuid();

        var gamePrepared = new GamePrepared(server.Id, gameId, lobby);
        serverStream.AppendOne(gamePrepared);

        var gameStream = MartenOps.StartStream<Game>(gameId, gamePrepared);

        return (new GameCreatedResponse(gameStream.StreamId), gameStream);
    }

    [WolverineGet(ApiRouteBuilder.GetGameByIdPathTemplate)]
    public async Task<Game?> GetGame([FromRoute] Guid id, IDocumentSession session) =>
        await session.LoadAsync<Game>(id);

    [AggregateHandler(AggregateType = typeof(Game))]
    [WolverinePost(ApiRouteBuilder.StartGamePath)]
    public async Task<GameStartedResponse> StartGame(LasertagCommands.StartGame command, IEventStream<Game> gameStream,
        IDocumentSession session, IMessageBus bus, ILogger logger)
    {
        var serverStream = await session.Events.FetchForWriting<Server>(command.ServerId);
        var server = serverStream.Aggregate;

        if (server is not { Status: ServerStatus.GamePrepared })
        {
            throw new InvalidOperationException($"Server is not in the right state: {server.Status}");
        }

        logger.LogInformation(
            "Starting Game with Id: {GameId} for duration: {Duration}",
            command.GameId,
            command.GameDuration);

        var gameStarted = new GameStarted(command.ServerId, command.GameId);
        serverStream.AppendOne(gameStarted);
        gameStream.AppendOne(gameStarted);

        var endGame = new LasertagCommands.EndGame(command.ServerId, command.GameId);
        await bus.SendAsync(endGame, new DeliveryOptions
        {
            ScheduleDelay = command.GameDuration
        });

        return new GameStartedResponse(command.GameId, command.ServerId);
    }

    [AggregateHandler(AggregateType = typeof(Game))]
    [WolverinePost(ApiRouteBuilder.StopGamePath)]
    public IResult EndGame(LasertagCommands.EndGame command, IEventStream<Game> gameStream,
        IEventStream<Server> serverStream, IMessageBus bus, ILogger logger)
    {
        var serverStatus = serverStream.Aggregate.Status;
        var gameStatus = gameStream.Aggregate.Status;

        if (serverStatus == ServerStatus.GameRunning
            && gameStatus == GameStatus.Started)
        {
            logger.LogInformation(
                "Ending Game with Id: {GameId}",
                command.GameId);

            var gameFinished = new GameFinished(command.ServerId, command.GameId);
            serverStream.AppendOne(gameFinished);
            gameStream.AppendOne(gameFinished);
            return TypedResults.Accepted(ApiRouteBuilder.GetGameById(command.GameId.ToString()));
        }

        throw new InvalidOperationException(
            $"The server or game is not in the right state: {serverStatus} / {gameStatus}");
    }

    [AggregateHandler(AggregateType = typeof(Game))]
    [WolverineDelete(ApiRouteBuilder.DeleteGamePath)]
    public IResult DeleteGame(LasertagCommands.DeleteGame command, IDocumentSession session, ILogger logger)
    {
        logger.LogInformation(
            "Deleting Game with Id: {GameId}",
            command.GameId);

        session.Events.ArchiveStream(command.GameId);
        return TypedResults.Ok();
    }

    static Team ConvertGroupingToTeam(IGrouping<int, (GameSet gameSet, int i)> groupings)
    {
        var team = new Team(groupings.Key);

        foreach (var (gameSet, _) in groupings)
        {
            team.Add(gameSet);
        }

        return team;
    }
}