using Marten;

namespace Lasertag.Tests.TestInfrastructure;

public static class DocumentStoreExtensions
{
    public static Task BulkInsertAsync<T>(this IDocumentStore store, IEnumerable<T> items) =>
        store.BulkInsertAsync(items.ToArray());
}