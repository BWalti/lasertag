using Marten;
using Wolverine;

// ReSharper disable UnusedMember.Global

namespace Admin.Api.Domain.Lasertag;

#pragma warning disable S1118

public static class GameCommandHandlers
{
    public class EndGameHandler
    {
        public static async Task<LasertagEvents.GameFinished> Handle(
            LasertagCommands.EndGame @event,
            IDocumentSession session,
            ILogger logger,
            IMessageBus bus)
        {
            logger.LogInformation(
                "Ending Game with Id: {GameId}",
                @event.GameId);

            var gameFinished = new LasertagEvents.GameFinished(@event.ServerId, @event.GameId);
            await session.Events.WriteToAggregate<Game>(@event.GameId, stream =>
            {
                if (stream.Aggregate.Status != GameStatus.Started)
                {
                    return;
                }

                stream.AppendOne(gameFinished);
            });

            await bus.SendAsync(gameFinished);

            return gameFinished;
        }
    }
}

#pragma warning restore S1118