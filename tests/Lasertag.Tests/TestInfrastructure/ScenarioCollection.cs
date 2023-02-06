using Xunit;

namespace Lasertag.Tests.TestInfrastructure;

[CollectionDefinition("integration")]
public class ScenarioCollection : ICollectionFixture<AppFixture>
{
}