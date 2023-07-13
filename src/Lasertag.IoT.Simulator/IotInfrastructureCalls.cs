using System.Net.Http.Json;
using Admin.Api;
using Lasertag.Core.Domain.Lasertag;
using Microsoft.Extensions.Logging;

namespace Lasertag.IoT.Simulator;

public class IotInfrastructureCalls
{
    readonly HttpClient _httpClient;
    readonly ILogger<IotInfrastructureCalls> _logger;

    public IotInfrastructureCalls(HttpClient httpClient, ILogger<IotInfrastructureCalls> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<LasertagEvents.GameSetRegistered?> RegisterGameSet(Server server)
    {
        _logger.LogInformation("Going to register game set on Server: {ServerId}", server.Id);

        var result = await _httpClient.PostAsync(ApiRouteBuilder.RegisterGameSetPath, null);
        result.EnsureSuccessStatusCode();

        return await result.Content.ReadFromJsonAsync<LasertagEvents.GameSetRegistered>();
    }

    public async Task<LasertagEvents.GamePrepared?> PrepareGame(Server server, LobbyConfiguration lobbyConfiguration)
    {
        _logger.LogInformation("Preparing Game on Server: {ServerId} with LobbyConfiguration: {Lobby}", server.Id,
            lobbyConfiguration);

        var result = await _httpClient.PostAsJsonAsync(ApiRouteBuilder.PrepareGamePath, lobbyConfiguration);
        result.EnsureSuccessStatusCode();

        return await result.Content.ReadFromJsonAsync<LasertagEvents.GamePrepared>();
    }

    public async Task StartGame(Game game, TimeSpan gameDuration)
    {
        _logger.LogInformation("Starting Game: {GameId} for Duration: {Duration}", game.Id, gameDuration);

        var result = await _httpClient.PostAsync(ApiRouteBuilder.StartGamePath, null);
        result.EnsureSuccessStatusCode();
    }

    public async Task DeleteGame(Game game)
    {
        _logger.LogInformation("Deleting Game: {GameId}", game.Id);

        var result = await _httpClient.DeleteAsync(ApiRouteBuilder.DeleteGamePath);
        result.EnsureSuccessStatusCode();
    }
}