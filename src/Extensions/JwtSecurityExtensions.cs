using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SACA.Options;

namespace SACA.Extensions
{
    public static class JwtSecurityExtensions
    {
        public static IServiceCollection AddJwtSecurity(this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var appOptions = serviceProvider.GetRequiredService<IOptionsSnapshot<AppOptions>>().Value;

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
                        IssuerSigningKey = appOptions.Token.Key,

                        // Validate the JWT Issuer (iss) claim
                        ValidateIssuer = false,
                        ValidIssuer = appOptions.Token.Issuer,

                        // Validate the JWT Audience (aud) claim
                        ValidateAudience = false,
                        ValidAudience = appOptions.Token.Audience,

                        // Validate the token expiry
                        ValidateLifetime = true
                    };
                });

            return services;
        }
    }
}