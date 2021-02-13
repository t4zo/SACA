using AutoMapper;
using FluentValidation.AspNetCore;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SACA.Authorization;
using SACA.Data;
using SACA.Extensions;
using SACA.i18n;
using SACA.Interfaces;
using SACA.Models.Identity;
using SACA.Options;
using SACA.Services;
using System;
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
            services.AddTransient<IUserService, UserService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddTransient<IImageService, ImageService>();

            services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
            services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

            services.AddCustomCors();

            services.AddDbContext<ApplicationDbContext>();

            services.AddOptions<AppOptions>().Bind(Configuration.GetSection(nameof(AppOptions)));
            services.AddOptions<CloudinaryOptions>().Bind(Configuration.GetSection(nameof(CloudinaryOptions)));

            services.AddJwtSecurity();

            services.AddProblemDetails(configure => { configure.IncludeExceptionDetails = (ctx, exp) => true; });

            services.AddIdentityCore<ApplicationUser>(options =>
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
                })
                .AddSignInManager()
                .AddRoles<ApplicationRole>()
                .AddErrorDescriber<PortugueseIdentityErrorDescriber>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

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

            app.UseProblemDetails();

            app.SeedDatabaseAsync().GetAwaiter().GetResult();

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseHttpsRedirection();

            app.UseConfiguredSwagger();

            app.UseRouting();
            app.UseCors();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers().RequireAuthorization(); });
        }
    }
}