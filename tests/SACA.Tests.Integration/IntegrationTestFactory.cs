using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;
using Respawn;
using Respawn.Graph;
using SACA.Data;
using SACA.Data.Seed.Models;
using SACA.Extensions;
using SACA.Interfaces;
using System.Data.Common;

namespace SACA.Tests.Integration;

public class IntegrationTestFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    public HttpClient HttpClient { get; private set; } = default!;
    private DbConnection _dbConnection = default!;
    public Respawner _respawner = default!;
    
    private readonly IContainer _dbContainer;
    private readonly string _connectionString;

    public IntegrationTestFactory()
    {
        var port = OSUtility.NextFreePort();
        
        _dbContainer = new ContainerBuilder()
            .WithImage("postgres:10-alpine")
            .WithEnvironment("POSTGRES_USER", "postgres")
            .WithEnvironment("POSTGRES_PASSWORD", "docker")
            .WithEnvironment("POSTGRES_DB", "saca")
            .WithPortBinding(port, 5432)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(5432))
            .WithCleanUp(true)
            .Build();

        _connectionString = $"Host={_dbContainer.Hostname};Port={port};Database=saca;User ID=postgres;Password=docker";
    }
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            // Remove DbContext
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
            if (descriptor is not null)
            {
                services.Remove(descriptor);
            }

            // Add DbContext pointing to test container
            services.AddDbContext<ApplicationDbContext>(options =>
                options
                    // .UseNpgsql(_dbContainer.ConnectionString, opt => opt.SetPostgresVersion(10, 0))
                    .UseNpgsql(_connectionString, opt => opt.SetPostgresVersion(10, 0))
                    .UseSnakeCaseNamingConvention()
            );
    
            // Ensure schema gets created
            var serviceProvider = services.BuildServiceProvider();
    
            using var scope = serviceProvider.CreateScope();
            var scopedServices = scope.ServiceProvider;
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
    
            try
            {
                context.Database.Migrate();
    
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
        _dbConnection = new NpgsqlConnection(_connectionString);
        HttpClient = CreateClient();
        InitializeRespawner();
    }
    
    public async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
    }
    
    public async Task InitializeRespawner()
    {
        await _dbConnection.OpenAsync();
        _respawner = await Respawner.CreateAsync(_dbConnection,
            new RespawnerOptions
            {
                DbAdapter = DbAdapter.Postgres,
                SchemasToInclude = new[] { "public" },
                TablesToIgnore = new Table[] { "__EFMigrationsHistory" }
            });
    }
    
    public async Task ResetDatabaseAsync()
    {
        await _respawner.ResetAsync(_dbConnection);
    }
}