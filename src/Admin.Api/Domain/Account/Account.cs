namespace Admin.Api.Domain.Account;

public class Account
{
    public int Balance { get; set; }
    public int MinimumThreshold { get; init; }
    public Guid Id { get; init; }
}