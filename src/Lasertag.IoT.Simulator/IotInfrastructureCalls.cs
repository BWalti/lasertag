﻿using System.Net.Http.Json;
using Admin.Api.Domain.Lasertag;
using Microsoft.Extensions.Logging;

namespace Lasertag.IoT.Simulator;

public static class ApiRouteBuilder
{
    public static string RegisterGameSet(Server server) =>
        $"/api/lasertag/server/{server.Id}/registerGameSet";

    public static string PrepareGame(Server server) =>
        $"/api/lasertag/server/{server.Id}/prepareGame";

    public static string StartGame(Game game, TimeSpan gameDuration) =>
        $"/api/lasertag/game/{game.Id}/start?gameDuration={gameDuration}";

    public static string DeleteGame(Game game) =>
        $"/api/lasertag/game/{game.Id}";
}

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

        var result = await _httpClient.PostAsync(ApiRouteBuilder.RegisterGameSet(server), null);
        result.EnsureSuccessStatusCode();

        return await result.Content.ReadFromJsonAsync<LasertagEvents.GameSetRegistered>();
    }

    public async Task<LasertagEvents.GamePrepared?> PrepareGame(Server server, LobbyConfiguration lobbyConfiguration)
    {
        _logger.LogInformation("Preparing Game on Server: {ServerId} with LobbyConfiguration: {Lobby}", server.Id,
            lobbyConfiguration);

        var result = await _httpClient.PostAsJsonAsync(ApiRouteBuilder.PrepareGame(server), lobbyConfiguration);
        result.EnsureSuccessStatusCode();

        return await result.Content.ReadFromJsonAsync<LasertagEvents.GamePrepared>();
    }

    public async Task StartGame(Game game, TimeSpan gameDuration)
    {
        _logger.LogInformation("Starting Game: {GameId} for Duration: {Duration}", game.Id, gameDuration);

        var result = await _httpClient.PostAsync(ApiRouteBuilder.StartGame(game, gameDuration), null);
        result.EnsureSuccessStatusCode();
    }

    public async Task DeleteGame(Game game)
    {
        _logger.LogInformation("Deleting Game: {GameId}", game.Id);

        var result = await _httpClient.DeleteAsync(ApiRouteBuilder.DeleteGame(game));
        result.EnsureSuccessStatusCode();
    }
}