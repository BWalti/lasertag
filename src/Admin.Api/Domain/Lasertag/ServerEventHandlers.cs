using Marten;

// ReSharper disable UnusedMember.Global

namespace Admin.Api.Domain.Lasertag;

#pragma warning disable S1118

public static class ServerEventHandlers
{
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
            var gamePrepared = new LasertagEvents.GamePrepared(prepare.ServerId, gameId, lobby);

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