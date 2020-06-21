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
using System.Threading.Tasks;

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

            var hasUser = await context.Users.AnyAsync();

            if (!hasUser)
            {
                var appConfiguration = configuration.GetSection("AppConfiguration").Get<AppConfiguration>();

                foreach (var _user in appConfiguration.Users)
                {
                    var user = new User { Email = _user.Email, UserName = _user.UserName };
                    var result = await userManager.CreateAsync(user, _user.Password);

                    var categories = context.Categories.ToList();

                    foreach (var category in categories)
                    {
                        await context.AddAsync(new UserCategory { UserId = user.Id, CategoryId = category.Id });
                    }

                    await context.SaveChangesAsync();

                    foreach (var role in _user.Roles)
                    {
                        await userManager.AddToRoleAsync(user, role);
                    };
                };
            }

            return app;
        }
    }
}
