using Lasertag.DomainModel;
using Lasertag.DomainModel.DomainEvents;
using Lasertag.DomainModel.DomainEvents.GameEvents;
using Lasertag.DomainModel.DomainEvents.RoundEvents;
using Lasertag.Manager;
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

    public Task<ApiResult<GameRound>> Fire(Guid gameRoundId, Guid sourceGameSetId)
    {
        return EventRaiser.RaiseEventWithChecks(gameRoundId, gameRound =>
        {
            if (gameRound.Status != GameRoundStatus.Started)
            {
                throw new InvalidStateException("Shots are not gonna be registered right now!");
            }

            var activeGameSet = gameRound.ActiveGameSets.FirstOrDefault(gs => gs.GameSetId == sourceGameSetId);
            if (activeGameSet == null)
            {
                throw new InvalidOperationException($"LasertagSet with ID {sourceGameSetId} is not active");
            }

            var sourceGroup = GetGroup(gameRound, sourceGameSetId);
            return new PlayerFiredShot(sourceGameSetId, activeGameSet.PlayerId, sourceGroup.GroupId);
        });
    }

    static GameGroup GetGroup(GameRound game, Guid gameSetId)
    {
        return game.GameSetGroups.First(gsg => gsg.GameSets.Any(gs => gs.GameSetId == gameSetId));
    }

    public Task<ApiResult<GameRound>> Hit(Guid gameRoundId, Guid sourceGameSetId, Guid targetGameSetId)
    {
        return EventRaiser.RaiseEventWithChecks(gameRoundId, gameRound =>
        {
            if (gameRound.Status != GameRoundStatus.Started)
            {
                throw new InvalidStateException("Hits are not gonna be registered right now!");
            }

            var sourceActiveGameSet = gameRound.ActiveGameSets.FirstOrDefault(gs => gs.GameSetId == sourceGameSetId);
            var targetActiveGameSet = gameRound.ActiveGameSets.FirstOrDefault(gs => gs.GameSetId == targetGameSetId);
            if (sourceActiveGameSet == null || targetActiveGameSet == null)
            {
                throw new InvalidOperationException($"Either LasertagSet with ID {sourceGameSetId} or {targetGameSetId} is not active");
            }

            var sourceGroup = GetGroup(gameRound, sourceGameSetId);
            var targetGroup = GetGroup(gameRound, targetGameSetId);
            return new PlayerGotHit(sourceGameSetId, targetGameSetId, sourceActiveGameSet.PlayerId, targetActiveGameSet.PlayerId, sourceGroup.GroupId, targetGroup.GroupId);
        });
    }
}