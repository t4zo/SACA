using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SACA.Configurations;
using System.Text;

namespace SACA.Extensions
{
    public static class JwtSecurityExtension
    {
        public static IServiceCollection AddJwtSecurity(
            this IServiceCollection services,
            IConfiguration configuration
            )
        {
            // configure strongly typed settings objects
            var appConfigurationSection = configuration.GetSection("AppConfiguration");
            services.Configure<AppConfiguration>("AppConfiguration", appConfigurationSection);

            // configure jwt authentication
            var appConfiguration = appConfigurationSection.Get<AppConfiguration>();
            var key = Encoding.ASCII.GetBytes(appConfiguration.Token.SecurityKey);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    // The signing key must match!
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = appConfiguration.Token.Key,

                    // Validate the JWT Issuer (iss) claim
                    ValidateIssuer = true,
                    ValidIssuer = appConfiguration.Token.Issuer,

                    // Validate the JWT Audience (aud) claim
                    ValidateAudience = true,
                    ValidAudience = appConfiguration.Token.Audience,

                    // Validate the token expiry
                    ValidateLifetime = true,
                };
            }).AddCookie(IdentityConstants.ApplicationScheme);

            return services;
        }
    }
}
