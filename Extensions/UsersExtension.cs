using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SACA.Configurations;
using SACA.Data;
using SACA.Models;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static SACA.Constants.AuthorizationConstants;

namespace SACA.Extensions
{
    public static class UsersExtension
    {
        public static async Task<IApplicationBuilder> CreateUsers(
            this IApplicationBuilder app,
            IServiceProvider serviceProvider,
            IConfiguration configuration
            )
        {
            var context = serviceProvider.GetRequiredService(typeof(ApplicationDbContext)) as ApplicationDbContext;
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();

            var hasUser = await context.Users.AnyAsync();

            if (!hasUser)
            {
                var appConfiguration = configuration.GetSection("AppConfiguration").Get<AppConfiguration>();

                foreach (var _user in appConfiguration.Users)
                {
                    var user = new User { Email = _user.Email, UserName = _user.UserName };
                    var result = await userManager.CreateAsync(user, _user.Password);

                    if (result.Succeeded)
                    {
                        foreach (var role in _user.Roles)
                        {
                            await userManager.AddToRoleAsync(user, role);

                            await SeedUserClaims(userManager, user, role);

                            await SeedRoleClaims(roleManager, role);
                        };

                        await AddUserCategory(context, user);

                        await context.SaveChangesAsync();
                    }
                };
            }

            return app;
        }

        private static async Task AddUserCategory(ApplicationDbContext context, User user)
        {
            var categories = context.Categories.ToList();
            foreach (var category in categories)
            {
                await context.AddAsync(new UserCategory { UserId = user.Id, CategoryId = category.Id });
            }
        }

        private static async Task SeedUserClaims(UserManager<User> userManager, User user, string role)
        {
            if (role.Equals(Roles.User))
            {
                await userManager.AddClaimAsync(user, new Claim(CustomClaimTypes.Permission, Permissions.Categories.View));
                await userManager.AddClaimAsync(user, new Claim(CustomClaimTypes.Permission, Permissions.Images.View));
                await userManager.AddClaimAsync(user, new Claim(CustomClaimTypes.Permission, Permissions.Images.Create));
                await userManager.AddClaimAsync(user, new Claim(CustomClaimTypes.Permission, Permissions.Images.Update));
                await userManager.AddClaimAsync(user, new Claim(CustomClaimTypes.Permission, Permissions.Images.Delete));
            }
        }

        private static async Task SeedRoleClaims(RoleManager<IdentityRole<int>> roleManager, string role)
        {
            if (role.Equals(Roles.User))
            {
                IdentityRole<int> _role = await roleManager.FindByNameAsync(role);
                await roleManager.AddClaimAsync(_role, new Claim(CustomClaimTypes.Permission, Permissions.Categories.View));
                await roleManager.AddClaimAsync(_role, new Claim(CustomClaimTypes.Permission, Permissions.Images.View));
                await roleManager.AddClaimAsync(_role, new Claim(CustomClaimTypes.Permission, Permissions.Images.Create));
                await roleManager.AddClaimAsync(_role, new Claim(CustomClaimTypes.Permission, Permissions.Images.Update));
                await roleManager.AddClaimAsync(_role, new Claim(CustomClaimTypes.Permission, Permissions.Images.Delete));
            }
        }
    }
}
