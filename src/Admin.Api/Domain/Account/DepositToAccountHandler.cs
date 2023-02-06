using Marten;
using Wolverine.Attributes;
// ReSharper disable UnusedMember.Global

namespace Admin.Api.Domain.Account;

#pragma warning disable S1118
public class DepositToAccountHandler
#pragma warning restore S1118
{
    [Transactional]
    public static void Handle(
        AccountMessages.DepositToAccount command,
        Account account,
        IDocumentSession session,
        ILogger<DepositToAccountHandler> logger)
    {
        logger.LogInformation("Depositing money to Account with ID {AccountId}, Amount: {Amount}", account.Id, command.Amount);
        account.Balance += command.Amount;
        session.Store(account);
    }
}