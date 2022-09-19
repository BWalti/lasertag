using Lasertag.DomainModel;
using Lasertag.DomainModel.DomainEvents;
using Lasertag.DomainModel.DomainEvents.GameEvents;
using Lasertag.DomainModel.DomainEvents.RoundEvents;
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

    public async Task<ApiResult<GameRound>> StartGameRound(Guid gameRoundId,
        IEnumerable<ActiveGameSet> activeLasertagSets, IEnumerable<GameGroup> groups)
    {
        return await EventRaiser.RaiseEventWithChecks(gameRoundId, gameRound =>
        {
            if (gameRound.Status != GameRoundStatus.Created)
            {
                throw new InvalidStateException("GameRound can only be started once!");
            }

            return new GameRoundStarted(gameRoundId, activeLasertagSets, groups);
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
            return new PlayerFiredShot(sourceLasertagSetId, activeGameSet.PlayerId, sourceGroup.GroupId);
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
            return new PlayerGotHitBy(sourceLasertagSetId, targetGameSetId, sourceActiveGameSet.PlayerId, targetActiveGameSet.PlayerId, sourceGroup.GroupId, targetGroup.GroupId);
        });
    }
}