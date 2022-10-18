using Admin.Api;
using Lasertag.Api;
using Microsoft.AspNetCore.Mvc;
using Orleans;

var builder = WebApplication
    .CreateBuilder(args)
    .UseDefaultInfrastructure()
    .UseSwaggerGeneratorHack(args);

builder.Services.AddTransient(provider =>
{
    var clusterClient = provider.GetRequiredService<IClusterClient>();
    return clusterClient.GetGrain<IGameCommands>(0);
});

builder.Services.AddTransient(provider =>
{
    var clusterClient = provider.GetRequiredService<IClusterClient>();
    return clusterClient.GetGrain<IGameRoundCommands>(0);
});

var app = builder.Build()
    .UseDevelopmentDefaults();

app.MapPost("/game/init",
        async (IGameCommands gameCommands) => { return await gameCommands.InitializeGame(Guid.NewGuid()); })
    .WithDisplayName("Initialize Game");

app.MapPost("/game/{gameId}/{gameSetId}/connect",
    async(IGameCommands gameCommands, [FromRoute] Guid gameId, [FromRoute] Guid gameSetId) =>
    {
    return await gameCommands.ConnectGameSet(gameId, gameSetId);
}).WithDisplayName("Connect GameSet");

app.MapPost("/game/{gameId}/createLobby",
    async(IGameCommands gameCommands, [FromRoute] Guid gameId, [FromQuery] int ? numberOfGroups) =>
    {
    return await gameCommands.CreateLobby(gameId, numberOfGroups ?? 2);
}).WithDisplayName("Open Lobby");

app.MapPost("/game/{gameId}/{gameSetId}/activate/{playerId}",
    async(IGameCommands gameCommands, [FromRoute] Guid gameId, [FromRoute] Guid gameSetId,
        [FromRoute] Guid playerId) =>
    {
    return await gameCommands.ActivateGameSet(gameId, gameSetId, playerId);
}).WithDisplayName("Activate GameSet");

app.MapPost("/game/{gameId}/start",
    async(IGameCommands gameCommands, [FromRoute] Guid gameId) =>
    {
    var startGameRound = await gameCommands.StartGameRound(gameId);

    return new GameRoundStartResult(startGameRound.Item1, startGameRound.Item2);
}).WithDisplayName("Start GameRound");

app.MapPost("/gameRound/{gameRoundId}/lasertagSet/{sourceLasertagSetId}/shotFired",
    async(IGameRoundCommands gameRoundCommands, [FromRoute] Guid gameRoundId, [FromRoute] Guid sourceLasertagSetId) =>
    {
    return await gameRoundCommands.Fire(gameRoundId, sourceLasertagSetId);
}).WithDisplayName("LasertagSet Fired Shot");

app.MapPost("/gameRound/{gameRoundId}/lasertagSet/{sourceLasertagSetId}/got-hit-from-lasertagSet/{targetLasertagSetId}",
    async(IGameRoundCommands gameRoundCommands, [FromRoute] Guid gameRoundId, [FromRoute] Guid sourceLasertagSetId,
        [FromRoute] Guid targetLasertagSetId) =>
    {
    return await gameRoundCommands.Hit(gameRoundId, sourceLasertagSetId, targetLasertagSetId);
}).WithDisplayName("LasertagSet Got Hit by other LasertagSet");

app.Run();