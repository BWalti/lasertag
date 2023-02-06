using Admin.Api.Domain.Account;
using Marten;
using Marten.Schema;

namespace Lasertag.Tests.TestInfrastructure;

class InitialAccountData : IInitialData
{
    public static Guid Account1 = Guid.NewGuid();
    public static Guid Account2 = Guid.NewGuid();
    public static Guid Account3 = Guid.NewGuid();

    public Task Populate(IDocumentStore store, CancellationToken cancellation) =>
        store.BulkInsertAsync(Accounts());

    IEnumerable<Account> Accounts()
    {
        yield return new Account
        {
            Id = Account1,
            Balance = 1000,
            MinimumThreshold = 500
        };

        yield return new Account
        {
            Id = Account2,
            Balance = 1200
        };

        yield return new Account
        {
            Id = Account3,
            Balance = 2500,
            MinimumThreshold = 100
        };
    }
}