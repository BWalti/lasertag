using Xunit;

namespace Lasertag.Grains.Tests;

[CollectionDefinition(nameof(OrleansCollection))]
public class OrleansCollection : ICollectionFixture<OrleansFixture>
{
}