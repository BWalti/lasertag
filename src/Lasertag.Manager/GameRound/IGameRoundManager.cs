using Lasertag.DomainModel.DomainEvents;

namespace Lasertag.Manager.GameRound;

public interface IGameRoundManager : IDomainManager<DomainModel.GameRound, GameRoundState, IDomainEventBase>
{
}