using Microsoft.AspNetCore.Mvc.Testing;
using SACA.Interfaces;
// using DotNet.Testcontainers.Builders;
// using DotNet.Testcontainers.Configurations;
// using DotNet.Testcontainers.Containers;
// using Microsoft.AspNetCore.Hosting;
// using Microsoft.AspNetCore.TestHost;
// using Microsoft.Extensions.DependencyInjection;
// using Microsoft.Extensions.DependencyInjection.Extensions;
// using SACA.Database;

namespace SACA.Tests.Integration;

public class AuthApiFactory : WebApplicationFactory<IApiMarker>
    // , IAsyncLifetime
{
    // private readonly PostgreSqlTestcontainer _dbContainer = new TestcontainersBuilder<PostgreSqlTestcontainer>()
    //     .WithDatabase(new PostgreSqlTestcontainerConfiguration
    //     {
    //         Database = "saca",
    //         Username = "postgres",
    //         Password = "docker"
    //     }).Build();
    //
    //
    // protected override void ConfigureWebHost(IWebHostBuilder builder)
    // {
    //     base.ConfigureWebHost(builder);
    //     builder.ConfigureTestServices(services =>
    //     {
    //         services.RemoveAll(typeof(IDbConnectionFactory));
    //         services.AddSingleton<IDbConnectionFactory>(_ => new NpgsqlConnectionFactory(_dbContainer.ConnectionString));
    //     });
    // }
    //
    // public async Task InitializeAsync()
    // {
    //     await _dbContainer.StartAsync();
    // }
    //
    // public async Task DisposeAsync()
    // {
    //     await _dbContainer.StopAsync();
    // }
}