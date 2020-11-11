using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;

namespace SACA.Extensions
{
    public static class SwaggerExtensions
    {
        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "SAED API",
                    Description = "Endpoints to v1 SAED API"
                });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Name = HeaderNames.Authorization,
                    Description = @"JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] token",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = JwtBearerDefaults.AuthenticationScheme
                });
            });

            return services;
        }

        public static IApplicationBuilder UseConfiguredSwagger(this IApplicationBuilder app)
        {
            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "SAED API V1");

                // Open Swagger UI on root url
                c.RoutePrefix = "saca/swagger";
            });

            return app;
        }
    }
}
