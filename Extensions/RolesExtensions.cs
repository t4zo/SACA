using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SACA.Options;
using SACA.Models.Identity;
using System;
using System.Threading.Tasks;

namespace SACA.Extensions
{
    public static class RolesExtensions
    {
        public static async Task<IApplicationBuilder> CreateRoles(this IApplicationBuilder app, IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            var appConfiguration = serviceProvider.GetRequiredService<IOptionsSnapshot<AppOptions>>().Value;

            if (!roleManager.Roles.AnyAsync().Result)
            {
                foreach (var role in appConfiguration.Roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                    {
                        await roleManager.CreateAsync(new ApplicationRole { Name = role, NormalizedName = role.ToUpper() });
                    }
                }
            }

            return app;
        }
    }
}
