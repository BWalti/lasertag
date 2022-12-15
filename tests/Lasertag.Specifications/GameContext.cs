using FluentAssertions;
using Lasertag.Api;
using Lasertag.DomainModel;
using LightBDD.Framework;
using LightBDD.Framework.Scenarios;

namespace Lasertag.Specifications;
#pragma warning disable S101

public class GameContext
{
    readonly IGameCommands _gameCommands;
    readonly IGameRoundCommands _gameRoundCommands;
    readonly IGameRoundQueries _gameRoundQueries;
    ApiResult<Game>? _game;

    Guid _gameId = Guid.Empty;
    ApiResult<GameRound>? _gameRound;

    Guid[]? _gameSetIds;

    public GameContext(IClusterClient client)
    {
        _gameCommands = client.GetGrain<IGameCommands>(0);
        _gameRoundCommands = client.GetGrain<IGameRoundCommands>(0);
        _gameRoundQueries = client.GetGrain<IGameRoundQueries>(0);
    }

    public Task Given_gameId_is_set()
    {
        _gameId = Guid.NewGuid();
        return Task.CompletedTask;
    }

    public async Task When_Initializing_Game()
    {
        _game = await _gameCommands.InitializeGame(_gameId);
    }

    public Task Then_Game_Is_Created()
    {
        _game.Should().NotBeNull();
        _game!.Success.Should().BeTrue();

        _game.Output.Should().NotBeNull();
        _game.Output!.GameId.Should().Be(_gameId);

        return Task.CompletedTask;
    }

    public Task<CompositeStep> Given_lobby_is_created_with_4_sets_and_two_groups() =>
        CompositeStep.DefineNew()
            .AddAsyncSteps(
                Given_gameId_is_set,
                Given_game_is_initialized)
            .AddAsyncSteps(
                _ => Given_number_of_gameSets_registered(4),
                _ => Given_number_of_gameSets_connected(4),
                _ => When_Lobby_is_created_for_number_of_groups(2))
            .AddAsyncSteps(
                Then_each_group_has_two_GameSets_available)
            .Build()
            .AsTask();

    public async Task Then_each_group_has_two_GameSets_available()
    {
        _gameRound = await _gameRoundQueries.GetGameRound(_game!.Output!.ActiveRoundId);
        _gameRound.Should().NotBeNull();
        _gameRound.Output.Should().NotBeNull();

        var gameSetGroups = _gameRound.Output!.GameSetGroups;
        gameSetGroups.Should().HaveCount(2);
        gameSetGroups[0].GameSets.Should().HaveCount(2);
        gameSetGroups[1].GameSets.Should().HaveCount(2);
    }

    public async Task When_Lobby_is_created_for_number_of_groups(int numberOfGroups)
    {
        _game = await _gameCommands.CreateLobby(_gameId, numberOfGroups);
    }

    public async Task Given_number_of_gameSets_connected(int numberOfGameSets)
    {
        for (var i = 0; i < numberOfGameSets; i++)
        {
            _game = await _gameCommands.ConnectGameSet(_gameId, _gameSetIds![i]);
        }
    }

    public async Task Given_number_of_gameSets_registered(int numberOfGameSets)
    {
        await When_I_register_gameSets(numberOfGameSets);
    }

    public async Task Given_game_is_initialized()
    {
        _game = await _gameCommands.InitializeGame(_gameId);
    }

    public async Task When_I_register_gameSets(int numberOfGameSets)
    {
        _gameSetIds = new Guid[numberOfGameSets];

        for (var i = 0; i < numberOfGameSets; i++)
        {
            _gameSetIds[i] = Guid.NewGuid();

            _game = await _gameCommands.RegisterGameSet(_gameId, new GameSetConfiguration
            {
                Id = _gameSetIds[i],
                IsTargetOnly = false
            });
        }
    }

    public Task Then_gameSets_are_registered(int numberOfGameSets)
    {
        _game.Should().NotBeNull();
        _game!.Output.Should().NotBeNull();
        _game!.Output!.GameSets.Should().HaveCount(numberOfGameSets);

        return Task.CompletedTask;
    }
}