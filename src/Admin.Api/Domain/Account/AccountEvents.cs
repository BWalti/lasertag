using Wolverine.Attributes;

namespace Admin.Api.Domain.Account;

public static class AccountEvents
{
    public interface IAccountEvents
    {
        public Guid AccountId { get; }
    }

    public record AccountOverdrawn(Guid AccountId) : IAccountEvents;

    [DeliverWithin(5)]
    public record AccountUpdated(Guid AccountId, int Balance) : IAccountEvents;

    public record LowBalanceDetected(Guid AccountId) : IAccountEvents;
}