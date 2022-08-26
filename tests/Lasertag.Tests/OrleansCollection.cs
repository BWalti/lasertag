using Xunit;

namespace Lasertag.Tests;

[CollectionDefinition(nameof(OrleansCollection))]
public class OrleansCollection : ICollectionFixture<OrleansFixture>
{
}