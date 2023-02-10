using Admin.Api.Domain.Account;
using Alba;
using FluentAssertions;
using Lasertag.Tests.TestInfrastructure;
using Xunit;

namespace Lasertag.Tests;

[Collection("integration")]
public class WhenDebitingAnAccount : IntegrationContext
{
    public WhenDebitingAnAccount(AppFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public async Task should_increase_the_account_balance_happy_path()
    {
        // Drive in a known data, so the "Arrange"
        var account = new Account
        {
            Balance = 2500,
            MinimumThreshold = 200
        };

        await using (var session = Store.LightweightSession())
        {
            session.Store(account);
            await session.SaveChangesAsync();
        }

        // The "Act" part of the test.
        var (tracked, _) = await TrackedHttpCall(x =>
        {
            // Send a JSON post with the DebitAccount command through the HTTP endpoint
            // BUT, it's all running in process
            x.Post.Json(new AccountCommands.WithdrawFromAccount(account.Id, 1200)).ToUrl("/api/accounts/debit");

            // This is the default behavior anyway, but still good to show it here
            x.StatusCodeShouldBeOk();
        });

        // Finally, let's do the "assert"
        await using (var session = Store.LightweightSession())
        {
            // Load the newly persisted copy of the data from Marten
            var persisted = await session.LoadAsync<Account>(account.Id);
            persisted!.Balance.Should().Be(1300); // Started with 2500, debited 1200
        }

        // And also assert that an AccountUpdated message was published as well
        var updated = tracked.Sent.SingleMessage<AccountEvents.AccountUpdated>();
        updated.AccountId.Should().Be(account.Id);
        updated.Balance.Should().Be(1300);
    }
}