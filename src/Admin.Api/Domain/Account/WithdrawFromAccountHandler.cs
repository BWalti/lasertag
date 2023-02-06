using Marten;
using Wolverine.Attributes;
// ReSharper disable UnusedMember.Global

namespace Admin.Api.Domain.Account;

#pragma warning disable S1118
public class WithdrawFromAccountHandler
#pragma warning restore S1118
{
    [Transactional]
    public static IEnumerable<object> Handle(
        AccountMessages.WithdrawFromAccount command,
        Account account,
        IDocumentSession session,
        ILogger<WithdrawFromAccountHandler> logger)
    {
        logger.LogInformation("Decreasing balance of Account with ID {AccountId} by {Amount}", account.Id, command.Amount);
        account.Balance -= command.Amount;

        // This just marks the account as changed, but
        // doesn't actually commit changes to the database
        // yet. That actually matters as I hopefully explain
        session.Store(account);

        if (account.Balance > 0 && account.Balance < account.MinimumThreshold)
        {
            logger.LogInformation("Minimum Threshold detected on Account with ID {AccountId} and Balance {Balance}", account.Id, account.Balance);
            yield return new AccountMessages.LowBalanceDetected(account.Id);
        }
        else if (account.Balance < 0)
        {
            logger.LogInformation("Account with ID {AccountId} Overdrawn! Balance: {Balance}", account.Id, account.Balance);

            yield return new AccountMessages.AccountOverdrawn(account.Id);

            // Give the customer 10 days to deal with the overdrawn account
            yield return new AccountMessages.EnforceAccountOverdrawnDeadline(account.Id);
        }

        yield return new AccountMessages.AccountUpdated(account.Id, account.Balance);
    }
}