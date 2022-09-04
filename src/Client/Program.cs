using Lasertag.Api;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Hosting;

var host = Host.CreateDefaultBuilder(args);
host.ConfigureServices(services =>
{
    services.AddLogging();
    services.AddOrleansClient(builder => { builder.UseLocalhostClustering(); });
});

host.ConfigureLogging(builder =>
{
    builder
        .AddFilter("Microsoft", LogLevel.Warning) // generic host lifecycle messages
        .AddFilter("Orleans", LogLevel.Information) // suppress status dumps
        .AddFilter("Runtime", LogLevel.Warning) // also an Orleans prefix
        .AddDebug() // VS Debug window
        .AddConsole();
});

var app = host.Build();

await app.StartAsync();

var client = app.Services.GetRequiredService<IClusterClient>();

var gameCommands = client.GetGrain<IGameCommands>(0);

var gameId = Guid.NewGuid();
var apiResult = await gameCommands.InitializeGame(gameId);
apiResult.EnsureSuccess();

var gameSetOneId = Guid.NewGuid();
apiResult = await gameCommands.ConnectGameSet(gameId, gameSetOneId);
apiResult.EnsureSuccess();

var gameSetTwoId = Guid.NewGuid();
apiResult = await gameCommands.ConnectGameSet(gameId, gameSetTwoId);
apiResult.EnsureSuccess();

apiResult = await gameCommands.CreateLobby(gameId, 2);
apiResult.EnsureSuccess();

var playerOneId = Guid.NewGuid();
apiResult = await gameCommands.ActivateGameSet(gameId, gameSetOneId, playerOneId);
apiResult.EnsureSuccess();

var playerTwoId = Guid.NewGuid();
apiResult = await gameCommands.ActivateGameSet(gameId, gameSetTwoId, playerTwoId);
apiResult.EnsureSuccess();

(apiResult, var apiResult2) = await gameCommands.StartGameRound(gameId);
apiResult.EnsureSuccess();
apiResult2.EnsureSuccess();

Console.ReadLine();
await app.StopAsync();