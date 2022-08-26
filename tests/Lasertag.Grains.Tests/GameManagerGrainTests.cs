using FluentAssertions;
using GrainInterfaces;
using GrainInterfaces.Models;
using Xunit;

namespace Lasertag.Grains.Tests;

[Collection(nameof(OrleansCollection))]
public class GameManagerGrainTests
{
    private readonly OrleansFixture _fixture;
    private readonly IGameManagerGrain _gameManagerGrain;
    
    public GameManagerGrainTests(OrleansFixture fixture)
    {
        _fixture = fixture;
        _gameManagerGrain = _fixture.ClusterClient.GetGrain<IGameManagerGrain>(Guid.NewGuid());
    }

    [Fact]
    public async Task GameManager_WhenInitialized_ThenNoGameSetsOnline()
    {
        var gameSets = await _gameManagerGrain.GetOnlineGameSets();
        gameSets.Should().Be(0);
    }
    
    [Fact]
    public async Task GameManager_WhenInitializedAndOneGameSetOnline_ThenOneOnline()
    {
        var gameSetOne = Guid.NewGuid();

        await _gameManagerGrain.GameSetOnline(gameSetOne);

        var gameSets = await _gameManagerGrain.GetOnlineGameSets();
        gameSets.Should().Be(1);
    }
    
    [Fact]
    public async Task GameManager_WhenInitializedAndOneGameSetOnlineTwice_ThenOneOnline()
    {
        var gameSetOne = Guid.NewGuid();

        await _gameManagerGrain.GameSetOnline(gameSetOne);
        await _gameManagerGrain.GameSetOnline(gameSetOne);

        var gameSets = await _gameManagerGrain.GetOnlineGameSets();
        gameSets.Should().Be(1);
    }
    
    [Fact]
    public async Task GameManager_WhenInitializedAndTwoGameSetsOnline_ThenTwoOnline()
    {
        var gameSetOne = Guid.NewGuid();
        var gameSetTwo = Guid.NewGuid();

        await _gameManagerGrain.GameSetOnline(gameSetOne);
        await _gameManagerGrain.GameSetOnline(gameSetTwo);

        var gameSets = await _gameManagerGrain.GetOnlineGameSets();
        gameSets.Should().Be(2);
    }
    
    [Fact]
    public async Task GameManager_PrepareGame()
    {
        var gameSetOne = Guid.NewGuid();
        var gameSetTwo = Guid.NewGuid();
        var gameSetThree = Guid.NewGuid();
        var gameSetFour = Guid.NewGuid();

        await _gameManagerGrain.GameSetOnline(gameSetOne);
        await _gameManagerGrain.GameSetOnline(gameSetTwo);
        await _gameManagerGrain.GameSetOnline(gameSetThree);
        await _gameManagerGrain.GameSetOnline(gameSetFour);

        var gameSets = await _gameManagerGrain.GetOnlineGameSets();
        gameSets.Should().Be(4);

        var numberOfGroups = 2;
        var game = await _gameManagerGrain.PrepareGame(new GameConfiguration(numberOfGroups));

        var groups = await game.GetPreparedGameSetGroups();
        groups.Value.Should().HaveCount(numberOfGroups);

        var playerOneId = Guid.NewGuid();
        await game.OnGameSetActivated(gameSetOne, playerOneId);

        var playerTwoId = Guid.NewGuid();
        await game.OnGameSetActivated(gameSetTwo, playerTwoId);

        await game.StartGame();
    }
}