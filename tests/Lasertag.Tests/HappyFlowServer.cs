using FluentAssertions;
using Lasertag.Core.Domain.Lasertag;
using Lasertag.Tests.TestInfrastructure;
using Xunit;
using Xunit.Abstractions;

namespace Lasertag.Tests;

[Collection("integration")]
public class HappyFlowServer : IntegrationContext
{
    readonly ITestOutputHelper _outputHelper;

    public HappyFlowServer(AppFixture fixture, ITestOutputHelper outputHelper) : base(fixture)
    {
        _outputHelper = outputHelper;
    }

    [Fact]
    public async Task HappyFlow()
    {
        var numberOfGameSets = 2;
        var numberOfTeams = 2;
        var gameDuration = TimeSpan.FromSeconds(1);

        var gameInfra = await GameInfraBuilder.Create(this)
            .WithGameDuration(gameDuration)
            .WithRegisteredGameSetCount(numberOfGameSets)
            .WithConnectedGameSetCount(numberOfGameSets)
            .WithPreparedGame(numberOfTeams)
            .Build();

        await gameInfra.ReloadGame(g =>
        {
            g.Lobby.Teams.Should().HaveCount(numberOfTeams);
            g.Lobby.Teams[0].Should().NotBeNull();
            g.Lobby.Teams[1].Should().NotBeNull();
            g.Lobby.Teams[0].GameSets.Should().HaveCount(1);
            g.Lobby.Teams[1].GameSets.Should().HaveCount(1);
        });

        await gameInfra.ActivateGameSet(0, 1);
        await gameInfra.ActivateGameSet(1, 2);
        await Task.Delay(TimeSpan.FromSeconds(1));

        await gameInfra.StartGame();
        await gameInfra.ReloadGame(g => g.Status.Should().Be(GameStatus.Started));

        await gameInfra.Shoot(0);
        await Task.WhenAll(
            gameInfra.Shoot(0),
            gameInfra.Shoot(1));

        await gameInfra.GotHit(1, 0, 1);

        var awaitShotsTime = gameDuration / 3;
        await Task.Delay(awaitShotsTime);
        await gameInfra.ReloadGame(g =>
        {
            g.Statistics.ShotsFired.Should().Be(3);
            g.Statistics.GotHit.Should().Be(1);
        });

        // await end of game:
        await Task.Delay(1.05 * gameDuration - awaitShotsTime);
        await gameInfra.ReloadGame(g => g.Status.Should().Be(GameStatus.Finished));

        await gameInfra.DeleteGame();

        // now as the game got deleted, we shouldn't be able to load it anymore:
        var game = await gameInfra.ReloadGame(mustExist: false);
        game.Should().BeNull();
    }
}