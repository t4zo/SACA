
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using SACA.Data;
using SACA.Data.Seed.Models;
using SACA.Database;
using SACA.Extensions;
using SACA.Interfaces;

namespace SACA.Tests.Integration;

public class TestFactory : WebApplicationFactory<Program>
    // , IAsyncLifetime
{
    // // private readonly PostgreSqlTestcontainer _dbContainer = new TestcontainersBuilder<PostgreSqlTestcontainer>()
    // //     .WithDatabase(new PostgreSqlTestcontainerConfiguration
    // //     {
    // //         Database = "saca",
    // //         Username = "postgres",
    // //         Password = "docker"
    // //     }).Build();
    //
    // // private readonly int _port = new Random().Next(10000, 60000);
    // private readonly int _port = 5555;
    // private readonly TestcontainersContainer _dbContainer;
    //
    // public CustomFactory()
    // {
    //     _dbContainer = new TestcontainersBuilder<TestcontainersContainer>()
    //         .WithImage("postgres:latest")
    //         .WithEnvironment("POSTGRES_USER", "postgres")
    //         .WithEnvironment("POSTGRES_PASSWORD", "docker")
    //         .WithEnvironment("POSTGRES_DB", "saca")
    //         .WithPortBinding(_port, 5432)
    //         .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(5432))
    //         .Build();
    // }
    //
    // protected override void ConfigureWebHost(IWebHostBuilder builder)
    // {
    //     builder.ConfigureTestServices(services =>
    //     {
    //         services.RemoveAll(typeof(IDbConnectionFactory));
    //         // services.AddSingleton<IDbConnectionFactory>(_ => new NpgsqlConnectionFactory(_dbContainer.ConnectionString));
    //         services.AddSingleton<IDbConnectionFactory>(_ => new NpgsqlConnectionFactory($"Host=localhost;Port={_port};Database=saca;User ID=postgres;Password=docker"));
    //         
    //         // Remove AppDbContext
    //         // var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
    //         // if (descriptor != null) services.Remove(descriptor);
    //         
    //         // Add DB context pointing to test container
    //         services.AddDbContext<ApplicationDbContext>(options => { options.UseNpgsql($"Host=localhost;Port={_port};Database=saca;User ID=postgres;Password=docker"); });
    //         
    //         // Ensure schema gets created
    //         var serviceProvider = services.BuildServiceProvider();
    //
    //         using var scope = serviceProvider.CreateScope();
    //         var scopedServices = scope.ServiceProvider;
    //         var context = scopedServices.GetRequiredService<ApplicationDbContext>();
    //
    //         try
    //         {
    //             context.Database.MigrateAsync().GetAwaiter().GetResult();
    //
    //             scope.ServiceProvider.CreateRolesAsync().GetAwaiter().GetResult();
    //             scope.ServiceProvider.CreateUsersAsync().GetAwaiter().GetResult();
    //
    //             var s3Service = scope.ServiceProvider.GetRequiredService<IS3Service>();
    //
    //             new CategoriesSeed(context).LoadAsync().GetAwaiter().GetResult();
    //             new ImagesSeed(context, s3Service, new MapperlyMapper()).LoadAsync().GetAwaiter().GetResult();
    //         }
    //         catch (Exception ex)
    //         {
    //             var logger = scopedServices.GetRequiredService<ILogger<Program>>();
    //             logger.LogError(ex, "An error occurred while migrating or initializing the database");
    //         }
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