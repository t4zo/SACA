using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SACA.Configurations;

namespace SACA.Extensions
{
    public static class JwtSecurityExtensions
    {
        public static IServiceCollection AddJwtSecurity(this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var appConfiguration = serviceProvider.GetRequiredService<IOptionsSnapshot<AppOptions>>().Value;

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
                    ValidateAudience = false,
                    ValidAudience = appConfiguration.Token.Audience,

                    // Validate the token expiry
                    ValidateLifetime = true,
                };
            }).AddCookie(IdentityConstants.ApplicationScheme);

            return services;
        }
    }
}
