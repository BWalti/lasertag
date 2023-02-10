using Admin.Api.Domain.Lasertag;
using Alba;
using FluentAssertions;
using Lasertag.Tests.TestInfrastructure;
using Wolverine;
using Wolverine.Tracking;
using Xunit;
using static Admin.Api.Domain.Lasertag.LasertagEvents;

namespace Lasertag.Tests;

[Collection("integration")]
public class HappyFlowServer : IntegrationContext
{
    public HappyFlowServer(AppFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task HappyFlow()
    {
        // Arrange:
        var server = new Server
        {
            Name = "Test Server",
            Id = 42
        };

        await using (var session = Store.LightweightSession())
        {
            session.Store(server);
            await session.SaveChangesAsync();
        }

        // Register GameSets:
        var gameSetRegistered1 = await RegisterGameSet(server);
        gameSetRegistered1.GameSetId.Should().Be(1);

        var gameSetRegistered2 = await RegisterGameSet(server);
        gameSetRegistered2.GameSetId.Should().Be(2);

        await using (var session = Store.LightweightSession())
        {
            var persisted = await session.LoadAsync<Server>(server.Id);
            persisted.Should().NotBeNull();
            persisted!.GameSets.Should().HaveCount(2);
        }

        var gamePrepared = await PrepareGame(server, new LobbyConfiguration
        {
            NumberOfTeams = 2
        });

        Game? game;
        await using (var session = Store.LightweightSession())
        {
            game = await session.LoadAsync<Game>(gamePrepared.GameId);
            game.Should().NotBeNull();
            game!.Lobby.Teams.Should().HaveCount(2);
            game.Lobby.Teams[0].Should().NotBeNull();
            game.Lobby.Teams[1].Should().NotBeNull();
            game.Lobby.Teams[0].GameSets.Should().HaveCount(1);
            game.Lobby.Teams[1].GameSets.Should().HaveCount(1);
        }

        var gameDuration = TimeSpan.FromSeconds(10);
        await StartGame(game, gameDuration);

        await using (var session = Store.LightweightSession())
        {
            game = await session.LoadAsync<Game>(gamePrepared.GameId);
            game.Should().NotBeNull();
        }

        await DeleteGame(game!);

        await using (var session = Store.LightweightSession())
        {
            game = await session.LoadAsync<Game>(gamePrepared.GameId);
            game.Should().BeNull();
        }
    }

    async Task<GameSetRegistered> RegisterGameSet(Server server)
    {
        var (tracked, scenarioResult) = await TrackedHttpCall(x =>
        {
            x.Post.Url($"/api/lasertag/server/{server.Id}/registerGameSet");
            x.StatusCodeShouldBeOk();
        });

        tracked.Status.Should().Be(TrackingStatus.Completed);
        scenarioResult.Should().NotBeNull();
        var gameSetRegistered = await scenarioResult!.ReadAsJsonAsync<GameSetRegistered>();
        gameSetRegistered.Should().NotBeNull();
        gameSetRegistered!.ServerId.Should().Be(server.Id);

        return gameSetRegistered;
    }

    async Task<GamePrepared> PrepareGame(Server server, LobbyConfiguration lobbyConfiguration)
    {
        var (tracked, _) = await TrackedHttpCall(x =>
        {
            x.Post.Json(lobbyConfiguration).ToUrl($"/api/lasertag/server/{server.Id}/prepareGame");
            x.StatusCodeShouldBeOk();
        });

        var gamePrepared = tracked.Sent.SingleMessage<GamePrepared>();
        return gamePrepared;
    }

    async Task StartGame(Game game, TimeSpan gameDuration)
    {
        var (tracked, _) = await TrackedHttpCall(x =>
        {
            x.Post.Url($"/api/lasertag/game/{game.Id}/start?gameDuration={gameDuration}");
            x.StatusCodeShouldBeOk();
        }, TimeSpan.FromMinutes(3));

        var gameStarted = tracked.Sent.SingleMessage<GameStarted>();
        gameStarted.Should().NotBeNull();

        var envelope = tracked.Sent.SingleEnvelope<GameFinished>();
        envelope.Message.Should().BeAssignableTo<GameFinished>();
        envelope.ScheduleDelayed(gameDuration);
    }

    async Task DeleteGame(Game game)
    {
        await TrackedHttpCall(x =>
        {
            x.Delete.Url($"/api/lasertag/game/{game.Id}");
            x.StatusCodeShouldBeOk();
        });
    }
}