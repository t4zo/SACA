using Microsoft.Extensions.DependencyInjection;

namespace SACA.Extensions
{
    public static class CorsExtension
    {
        public static IServiceCollection AddCustomCors(this IServiceCollection service, string _defaultCorsPolicyName)
        {
            service.AddCors(options =>
            {
                options.AddPolicy(_defaultCorsPolicyName, configurePolicy =>
                {
                    configurePolicy
                        .AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            return service;
        }
    }
}
