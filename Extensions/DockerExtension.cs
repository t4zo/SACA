using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SACA.Data;
using System;

namespace SACA.Extensions
{
    public static class DockerExtension
    {
        public static IApplicationBuilder IsInDocker(this IApplicationBuilder applicationBuilder, IServiceProvider serviceProvider, IConfiguration configuration)
        {
            if (configuration["DOCKER"] == "True")
            {
                var context = serviceProvider.GetService<ApplicationDbContext>();
                context.Database.Migrate();
            }

            return applicationBuilder;
        }
    }
}
