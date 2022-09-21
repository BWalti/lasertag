﻿using Lasertag.DomainModel;
using Orleans;

namespace Lasertag.Api;

public interface IGameRoundCommands : IGrainWithIntegerKey
{
    Task<ApiResult<GameRound>> StartGameRound(Guid gameRoundId, IEnumerable<ActiveGameSet> activeLasertagSets,
        IEnumerable<GameGroup> groups);
    Task<ApiResult<GameRound>> Fire(Guid gameRoundId, Guid sourceLasertagSetId);
    Task<ApiResult<GameRound>> Hit(Guid gameRoundId, Guid sourceLasertagSetId, Guid targetGameSetId);
}