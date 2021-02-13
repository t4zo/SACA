using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SACA.Data;
using SACA.Data.Seed.Models;
using SACA.Interfaces;
using System;
using System.Threading.Tasks;

namespace SACA.Extensions
{
    public static class SeedExtensions
    {
        public static async Task<IApplicationBuilder> SeedDatabaseAsync(this IApplicationBuilder app)
        {
            var serviceScope = app.ApplicationServices.CreateScope();
            var context = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            await serviceScope.ServiceProvider.CreateRolesAsync();
            await serviceScope.ServiceProvider.CreateUsersAsync();
            
            var mapper = serviceScope.ServiceProvider.GetRequiredService<IMapper>();
            var imageService = serviceScope.ServiceProvider.GetRequiredService<IImageService>();

            await new CategoriesSeed(context).LoadAsync();
            await new ImagesSeed(context, imageService, mapper).LoadAsync();

            return app;
        }
    }
}