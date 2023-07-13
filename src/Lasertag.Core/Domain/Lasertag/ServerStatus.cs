namespace Lasertag.Core.Domain.Lasertag;

public enum ServerStatus
{
    /// <summary>
    /// Server created
    /// </summary>
    Created,

    /// <summary>
    /// Marks that this server would be ready for games,
    /// meaning it has at least two GameSets registered
    /// and currently no game is prepared / running.
    /// </summary>
    ReadyForLobby,

    /// <summary>
    /// Lobby created.
    /// </summary>
    GamePrepared,

    /// <summary>
    /// After the game is finished, the server state switches back to ReadyForLobby.
    /// </summary>
    GameRunning,
}