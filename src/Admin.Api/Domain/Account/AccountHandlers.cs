using Marten;
using Wolverine.Attributes;

// ReSharper disable UnusedMember.Global

namespace Admin.Api.Domain.Account;

#pragma warning disable S1118

public static class AccountHandlers
{
    public class AccountOverdrawnHandler
    {
        [Transactional]
        public static void Handle(
            AccountEvents.AccountOverdrawn @event,
            ILogger<AccountOverdrawnHandler> logger)
        {
            logger.LogInformation("Account with ID {AccountId} has been overdrawn!", @event.AccountId);
        }
    }

    public class DepositToAccountHandler
    {
        [Transactional]
        public static void Handle(
            AccountCommands.DepositToAccount command,
            Account account,
            IDocumentSession session,
            ILogger<DepositToAccountHandler> logger)
        {
            logger.LogInformation("Depositing money to Account with ID {AccountId}, Amount: {Amount}", account.Id,
                command.Amount);
            account.Balance += command.Amount;
            session.Store(account);
        }
    }

    public class WithdrawFromAccountHandler
    {
        [Transactional]
        public static IEnumerable<object> Handle(
            AccountCommands.WithdrawFromAccount command,
            Account account,
            IDocumentSession session,
            ILogger<WithdrawFromAccountHandler> logger)
        {
            logger.LogInformation("Decreasing balance of Account with ID {AccountId} by {Amount}", account.Id,
                command.Amount);
            account.Balance -= command.Amount;

            // This just marks the account as changed, but
            // doesn't actually commit changes to the database
            // yet. That actually matters as I hopefully explain
            session.Store(account);

            if (account.Balance > 0 && account.Balance < account.MinimumThreshold)
            {
                logger.LogInformation("Minimum Threshold detected on Account with ID {AccountId} and Balance {Balance}",
                    account.Id, account.Balance);
                yield return new AccountEvents.LowBalanceDetected(account.Id);
            }
            else if (account.Balance < 0)
            {
                logger.LogInformation("Account with ID {AccountId} Overdrawn! Balance: {Balance}", account.Id,
                    account.Balance);

                yield return new AccountEvents.AccountOverdrawn(account.Id);

                // Give the customer 10 days to deal with the overdrawn account
                yield return new AccountCommands.EnforceAccountOverdrawnDeadline(account.Id);
            }

            yield return new AccountEvents.AccountUpdated(account.Id, account.Balance);
        }
    }

    public class DummyAccountHandler
    {
        public void Handle(AccountEvents.AccountUpdated _)
        {
            // do nothing!
        }
    }
}

#pragma warning restore S1118