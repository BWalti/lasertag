using GrainInterfaces.Models;
using Orleans;
using Orleans.Concurrency;

namespace GrainInterfaces;

public interface IGameManagerGrain : IGrainWithGuidKey
{
    Task<int> GetOnlineGameSets();

    Task GameSetOnline(Guid id);
    Task GameSetOffline(Guid id);
    
    Task<IGameGrain> PrepareGame(GameConfiguration config);
}

public interface IGameGrain : IGrainWithGuidKey
{
    /// <summary>
    /// When a game gets prepared, the GameManager creates a new game and sets the "PreparedGroups" meaning which GameSet would belong to which group (color).
    /// </summary>
    /// <param name="groups">The GameGroups</param>
    Task SetPreparedGameSetGroups(Immutable<GameGroup[]> groups);
    
    /// <summary>
    /// Gets the prepared GameGroups (color).
    /// </summary>
    Task<Immutable<GameGroup[]>> GetPreparedGameSetGroups();

    /// <summary>
    /// If a player activates a GameSet (e.g. by putting a NFC card on the reader)
    /// </summary>
    /// <param name="gameSetId">The ID of the GameSet (Gun / Vest)</param>
    /// <param name="playerId">The ID of the player (NFC card)</param>
    Task OnGameSetActivated(Guid gameSetId, Guid playerId);

    /// <summary>
    /// Starts the game (with all the activated GameSets, thus not all "prepared ones" may actually be used).
    /// </summary>
    Task StartGame();


}