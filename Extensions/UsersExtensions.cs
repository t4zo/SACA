using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SACA.Data;
using SACA.Models;
using SACA.Models.Identity;
using SACA.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static SACA.Constants.AuthorizationConstants;

namespace SACA.Extensions
{
    public static class UsersExtensions
    {
        public static async Task<IApplicationBuilder> CreateUsers(this IApplicationBuilder app, IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            var appConfiguration = serviceProvider.GetRequiredService<IOptionsSnapshot<AppOptions>>().Value;

            var hasUser = await context.Users.AnyAsync();

            if (!hasUser)
            {
                var categories = context.Categories.Include(x => x.ApplicationUsers).ToList();

                foreach (var _user in appConfiguration.Users)
                {
                    var user = new ApplicationUser { Email = _user.Email, UserName = _user.UserName, Categories = new List<Category>() };
                    var result = await userManager.CreateAsync(user, _user.Password);

                    user.Categories = categories;

                    if (result.Succeeded)
                    {
                        foreach (var role in _user.Roles)
                        {
                            await userManager.AddToRoleAsync(user, role);

                            await SeedUserClaims(userManager, user, role);

                            await SeedRoleClaims(roleManager, role);
                        };
                    }

                    await context.SaveChangesAsync();
                };
            }

            return app;
        }

        private static async Task SeedUserClaims(UserManager<ApplicationUser> userManager, ApplicationUser user, string role)
        {
            if (role.Equals(Roles.User))
            {
                await userManager.AddClaimAsync(user, new Claim(CustomClaimTypes.Permissions, Permissions.Categories.View));
                await userManager.AddClaimAsync(user, new Claim(CustomClaimTypes.Permissions, Permissions.Images.View));
                await userManager.AddClaimAsync(user, new Claim(CustomClaimTypes.Permissions, Permissions.Images.Create));
                await userManager.AddClaimAsync(user, new Claim(CustomClaimTypes.Permissions, Permissions.Images.Update));
                await userManager.AddClaimAsync(user, new Claim(CustomClaimTypes.Permissions, Permissions.Images.Delete));
            }
        }

        private static async Task SeedRoleClaims(RoleManager<ApplicationRole> roleManager, string role)
        {
            if (role.Equals(Roles.User))
            {
                var _role = await roleManager.FindByNameAsync(role);
                await roleManager.AddClaimAsync(_role, new Claim(CustomClaimTypes.Permissions, Permissions.Categories.View));
                await roleManager.AddClaimAsync(_role, new Claim(CustomClaimTypes.Permissions, Permissions.Images.View));
                await roleManager.AddClaimAsync(_role, new Claim(CustomClaimTypes.Permissions, Permissions.Images.Create));
                await roleManager.AddClaimAsync(_role, new Claim(CustomClaimTypes.Permissions, Permissions.Images.Update));
                await roleManager.AddClaimAsync(_role, new Claim(CustomClaimTypes.Permissions, Permissions.Images.Delete));
            }
        }
    }
}
