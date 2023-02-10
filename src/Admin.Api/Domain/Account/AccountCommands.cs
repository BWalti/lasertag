using JasperFx.Core;
using Wolverine;

namespace Admin.Api.Domain.Account;

public static class AccountCommands
{
    public interface IAccountCommand
    {
        public Guid AccountId { get; }
    }

    public record WithdrawFromAccount(Guid AccountId, int Amount) : IAccountCommand;

    public record DepositToAccount(Guid AccountId, int Amount) : IAccountCommand;

    public record EnforceAccountOverdrawnDeadline(Guid AccountId) : TimeoutMessage(10.Days()), IAccountCommand;
}