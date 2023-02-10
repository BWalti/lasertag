using Admin.Api.Domain.Account;
using FakeItEasy;
using FluentAssertions;
using Marten;
using Microsoft.Extensions.Logging;
using Wolverine;
using Xunit;

namespace Lasertag.Tests;

public class WhenTheAccountIsOverdrawn
{
    readonly Account _theAccount = new()
    {
        Balance = 1000,
        MinimumThreshold = 100,
        Id = Guid.NewGuid()
    };

    readonly IDocumentSession _documentSession = A.Fake<IDocumentSession>();
    readonly object[]? _outboundMessages;

    public WhenTheAccountIsOverdrawn()
    {
        var command = new AccountCommands.WithdrawFromAccount(_theAccount.Id, 1200);
        _outboundMessages = AccountHandlers.WithdrawFromAccountHandler.Handle(command, _theAccount, _documentSession, A.Fake<ILogger<AccountHandlers.WithdrawFromAccountHandler>>()).ToArray();
    }

    [Fact]
    public void the_account_balance_should_be_negative()
    {
        _theAccount.Balance.Should().Be(-200);
    }

    [Fact]
    public void raises_an_account_overdrawn_message()
    {
        // ShouldHaveMessageOfType() is an extension method in 
        // Wolverine itself to facilitate unit testing assertions like this
        _outboundMessages!.ShouldHaveMessageOfType<AccountEvents.AccountOverdrawn>()
            .AccountId.Should().Be(_theAccount.Id);
    }

    [Fact]
    public void raises_an_overdrawn_deadline_message_in_10_days()
    {
        var scheduledTime = _outboundMessages!
            // Also an extension method in Wolverine for testing
            .ShouldHaveMessageOfType<AccountCommands.EnforceAccountOverdrawnDeadline>();

        scheduledTime.DelayTime.Should().BeCloseTo(TimeSpan.FromDays(10), TimeSpan.FromSeconds(10));
    }

    [Fact]
    public void should_not_raise_account_balance_low_event()
    {
        _outboundMessages!.ShouldHaveNoMessageOfType<AccountEvents.LowBalanceDetected>();
    }
}