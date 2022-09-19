using Admin.Api;
using EasyNetQ.ConnectionString;
using Lasertag.Api;
using Microsoft.AspNetCore.Mvc;
using Orleans;
using Orleans.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Logging
    .AddFilter("Microsoft", LogLevel.Warning) // generic host lifecycle messages
    .AddFilter("Orleans", LogLevel.Information) // suppress status dumps
    .AddFilter("Runtime", LogLevel.Warning) // also an Orleans prefix
    .AddDebug() // VS Debug window
    .AddConsole();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();

if (args.Contains("start"))
{
    builder.Services.AddOrleansClient(clientBuilder => { clientBuilder.UseLocalhostClustering(); });
    builder.Services.RegisterEasyNetQ(resolver =>
    {
        var parser = resolver.Resolve<IConnectionStringParser>();
        var configuration = resolver.Resolve<IConfiguration>();

        var connectionString = configuration["Mq:Host"];
        return parser.Parse(connectionString);
    });
}
else
{
    builder.Services.AddSingleton<IClusterClient>(provider => new MockClusterClient(provider));
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseCors(corsBuilder => corsBuilder.WithOrigins("http://localhost:5173").AllowAnyMethod().AllowAnyHeader());

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPost("/game/init", async (IClusterClient clusterClient) =>
{
    var gameCommands = clusterClient.GetGrain<IGameCommands>(0);
    return await gameCommands.InitializeGame(Guid.NewGuid());
}).WithDisplayName("Initialize Game");

app.MapPost("/game/{gameId}/{gameSetId}/connect",
    async(IClusterClient clusterClient, [FromRoute] Guid gameId, [FromRoute] Guid gameSetId) =>
    {
    var gameCommands = clusterClient.GetGrain<IGameCommands>(0);
    return await gameCommands.ConnectGameSet(gameId, gameSetId);
}).WithDisplayName("Connect GameSet");

app.MapPost("/game/{gameId}/createLobby",
    async(IClusterClient clusterClient, [FromRoute] Guid gameId, [FromQuery] int ? numberOfGroups) =>
    {
    var gameCommands = clusterClient.GetGrain<IGameCommands>(0);
    return await gameCommands.CreateLobby(gameId, numberOfGroups ?? 2);
}).WithDisplayName("Open Lobby");

app.MapPost("/game/{gameId}/{gameSetId}/activate/{playerId}",
    async(IClusterClient clusterClient, [FromRoute] Guid gameId, [FromRoute] Guid gameSetId, [FromRoute]Guid playerId) =>
    {
    var gameCommands = clusterClient.GetGrain<IGameCommands>(0);
    return await gameCommands.ActivateGameSet(gameId, gameSetId, playerId);
}).WithDisplayName("Activate GameSet");

app.MapPost("/game/{gameId}/start",
    async(IClusterClient clusterClient, [FromRoute] Guid gameId) =>
    {
    var gameCommands = clusterClient.GetGrain<IGameCommands>(0);
    var startGameRound = await gameCommands.StartGameRound(gameId);

    return new GameRoundStartResult(startGameRound.Item1, startGameRound.Item2);
}).WithDisplayName("Start GameRound");

app.MapPost("/gameRound/{gameRoundId}/lasertagSet/{sourceLasertagSetId}/shotFired", async(IClusterClient clusterClient, [FromRoute] Guid gameRoundId, [FromRoute] Guid sourceLasertagSetId) =>
{
    var gameRoundCommands = clusterClient.GetGrain<IGameRoundCommands>(0);
    return await gameRoundCommands.Fire(gameRoundId, sourceLasertagSetId);
}).WithDisplayName("LasertagSet Fired Shot");

app.MapPost("/gameRound/{gameRoundId}/lasertagSet/{sourceLasertagSetId}/got-hit-from-lasertagSet/{targetLasertagSetId}", async(IClusterClient clusterClient, [FromRoute] Guid gameRoundId, [FromRoute] Guid sourceLasertagSetId, [FromRoute] Guid targetLasertagSetId) =>
{
    var gameRoundCommands = clusterClient.GetGrain<IGameRoundCommands>(0);
    return await gameRoundCommands.Hit(gameRoundId, sourceLasertagSetId, targetLasertagSetId);
}).WithDisplayName("LasertagSet Got Hit by other LasertagSet");

app.Run();