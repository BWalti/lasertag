using Lasertag.DomainModel;
using Lasertag.DomainModel.DomainEvents;

namespace Lasertag.Manager;

public interface IGameManager : IDomainManager<Game, GameState, IDomainEventBase>
{
}