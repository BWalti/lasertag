using Marten.Events;
using Wolverine.Marten;

// ReSharper disable UnusedMember.Global

namespace Lasertag.Core.Domain.Lasertag;

#pragma warning disable S1118

public static class CommandHandlers
{
    public static class GameSetCommandHandler
    {
        [AggregateHandler]
        public static LasertagEvents.GameSetConnected Handle(LasertagCommands.ConnectGameSet command, Server server)
        {
            if (server.GameSets.TrueForAll(gs => gs.Id != command.GameSetId))
            {
                throw new InvalidOperationException($"The GameSet with ID {command.GameSetId} is unknown to this server!");
            }

            var gameSetConnected = new LasertagEvents.GameSetConnected(command.ServerId, command.GameSetId);
            return gameSetConnected;
        }

        [AggregateHandler]
        public static LasertagEvents.GameSetActivated Handle(LasertagCommands.ActivateGameSet command, Game game)
        {
            if (!game.Lobby.Teams.Any(t => t.Value.GameSets.Any(gs => gs.Id == command.GameSetId)))
            {
                throw new InvalidOperationException($"GameSet with ID {command.GameSetId} is unknown in this Game!");
            }

            var gameSetActivated = new LasertagEvents.GameSetActivated(command.GameId, command.GameSetId, command.PlayerId);
            return gameSetActivated;
        }

        [AggregateHandler]
        public static LasertagEvents.GameSetFiredShot Handle(LasertagCommands.FireShot command, Game game)
        {
            if (!game.Lobby.Teams.Any(t => t.Value.GameSets.Any(gs => gs.Id == command.GameSetId)))
            {
                throw new InvalidOperationException($"GameSet with ID {command.GameSetId} is unknown in this Game!");
            }

            var gameSetFiredShot = new LasertagEvents.GameSetFiredShot(command.GameId, command.GameSetId);
            return gameSetFiredShot;
        }

        [AggregateHandler]
        public static LasertagEvents.GameSetGotHit Handle(LasertagCommands.RegisterHit command, Game game)
        {
            if (!game.Lobby.Teams.Any(t => t.Value.GameSets.Any(gs => gs.Id == command.GameSetId)))
            {
                throw new InvalidOperationException($"GameSet with ID {command.GameSetId} is unknown in this Game!");
            }

            return new LasertagEvents.GameSetGotHit(command.GameId, command.ShotSourceGameSetId, command.GameSetId);
        }
    }

    public static class PrepareGameHandler
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
                    .ToDictionary(g => g.TeamId, g => g)
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

    public static class EndGameHandler
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