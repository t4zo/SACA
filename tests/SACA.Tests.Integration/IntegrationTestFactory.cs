using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Respawn;
using Respawn.Graph;
using SACA.Data;
using SACA.Data.Seed;
using SACA.Data.Seed.Models;
using SACA.Extensions;
using SACA.Interfaces;
using SACA.Utilities;

namespace SACA.Tests.Integration;

public class IntegrationTestFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    public HttpClient HttpClient { get; private set; }
    private Respawner Respawner { get; set; }
    
    private IServiceProvider ServiceProvider { get; set; }
    private ApplicationDbContext Context { get; set; }
    
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
        // _connectionString = $"Host=localhost;Port=5432;Database=saca;User ID=postgres;Password=docker";
    }
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            // Remove current DbContext
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
            if (descriptor is not null)
            {
                services.Remove(descriptor);
            }

            // Add DbContext pointing to test container
            services.AddDbContext<ApplicationDbContext>(options =>
                options
                    .UseNpgsql(_connectionString, opt => opt.SetPostgresVersion(10, 0))
                    .UseSnakeCaseNamingConvention()
            );
    
            ServiceProvider = services.BuildServiceProvider();
            Context = ServiceProvider.GetRequiredService<ApplicationDbContext>();

            try
            {
                // Migrate and populate database
                Context.Database.Migrate();
                ServiceProvider.CreateRolesAsync().GetAwaiter().GetResult();
                ServiceProvider.CreateUsersAsync().GetAwaiter().GetResult();
                
                ReseedTestDatabase();
            }
            catch (Exception ex)
            {
                var logger = ServiceProvider.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred while migrating or initializing the database");
            }
        });
    }

    private void ReseedTestDatabase()
    {
        Context.ChangeTracker.Clear();

        var s3Service = ServiceProvider.GetRequiredService<IS3Service>();
        
        new CategoriesSeed(Context).LoadAsync().GetAwaiter().GetResult();
        new ImagesSeed(Context, s3Service, new MapperlyMapper()).LoadAsync(new LoadAsyncOptions { UploadImage = false }).GetAwaiter().GetResult();
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();
        HttpClient = CreateClient();
        await InitializeRespawner();
    }
    
    public new async Task DisposeAsync()
    {
        await _dbContainer.StopAsync();
    }

    private async Task InitializeRespawner()
    {
        var dbConnection = Context.Database.GetDbConnection();
        await dbConnection.OpenAsync();
        Respawner = await Respawner.CreateAsync(dbConnection,
            new RespawnerOptions
            {
                DbAdapter = DbAdapter.Postgres,
                SchemasToInclude = new[] { "public" },
                TablesToIgnore = new Table[] { "__EFMigrationsHistory", "AspNetUsers", "AspNetRoles", "AspNetUserRoles" }
            });
    }
    
    public async Task ResetDatabaseAsync()
    {
        await Respawner.ResetAsync(Context.Database.GetDbConnection());
        ReseedTestDatabase();
    }
}