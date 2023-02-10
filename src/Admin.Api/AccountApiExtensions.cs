using Admin.Api.Domain.Account;
using Marten;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace Admin.Api;

public static class AccountApiExtensions
{
    public static RouteGroupBuilder MapAccountApi(this RouteGroupBuilder group)
    {
        group.MapGet("/create",
            async (IDocumentSession session) =>
            {
                var account = new Account
                {
                    Balance = 0,
                    MinimumThreshold = 100,
                    Id = Guid.NewGuid()
                };

                session.Store(account);
                await session.SaveChangesAsync();

                return account;
            });

        group.MapPost("/deposit",
            (IMessageBus bus, [FromBody] AccountCommands.DepositToAccount deposit) => bus.PublishAsync(deposit));

        group.MapPost("/debit",
            (IMessageBus bus, [FromBody] AccountCommands.WithdrawFromAccount withdraw) => bus.PublishAsync(withdraw));

        return group;
    }
}