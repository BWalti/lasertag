using Admin.Api.Domain.Account;
using FakeItEasy;
using FluentAssertions;
using Marten;
using Wolverine;
using Wolverine.Attributes;
using Xunit;
using Xunit.Abstractions;

namespace Lasertag.Tests;

public class WhenTheAccountIsOverdrawn
{
    static class DebitAccountHandler
    {
        [Transactional]
        public static IEnumerable<object> Handle(
            AccountMessages.DebitAccount command,
            Account account,
            IDocumentSession session)
        {
            account.Balance -= command.Amount;

            // This just marks the account as changed, but
            // doesn't actually commit changes to the database
            // yet. That actually matters as I hopefully explain
            session.Store(account);

            if (account.Balance > 0 && account.Balance < account.MinimumThreshold)
            {
                yield return new AccountMessages.LowBalanceDetected(account.Id);
            }
            else if (account.Balance < 0)
            {
                yield return new AccountMessages.AccountOverdrawn(account.Id);

                // Give the customer 10 days to deal with the overdrawn account
                yield return new AccountMessages.EnforceAccountOverdrawnDeadline(account.Id);
            }
        }
    }

    readonly ITestOutputHelper _output;

    readonly Account _theAccount = new()
    {
        Balance = 1000,
        MinimumThreshold = 100,
        Id = Guid.NewGuid()
    };

    // I happen to like NSubstitute for mocking or dynamic stubs
    readonly IDocumentSession _documentSession = A.Fake<IDocumentSession>();
    readonly object[]? _outboundMessages;

    public WhenTheAccountIsOverdrawn(ITestOutputHelper output)
    {
        _output = output;

        var command = new AccountMessages.DebitAccount(_theAccount.Id, 1200);
        _outboundMessages = DebitAccountHandler.Handle(command, _theAccount, _documentSession).ToArray();
    }

    [Fact]
    public void the_account_balance_should_be_negative()
    {
        _output.WriteLine(nameof(the_account_balance_should_be_negative));

        _theAccount.Balance.Should().Be(-200);
    }

    [Fact]
    public void raises_an_account_overdrawn_message()
    {
        _output.WriteLine(nameof(raises_an_account_overdrawn_message));

        // ShouldHaveMessageOfType() is an extension method in 
        // Wolverine itself to facilitate unit testing assertions like this
        _outboundMessages!.ShouldHaveMessageOfType<AccountMessages.AccountOverdrawn>()
            .AccountId.Should().Be(_theAccount.Id);
    }

    [Fact]
    public void raises_an_overdrawn_deadline_message_in_10_days()
    {
        _output.WriteLine(nameof(raises_an_overdrawn_deadline_message_in_10_days));

        var scheduledTime = _outboundMessages!
            // Also an extension method in Wolverine for testing
            .ShouldHaveMessageOfType<AccountMessages.EnforceAccountOverdrawnDeadline>();

        scheduledTime.DelayTime.Should().BeCloseTo(TimeSpan.FromDays(10), TimeSpan.FromSeconds(10));
    }

    [Fact]
    public void should_not_raise_account_balance_low_event()
    {
        _output.WriteLine(nameof(should_not_raise_account_balance_low_event));

        _outboundMessages!.ShouldHaveNoMessageOfType<AccountMessages.LowBalanceDetected>();
    }
}