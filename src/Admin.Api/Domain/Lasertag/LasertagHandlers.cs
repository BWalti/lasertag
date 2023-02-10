using Marten;
using Wolverine.Attributes;
// ReSharper disable UnusedMember.Global

namespace Admin.Api.Domain.Lasertag;

#pragma warning disable S1118

public static class LasertagHandlers
{
    public class AddGameSetHandler
    {
        [Transactional]
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

    public class PrepareGameHandler
    {
        [Transactional]
        public static LasertagEvents.GamePrepared Handle(
            LasertagCommands.PrepareGame prepare,
            Server server,
            ILogger<AddGameSetHandler> logger)
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

            return new LasertagEvents.GamePrepared(server.Id, lobby);
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
