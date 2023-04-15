using DotNet.Testcontainers.Builders;
using Microsoft.AspNetCore.Mvc.Testing;
using SACA.Interfaces;

namespace SACA.Tests.Integration;

public class ImageApiFactory : WebApplicationFactory<IApiMarker>
    // , IAsyncLifetime
{
    // private readonly PostgreSqlTestContainer _dbContainer = new TestcontainersBuilder<PostgreSqlTestContainer>()
    //     .WithDatabase(new PostgreSqlTestContainerConfiguration
    //     {
    //         Database = "saca",
    //         Username = "postgres",
    //         password = "docker"
    //     }).Build();
    //
    // public Task InitializeAsync()
    // {
    //     throw new NotImplementedException();
    // }
    //
    // public Task DisposeAsync()
    // {
    //     throw new NotImplementedException();
    // }
}