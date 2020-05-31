using SACA.Configurations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace SACA.Extensions
{
    public static class RolesExtension
    {
        public static async Task<IApplicationBuilder> CreateRoles(
            this IApplicationBuilder app,
            IServiceProvider serviceProvider,
            IConfiguration configuration
        )
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();

            if (!roleManager.Roles.AnyAsync().Result)
            {
                var appConfigurations = configuration.GetSection("AppConfiguration").Get<AppConfiguration>();

                foreach (var role in appConfigurations.Roles)
                {
                    var exists = await roleManager.RoleExistsAsync(role);

                    if (!exists)
                    {
                        await roleManager.CreateAsync(new IdentityRole<int> { Name = role, NormalizedName = role.ToUpper() });
                    }
                }
            }

            return app;
        }
    }
}
