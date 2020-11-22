using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SACA.Data;
using SACA.Models;
using SACA.Models.Identity;
using SACA.Options;
using static SACA.Constants.AuthorizationConstants;

namespace SACA.Extensions
{
    public static class UsersExtensions
    {
        public static async Task<IApplicationBuilder> CreateUsers(this IApplicationBuilder app,
            IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            var appOptions = serviceProvider.GetRequiredService<IOptionsSnapshot<AppOptions>>().Value;

            var hasUsers = await context.Users.AnyAsync();
            if (hasUsers) return app;

            var categories = context.Categories.Include(x => x.ApplicationUsers).ToList();

            foreach (var userOptions in appOptions.Users)
            {
                var user = new ApplicationUser
                    {Email = userOptions.Email, UserName = userOptions.UserName, Categories = new List<Category>()};
                var result = await userManager.CreateAsync(user, userOptions.Password);

                user.Categories = categories;

                if (result.Succeeded)
                    foreach (var role in userOptions.Roles)
                    {
                        await userManager.AddToRoleAsync(user, role);

                        await SeedUserClaims(userManager, user, role);

                        await SeedRoleClaims(roleManager, role);
                    }

                await context.SaveChangesAsync();
            }

            return app;
        }

        private static async Task SeedUserClaims(UserManager<ApplicationUser> userManager, ApplicationUser user,
            string role)
        {
            if (role.Equals(Roles.User))
            {
                await userManager.AddClaimAsync(user,
                    new Claim(CustomClaimTypes.Permissions, Permissions.Categories.View));
                await userManager.AddClaimAsync(user, new Claim(CustomClaimTypes.Permissions, Permissions.Images.View));
                await userManager.AddClaimAsync(user,
                    new Claim(CustomClaimTypes.Permissions, Permissions.Images.Create));
                await userManager.AddClaimAsync(user,
                    new Claim(CustomClaimTypes.Permissions, Permissions.Images.Update));
                await userManager.AddClaimAsync(user,
                    new Claim(CustomClaimTypes.Permissions, Permissions.Images.Delete));
            }
        }

        private static async Task SeedRoleClaims(RoleManager<ApplicationRole> roleManager, string roleName)
        {
            if (roleName.Equals(Roles.User))
            {
                var role = await roleManager.FindByNameAsync(roleName);
                await roleManager.AddClaimAsync(role,
                    new Claim(CustomClaimTypes.Permissions, Permissions.Categories.View));
                await roleManager.AddClaimAsync(role,
                    new Claim(CustomClaimTypes.Permissions, Permissions.Images.View));
                await roleManager.AddClaimAsync(role,
                    new Claim(CustomClaimTypes.Permissions, Permissions.Images.Create));
                await roleManager.AddClaimAsync(role,
                    new Claim(CustomClaimTypes.Permissions, Permissions.Images.Update));
                await roleManager.AddClaimAsync(role,
                    new Claim(CustomClaimTypes.Permissions, Permissions.Images.Delete));
            }
        }
    }
}