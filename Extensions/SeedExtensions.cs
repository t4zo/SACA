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
        public static async Task<IApplicationBuilder> SeedDatabase(this IApplicationBuilder app, IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var mapper = serviceProvider.GetRequiredService<IMapper>();
            var imageService = serviceProvider.GetRequiredService<IImageService>();

            //var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            //var databaseProvider = configuration[DatabaseConstants.Database];

            await new CategoriesSeed(context).LoadAsync();
            await new ImagesSeed(context, imageService, mapper).LoadAsync();

            return app;
        }
    }
}
