
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Npgsql;
using Respawn;
using Respawn.Graph;
using SACA.Data;
using SACA.Data.Seed.Models;
using SACA.Database;
using SACA.Extensions;
using SACA.Interfaces;

namespace SACA.Tests.Integration;

public class TestFactory : WebApplicationFactory<Program>
    , IAsyncLifetime
{
    // private static string _unixSocketAddr = "unix:/var/run/docker.sock";
    // private static string dockerEndpoint = Environment.GetEnvironmentVariable("DOCKER_HOST") ?? _unixSocketAddr;
    //
    // private readonly PostgreSqlTestcontainer _dbContainer = new TestcontainersBuilder<PostgreSqlTestcontainer>()
    //     // .WithDockerEndpoint(dockerEndpoint)
    //     .WithDatabase(new PostgreSqlTestcontainerConfiguration
    //     {
    //         Database = "saca",
    //         Username = "postgres",
    //         Password = "docker"
    //     })
    //     .WithImage("postgres:10-alpine")
    //     .WithCleanUp(true)
    //     .Build();
    
    // private readonly int _port = new Random().Next(10000, 60000);
    private readonly int _port = 5555;
    private readonly TestcontainersContainer _dbContainer;
    
    private readonly string connectionString;

    public TestFactory()
    {
        _dbContainer = new TestcontainersBuilder<TestcontainersContainer>()
            .WithImage("postgres:10-alpine")
            .WithEnvironment("POSTGRES_USER", "postgres")
            .WithEnvironment("POSTGRES_PASSWORD", "docker")
            .WithEnvironment("POSTGRES_DB", "saca")
            .WithPortBinding(_port, 5432)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(5432))
            .Build();
    
        connectionString = $"Host={_dbContainer.Hostname};Port={_port};Database=saca;User ID=postgres;Password=docker";
    }
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            // services.RemoveAll(typeof(IDbConnectionFactory));

            // var con = new NpgsqlConnectionFactory(connectionString).CreateConnectionAsync().GetAwaiter().GetResult();

            // services.AddSingleton<IDbConnectionFactory>(_ => new NpgsqlConnectionFactory(_dbContainer.ConnectionString));
            // services.AddSingleton<IDbConnectionFactory>(_ => new NpgsqlConnectionFactory(connectionString));
            
            // Remove AppDbContext
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
            if (descriptor is not null) services.Remove(descriptor);

            // Add DB context pointing to test container
            services.AddDbContext<ApplicationDbContext>(options => { options.UseNpgsql(connectionString); });

            // Ensure schema gets created
            var serviceProvider = services.BuildServiceProvider();
    
            using var scope = serviceProvider.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var context = scopedServices.GetRequiredService<ApplicationDbContext>();
    
            try
            {
                // context.Database.EnsureCreated();
                context.Database.MigrateAsync().GetAwaiter().GetResult();
    
                scope.ServiceProvider.CreateRolesAsync().GetAwaiter().GetResult();
                scope.ServiceProvider.CreateUsersAsync().GetAwaiter().GetResult();
    
                var s3Service = scope.ServiceProvider.GetRequiredService<IS3Service>();
    
                new CategoriesSeed(context).LoadAsync().GetAwaiter().GetResult();
                new ImagesSeed(context, s3Service, new MapperlyMapper()).LoadAsync().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                var logger = scopedServices.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred while migrating or initializing the database");
            }
        });
    }
    
    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
    }
    
    public async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
    }
    
    public async Task ResetDatabase()
    {
        await using var npsqlConnection = new NpgsqlConnection(connectionString);
        await npsqlConnection.OpenAsync();
        var respawner = await Respawner.CreateAsync(npsqlConnection,
            new RespawnerOptions
            {
                DbAdapter = DbAdapter.Postgres,
                SchemasToInclude = new[] { "public" },
                TablesToIgnore = new Table[] { "__EFMigrationsHistory" }
            });
        await respawner.ResetAsync(npsqlConnection);
    }
}