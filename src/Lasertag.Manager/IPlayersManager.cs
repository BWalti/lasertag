using Lasertag.DomainModel;
using Orleans;

namespace Lasertag.Manager;

public interface IPlayersManager : IGrainWithGuidKey
{
    Task<GameRound?> GetResult();
}