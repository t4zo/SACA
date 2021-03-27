using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SACA.Entities.Identity;
using SACA.Options;
using System;
using System.Threading.Tasks;

namespace SACA.Extensions
{
    public static class RolesExtensions
    {
        public static async Task<IServiceProvider> CreateRolesAsync(this IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            var appOptions = serviceProvider.GetRequiredService<IOptionsSnapshot<AppOptions>>().Value;

            if (!await roleManager.Roles.AnyAsync())
            {
                foreach (var role in appOptions.Roles)
                {
                    var roleExists = await roleManager.RoleExistsAsync(role);
                    if (!roleExists)
                    {
                        await roleManager.CreateAsync(new ApplicationRole { Name = role, NormalizedName = role.ToUpper() });
                    }
                }
            }

            return serviceProvider;
        }
    }
}