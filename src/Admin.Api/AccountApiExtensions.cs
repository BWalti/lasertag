using System.Diagnostics;
using Admin.Api.Domain.Account;
using Admin.Api.Extensions;
using Marten;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace Admin.Api;

public static class AccountApiExtensions
{
    public static RouteGroupBuilder MapAccountApi(this RouteGroupBuilder group)
    {
        group.MapGet("/create",
            async (IDocumentSession session, ILogger<Account> logger) =>
            {
                using var outerActivity = OpenTelemetryExtensions.ActivitySource.StartActivity();
                logger.LogInformation("I'm going to create a new account!");

                var account = new Account
                {
                    Balance = 0,
                    MinimumThreshold = 100,
                    Id = Guid.NewGuid()
                };

                session.Store(account);

                using var saveActivity = OpenTelemetryExtensions.ActivitySource.StartActivity(nameof(session.SaveChangesAsync), ActivityKind.Client);
                await session.SaveChangesAsync();
                saveActivity?.Dispose();

                return account;
            });

        group.MapPost("/deposit",
            (IMessageBus bus, [FromBody] AccountCommands.DepositToAccount deposit) => bus.PublishAsync(deposit));

        group.MapPost("/debit",
            (IMessageBus bus, [FromBody] AccountCommands.WithdrawFromAccount withdraw) => bus.PublishAsync(withdraw));

        return group;
    }
}