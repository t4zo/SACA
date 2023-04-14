#if !DEBUG
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SACA.Data;
using SACA.Data.Seed.Models;
using SACA.Interfaces;

namespace SACA.Extensions
{

    public static class SeedExtensions
    {
        public static async Task<IApplicationBuilder> SeedDatabaseAsync(this IApplicationBuilder app)
        {
            var scope = app.ApplicationServices.CreateScope();

            try
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                await context.Database.MigrateAsync();

                await scope.ServiceProvider.CreateRolesAsync();
                await scope.ServiceProvider.CreateUsersAsync();

                var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
                var s3Service = scope.ServiceProvider.GetRequiredService<IS3Service>();

                await new CategoriesSeed(context).LoadAsync();
                await new ImagesSeed(context, s3Service, mapper).LoadAsync();
            }
            catch (Exception ex)
            {
                var logger = app.ApplicationServices.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred while migrating or initializing the database");
            }


            return app;
        }
    }
}
#endif