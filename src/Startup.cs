using FluentValidation.AspNetCore;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Authorization;
using SACA.Authorization;
using SACA.Data;
using SACA.Extensions;
using SACA.Interfaces;
using SACA.Middlewares;
using SACA.Options;
using SACA.Repositories;
using SACA.Repositories.Interfaces;
using SACA.Services;
using System.Reflection;

namespace SACA
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IImageRepository, ImageRepository>();
            services.AddTransient<ICategoryRepository, CategoryRepository>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IUnityOfWork, UnityOfWork>();

            services.AddTransient<IUserService, UserService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddTransient<IImageService, ImageService>();
            services.AddTransient<IS3Service, S3Service>();

            services.AddTransient<ExceptionHandlerMiddleware>();

            services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
            services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

            services.AddCustomCors();

            services.AddOptions<AppOptions>().Bind(Configuration.GetSection(nameof(AppOptions)));
            services.AddOptions<AWSOptions>().Bind(Configuration.GetSection(nameof(AWSOptions)));
            services.AddOptions<CloudinaryOptions>().Bind(Configuration.GetSection(nameof(CloudinaryOptions)));

            services.AddJwtSecurity();

            services.AddDbContext<ApplicationDbContext>();

            services.AddProblemDetails(configure => { configure.IncludeExceptionDetails = (ctx, exp) => true; });

            services.AddHealthChecks();

            services.AddIdentityCoreConfiguration();

            services.AddAutoMapper(typeof(Startup));

            services.AddSwagger();

            services.AddRouting(options => options.LowercaseUrls = true);

            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddControllers()
                .AddFluentValidation(configureExpression =>
                {
                    configureExpression.LocalizationEnabled = true;
                    configureExpression.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly());
                });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
            }
            
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            app.UseProblemDetails();

            app.SeedDatabaseAsync().GetAwaiter().GetResult();

            app.UseCustomSwagger();

            app.UseRouting();

            app.UseMiddleware<ExceptionHandlerMiddleware>();
            app.UseCors();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers().RequireAuthorization();
                endpoints.MapHealthChecks("/health");
            });
        }
    }
}