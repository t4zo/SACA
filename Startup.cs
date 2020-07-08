using AutoMapper;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SACA.Authorization;
using SACA.Constants;
using SACA.Data;
using SACA.Extensions;
using SACA.i18n;
using SACA.Interfaces;
using SACA.Models;
using SACA.Services;
using SACA.Transactions;
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

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IUserService, UserService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddTransient<IImageService, ImageService>();

            services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
            services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

            services.AddTransient<IUnityOfWork, UnityOfWork>();

            services.AddCustomCors(AuthorizationConstants.DefaultCorsPolicyName);

            services.AddDbContext<ApplicationDbContext>();
            services.AddControllers()
                .AddFluentValidation(configureExpression => configureExpression.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly()));

            services.AddJwtSecurity(Configuration);

            services.AddAuthorization();

            services.AddIdentityCore<User>(options =>
            {
                options.User.AllowedUserNameCharacters = String.Empty;

                options.Password.RequireDigit = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;

                options.SignIn.RequireConfirmedEmail = false;
            })
                .AddSignInManager()
                .AddRoles<IdentityRole<int>>()
                .AddErrorDescriber<PortugueseIdentityErrorDescriber>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddAutoMapper(typeof(Startup));

            services.AddRouting(options =>
            {
                options.LowercaseUrls = true;
            });

            services.AddSwagger();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.IsInDocker(serviceProvider, Configuration);

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.SeedDatabase(serviceProvider).Wait();
            app.CreateRoles(serviceProvider, Configuration).Wait();
            app.CreateUsers(serviceProvider, Configuration).Wait();

            app.UseCors(AuthorizationConstants.DefaultCorsPolicyName);

            app.UseConfiguredSwagger();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers().RequireAuthorization();
            });
        }
    }
}
