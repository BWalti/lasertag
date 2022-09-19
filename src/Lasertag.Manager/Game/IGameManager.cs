using Lasertag.DomainModel.DomainEvents;

namespace Lasertag.Manager.Game;

public interface IGameManager : IDomainManager<DomainModel.Game, GameState, IDomainEventBase>
{
}