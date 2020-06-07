using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SACA.Models;
using System.Reflection;

namespace SACA.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        private readonly IConfiguration _configuration;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IConfiguration configuration)
            : base(options)
        {
            _configuration = configuration;
        }

        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<UserCategory> UserCategories { get; set; }
        public virtual DbSet<Image> Images { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            
            string connectionString = GetConnectionEnvironmentString();

            optionsBuilder
                .UseNpgsql(connectionString)
                .UseSnakeCaseNamingConvention();
        }

        private string GetConnectionEnvironmentString()
        {
            var env = _configuration["ASPNETCORE_ENVIRONMENT"];

            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            if (env == "Heroku")
            {
                var connUrl = _configuration["DATABASE_URL"];

                // Parse connection URL to connection string for Npgsql
                connUrl = connUrl.Replace("postgres://", string.Empty);

                var pgUserPass = connUrl.Split("@")[0];
                var pgHostPortDb = connUrl.Split("@")[1];
                var pgHostPort = pgHostPortDb.Split("/")[0];

                var pgHost = pgHostPort.Split(":")[0];
                var pgDb = pgHostPortDb.Split("/")[1];
                var pgUser = pgUserPass.Split(":")[0];
                var pgPort = pgHostPort.Split(":")[1];
                var pgPass = pgUserPass.Split(":")[1];

                connectionString = $"Host={pgHost};Database={pgDb};User Id={pgUser};Port={pgPort};Password={pgPass}";
            }

            return connectionString;
        }

        private void UseHiLoStartingSequence(ModelBuilder modelBuilder)
        {
            modelBuilder.HasSequence<int>("DbHiLoSequence").StartsAt(1000).IncrementsBy(1);
            NpgsqlModelBuilderExtensions.UseHiLo(modelBuilder, "DbHiLoSequence");
        }
    }
}
