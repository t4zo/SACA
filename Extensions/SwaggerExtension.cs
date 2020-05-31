//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.AspNetCore.Builder;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.OpenApi.Models;

//namespace SACA.Extensions
//{
//    public static class SwaggerExtension
//    {
//        public static IServiceCollection AddSwagger(this IServiceCollection services)
//        {
//            // Register the Swagger generator, defining 1 or more Swagger documents
//            services.AddSwaggerGen(options =>
//            {
//                options.SwaggerDoc("v1", new OpenApiInfo { Title = "SAED API", Version = "v1" });

//                // Define the BearerAuth scheme that's in use
//                options.AddSecurityDefinition("bearerAuth", new OpenApiSecurityScheme()
//                {
//                    In = ParameterLocation.Header,
//                    Description = "Autenticação baseada em Json Web Token (JWT). Exemplo: \"Bearer {token}\"",
//                    Name = "Authorization",
//                    Type = SecuritySchemeType.ApiKey,
//                    Scheme = JwtBearerDefaults.AuthenticationScheme
//                });
//            });

//            return services;
//        }

//        public static IApplicationBuilder ConfigureSwagger(this IApplicationBuilder app)
//        {
//            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
//            // specifying the Swagger JSON endpoint.
//            app.UseSwaggerUI(c =>
//            {
//                c.SwaggerEndpoint("/swagger/v1/swagger.json", "SAED API V1");
//                c.RoutePrefix = "swagger";
//            });
//            return app;
//        }
//    }
//}
