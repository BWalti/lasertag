 using Marten.Events;
 using Wolverine.Marten;

// ReSharper disable UnusedMember.Global

namespace Lasertag.Core.Domain.Lasertag;

#pragma warning disable S1118

public static class CommandHandlers
{
    public class PrepareGameHandler
    {
        public static (LasertagEvents.GamePrepared, IMartenOp) Handle(
            LasertagCommands.PrepareGame prepare,
            Server server)
        {
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

            var o = MartenOps.StartStream<Game>(gameId, gamePrepared);
            
            return (gamePrepared, o);
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

    public class EndGameHandler
    {
        [AggregateHandler]
        public static LasertagEvents.GameFinished Handle(
            LasertagCommands.EndGame @event,
            IEventStream<Game> gameStream)
        {
            var gameFinished = new LasertagEvents.GameFinished(@event.ServerId, @event.GameId);
            
            if (gameStream.Aggregate.Status == GameStatus.Started)
            {
                gameStream.AppendOne(gameFinished);
            }
            
            return gameFinished;
        }
    }
}

#pragma warning restore S1118