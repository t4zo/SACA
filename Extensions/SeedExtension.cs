using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SACA.Data;
using SACA.Data.Seed.Models;
using SACA.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace SACA.Extensions
{
    public static class SeedExtension
    {
        public static async Task<IApplicationBuilder> SeedDatabase(
            this IApplicationBuilder app,
            IServiceProvider serviceProvider
            )
        {
            var context = serviceProvider.GetRequiredService(typeof(ApplicationDbContext)) as ApplicationDbContext;
            var mapper = serviceProvider.GetRequiredService<IMapper>();
            var imageService = serviceProvider.GetRequiredService<IImageService>();

            await new CategoriesSeed(context).LoadAsync();
            await new ImagesSeed(context, imageService, mapper).LoadAsync();

            return app;
        }
    }
}
