using Marten;
using Wolverine;
using Wolverine.Marten;

// ReSharper disable UnusedMember.Global

namespace Admin.Api.Domain.Lasertag;

#pragma warning disable S1118

public static class GameCommandHandlers
{
    public class StartGameHandler
    {
        public static async Task Handle(
            LasertagCommands.StartGame @event,
            IDocumentSession session,
            ILogger logger,
            IMessageBus bus)
        {
            logger.LogInformation(
                "Starting Game with Id: {GameId} for duration: {Duration}",
                @event.GameId,
                @event.GameDuration);

            var endGame = new LasertagCommands.EndGame(@event.GameId);
            await bus.SendAsync(endGame, new DeliveryOptions
            {
                ScheduleDelay = @event.GameDuration
            });

            var gameStarted = new LasertagEvents.GameStarted(@event.GameId);
            await session.Events.WriteToAggregate<Game>(@event.GameId, stream =>
            {
                stream.AppendOne(gameStarted);
            });
        }
    }

    public class EndGameHandler
    {
        public static async Task Handle(
            LasertagCommands.EndGame @event,
            IDocumentSession session,
            ILogger logger,
            IMessageBus bus)
        {
            logger.LogInformation(
                "Ending Game with Id: {GameId}",
                @event.GameId);

            var gameFinished = new LasertagEvents.GameFinished(@event.GameId);
            await session.Events.WriteToAggregate<Game>(@event.GameId, stream =>
            {
                if (!stream.Aggregate.IsGameRunning)
                {
                    return;
                }

                stream.AppendOne(gameFinished);
            });

            await bus.SendAsync(gameFinished);
        }
    }

    public class DeleteGameHandler
    {
        public static void Handle(
            LasertagCommands.DeleteGame @event,
            IDocumentSession session,
            ILogger logger)
        {
            logger.LogInformation(
                "Deleting Game with Id: {GameId}",
                @event.GameId);

            session.Events.ArchiveStream(@event.GameId);
        }
    }
}

#pragma warning restore S1118