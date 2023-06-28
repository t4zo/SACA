using Xunit;

namespace SACA.Tests.Integration;

public static class IntegrationTestCollectionConstants
{
    public const string CollectionDefinitionName = "Integration test collection";
}

[CollectionDefinition(IntegrationTestCollectionConstants.CollectionDefinitionName)]
public class IntegrationTestCollection : ICollectionFixture<IntegrationTestFactory>
{
    
}