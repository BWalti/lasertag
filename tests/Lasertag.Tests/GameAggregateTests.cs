using FluentAssertions;
using Lasertag.DomainModel;
using Lasertag.DomainModel.DomainEvents;
using Lasertag.Manager.Game;
using Xunit;

namespace Lasertag.Tests;

public class GameAggregateTests
{
    readonly GameState _testee;

    public GameAggregateTests()
    {
        _testee = new GameState();
    }

    [Fact]
    public void WhenGameServerInitialized_ThenGameIdSet()
    {
        var gameId = Guid.NewGuid();
        var gameServerInitialized = new InfrastructureEvents.GameServerInitialized(gameId);

        _testee.Apply(gameServerInitialized);

        _testee.GameId.Should().Be(gameId);
        _testee.Status.Should().Be(GameStatus.Initialized);
    }
}