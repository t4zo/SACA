using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SACA.Data;
using SACA.Entities.Identity;
using SACA.Options;
using System.Security.Claims;
using static SACA.Constants.AuthorizationConstants;

namespace SACA.Extensions
{
    public static class UsersExtensions
    {
        public static int? GetId(this ClaimsPrincipal claimsPrincipal)
        {
            var userId = claimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
            {
                return null;
            }

            return int.Parse(userId);
        }

        public static async Task<IServiceProvider> CreateUsersAsync(this IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            var appOptions = serviceProvider.GetRequiredService<IOptionsSnapshot<AppOptions>>().Value;

            if (!await context.Users.AnyAsync())
            {
                foreach (var userOptions in appOptions.Users)
                {
                    var user = new ApplicationUser { Email = userOptions.Email, UserName = userOptions.UserName };
                    var result = await userManager.CreateAsync(user, userOptions.Password);

                    // Seeding M2M Table
                    user.Categories = await context.Categories.ToListAsync();

                    if (result.Succeeded)
                    {
                        foreach (var role in userOptions.Roles)
                        {
                            await userManager.AddToRoleAsync(user, role);
                            await SeedUserClaims(userManager, user, role);
                            await SeedRoleClaims(roleManager, role);
                        }
                    }

                    await context.SaveChangesAsync();
                }
            }

            return serviceProvider;
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

        private static async Task SeedRoleClaims(RoleManager<ApplicationRole> roleManager, string roleName)
        {
            if (roleName.Equals(Roles.User))
            {
                var role = await roleManager.FindByNameAsync(roleName);
                await roleManager.AddClaimAsync(role, new Claim(CustomClaimTypes.Permissions, Permissions.Categories.View));
                await roleManager.AddClaimAsync(role, new Claim(CustomClaimTypes.Permissions, Permissions.Images.View));
                await roleManager.AddClaimAsync(role, new Claim(CustomClaimTypes.Permissions, Permissions.Images.Create));
                await roleManager.AddClaimAsync(role, new Claim(CustomClaimTypes.Permissions, Permissions.Images.Update));
                await roleManager.AddClaimAsync(role, new Claim(CustomClaimTypes.Permissions, Permissions.Images.Delete));
            }
        }
    }
}