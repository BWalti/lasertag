using Marten;
using Wolverine.Marten;

// ReSharper disable UnusedMember.Global

namespace Admin.Api.Domain.Lasertag;

#pragma warning disable S1118

public static class ServerCommandHandlers
{
    public class CreateServerHandler
    {
        public static async Task<Server> Handle(
            LasertagCommands.CreateServer _,
            IDocumentSession session,
            IMartenOutbox outbox)
        {
            var server = new Server
            {
                Id = Guid.NewGuid()
            };

            session.Store(server);

            await outbox.PublishAsync(new LasertagEvents.ServerCreated(server.Id));
            await session.SaveChangesAsync();

            return server;
        }
    }

    public class AddGameSetHandler
    {
        public static LasertagEvents.GameSetRegistered Handle(
            LasertagCommands.RegisterGameSet _,
            Server server,
            IDocumentSession session,
            ILogger<AddGameSetHandler> logger)
        {
            logger.LogInformation("Adding GameSet");

            var gameSet = new GameSet(server.GameSets.Count + 1);
            server.GameSets.Add(gameSet);

            session.Store(server);

            return new LasertagEvents.GameSetRegistered(server.Id, gameSet.Id);
        }
    }

    public class GameSetConnectedHandler
    {
        public static void Handle(
            LasertagEvents.GameSetConnected @event,
            Server server,
            IDocumentSession session,
            ILogger<GameSetConnectedHandler> logger)
        {
            logger.LogInformation("GameSet Connected");

            var gameSet = server.GameSets.FirstOrDefault(gs => gs.Id == @event.GameSetId);
            if (gameSet == null)
            {
                throw new ArgumentOutOfRangeException(nameof(@event),
                    $"GameSet with ID {@event.GameSetId} is unknown to server {@event.ServerId}");
            }

            gameSet.IsConnected = true;
            session.Store(server);
        }
    }

    public class PrepareGameHandler
    {
        public static async Task<LasertagEvents.GamePrepared> Handle(
            LasertagCommands.PrepareGame prepare,
            Server server,
            ILogger<PrepareGameHandler> logger,
            IDocumentSession session)
        {
            logger.LogInformation("Preparing game");

            var inputConfig = prepare.Configuration;

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
            var gamePrepared = new LasertagEvents.GamePrepared(gameId, lobby);

            session.Events.StartStream<Game>(gameId, gamePrepared);
            await session.SaveChangesAsync();

            return gamePrepared;
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
}

#pragma warning restore S1118