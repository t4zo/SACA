using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SACA.Models.Identity;
using SACA.Options;
using System;
using System.Threading.Tasks;

namespace SACA.Extensions
{
    public static class RolesExtensions
    {
        public static async Task<IApplicationBuilder> CreateRoles(this IApplicationBuilder app, IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            var appOptions = serviceProvider.GetRequiredService<IOptionsSnapshot<AppOptions>>().Value;

            var hasRoles = await roleManager.Roles.AnyAsync();
            if (!hasRoles)
            {
                foreach (var role in appOptions.Roles)
                {
                    var roleExists = await roleManager.RoleExistsAsync(role);
                    if (!roleExists)
                    {
                        await roleManager.CreateAsync(new ApplicationRole {Name = role, NormalizedName = role.ToUpper()});
                    }
                }
            }

            return app;
        }
    }
}