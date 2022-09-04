using Lasertag.DomainModel;
using Lasertag.DomainModel.DomainEvents;

namespace Lasertag.Manager;

public interface IGameRoundManager : IDomainManager<GameRound, GameRoundState, IDomainEventBase>
{
}