namespace Admin.Api.Domain.Account;

public interface IAccountCommand
{
    public Guid AccountId { get; }
}