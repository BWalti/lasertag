using Lasertag.Api;
using Lasertag.DomainModel;

namespace Admin.Api;

public record GameRoundStartResult(ApiResult<Game> Game, ApiResult<GameRound> GameRound);