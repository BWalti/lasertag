using Lasertag.DomainModel;
using Lasertag.DomainModel.DomainEvents;
using static Lasertag.DomainModel.DomainEvents.GameRoundEvents;
using Lasertag.Manager.GameRound;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Concurrency;

namespace Lasertag.Api;

[Reentrant]
[StatelessWorker]
public class GameRoundCommands : Grain, IGameRoundCommands
{
    public GameRoundCommands(ILogger<GameCommands> logger)
    {
        EventRaiser = new EventRaiser<IGameRoundManager, GameRound, GameRoundState, IDomainEventBase>(logger, GrainFactory);
    }

    public EventRaiser<IGameRoundManager, GameRound, GameRoundState, IDomainEventBase> EventRaiser { get; }


    public async Task<ApiResult<GameRound>> CreateLobby(Guid gameRoundId, GameGroup[] gameSetGroups)
    {
        return await EventRaiser.RaiseEvent(gameRoundId, () => new LobbyCreated(gameRoundId, gameSetGroups));
    }

    public async Task<ApiResult<GameRound>> ActivateGameSet(Guid gameRoundId, Guid gameSetId, Guid playerId)
    {
        return await EventRaiser.RaiseEventWithChecks(gameRoundId, gameRound =>
        {
            if (gameRound.Status != GameRoundStatus.Created)
            {
                throw new InvalidStateException("Game is not ready for players!");
            }

            if (gameRound.ActiveGameSets.All(gs => gs.GameSetId != gameSetId))
            {
                throw new InvalidOperationException("This LasertagSet is unknown!");
            }

            if (gameRound.ActiveGameSets.Any(gs => gs.GameSetId == gameSetId))
            {
                throw new InvalidOperationException("This LasertagSet is already active!");
            }

            if (gameRound.ActiveGameSets.Any(gs => gs.PlayerId == playerId))
            {
                throw new InvalidOperationException("This Player is already active!");
            }

            return new GameSetActivated(gameRoundId, playerId, gameSetId);
        });
    }


    public async Task<ApiResult<GameRound>> Start(Guid gameRoundId)
    {
        return await EventRaiser.RaiseEventWithChecks(gameRoundId, gameRound =>
        {
            if (gameRound.Status != GameRoundStatus.Created)
            {
                throw new InvalidStateException("GameRound can only be started once!");
            }

            return new Started(gameRoundId);
        });
    }

    public Task<ApiResult<GameRound>> Fire(Guid gameRoundId, Guid sourceLasertagSetId)
    {
        return EventRaiser.RaiseEventWithChecks(gameRoundId, gameRound =>
        {
            if (gameRound.Status != GameRoundStatus.Started)
            {
                throw new InvalidStateException("Shots are not gonna be registered right now!");
            }

            var activeGameSet = gameRound.ActiveGameSets.FirstOrDefault(gs => gs.GameSetId == sourceLasertagSetId);
            if (activeGameSet == null)
            {
                throw new InvalidOperationException($"LasertagSet with ID {sourceLasertagSetId} is not active");
            }

            var sourceGroup = GetGroup(gameRound, sourceLasertagSetId);
            return new PlayerFiredShot(gameRoundId, sourceLasertagSetId, activeGameSet.PlayerId, sourceGroup.GroupId);
        });
    }

    static GameGroup GetGroup(GameRound game, Guid gameSetId)
    {
        return game.GameSetGroups.First(gsg => gsg.GameSets.Any(gs => gs.Id == gameSetId));
    }

    public Task<ApiResult<GameRound>> Hit(Guid gameRoundId, Guid sourceLasertagSetId, Guid targetGameSetId)
    {
        return EventRaiser.RaiseEventWithChecks(gameRoundId, gameRound =>
        {
            if (gameRound.Status != GameRoundStatus.Started)
            {
                throw new InvalidStateException("Hits are not gonna be registered right now!");
            }

            var sourceActiveGameSet = gameRound.ActiveGameSets.FirstOrDefault(gs => gs.GameSetId == sourceLasertagSetId);
            var targetActiveGameSet = gameRound.ActiveGameSets.FirstOrDefault(gs => gs.GameSetId == targetGameSetId);
            if (sourceActiveGameSet == null || targetActiveGameSet == null)
            {
                throw new InvalidOperationException($"Either LasertagSet with ID {sourceLasertagSetId} or {targetGameSetId} is not active");
            }

            var sourceGroup = GetGroup(gameRound, sourceLasertagSetId);
            var targetGroup = GetGroup(gameRound, targetGameSetId);
            return new PlayerGotHitBy(gameRoundId, sourceLasertagSetId, targetGameSetId, sourceActiveGameSet.PlayerId, targetActiveGameSet.PlayerId, sourceGroup.GroupId, targetGroup.GroupId);
        });
    }
}