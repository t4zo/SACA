using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SACA.Entities;
using SACA.Entities.Identity;
using System.Reflection;

namespace SACA.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, int>
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<Image> Images { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            
            var entities = builder.Model.GetEntityTypes();

            foreach (var entity in entities)
            {
                // entity.FindProperty("Id")?.SetIdentityStartValue(1000);
            }

            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        //private void UseHiLoStartingSequence(ModelBuilder modelBuilder)
        //{
        //    modelBuilder.HasSequence<int>("DbHiLoSequence").StartsAt(1000).IncrementsBy(1);
        //    NpgsqlModelBuilderExtensions.UseHiLo(modelBuilder, "DbHiLoSequence");
        //}
    }
}