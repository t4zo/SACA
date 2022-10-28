using Microsoft.AspNetCore.Identity;
using SACA.Data;
using SACA.Entities.Identity;
using SACA.i18n;

namespace SACA.Extensions
{
    public static class IdentityExtensions
    {
        public static IServiceCollection AddIdentityCoreConfiguration(this IServiceCollection services)
        {
            var identityBuilder = services.AddDefaultIdentity<ApplicationUser>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 10;
                options.Lockout.AllowedForNewUsers = false;

                options.User.AllowedUserNameCharacters =
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;
            });

            identityBuilder
                .AddSignInManager()
                .AddRoles<ApplicationRole>()
                .AddErrorDescriber<PortugueseIdentityErrorDescriber>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            return services;
        }
    }
}
