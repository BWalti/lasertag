using JasperFx.Core;
using Wolverine;
using Wolverine.Attributes;

namespace Admin.Api.Domain.Account;

public static class AccountMessages
{
    public record AccountOverdrawn(Guid AccountId) : IAccountCommand;

    [DeliverWithin(5)]
    public record AccountUpdated(Guid AccountId, int Balance) : IAccountCommand;

    public record DebitAccount(Guid AccountId, int Amount) : IAccountCommand;

    public record EnforceAccountOverdrawnDeadline(Guid AccountId) : TimeoutMessage(10.Days()), IAccountCommand;

    public record LowBalanceDetected(Guid AccountId) : IAccountCommand;

    public record WithdrawFromAccount(Guid AccountId, int Amount) : IAccountCommand;

    public record DepositToAccount(Guid AccountId, int Amount) : IAccountCommand;
}