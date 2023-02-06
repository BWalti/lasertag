using Marten;
using Wolverine;
// ReSharper disable UnusedMember.Global

namespace Admin.Api.Domain.Account;

#pragma warning disable S1118
public class AccountLookupMiddleware
#pragma warning restore S1118
{
    // The message *has* to be first in the parameter list
    // Before or BeforeAsync tells Wolverine this method should be called before the actual action
    public static async Task<(HandlerContinuation, Account?)> LoadAsync(
        IAccountCommand command,
        ILogger<AccountLookupMiddleware> logger,
        // This app is using Marten for persistence
        IDocumentSession session,
        CancellationToken cancellation)
    {
        var account = await session.LoadAsync<Account>(command.AccountId, cancellation);
        if (account == null)
        {
            logger.LogWarning("Unable to find an account for {AccountId}, aborting the requested operation", command.AccountId);
        }
        else
        {
            logger.LogInformation("Loaded account for {AccountId}", command.AccountId);
        }

        return (account == null ? HandlerContinuation.Stop : HandlerContinuation.Continue, account);
    }
}