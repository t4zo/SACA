using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SACA.Data;
using SACA.Extensions;
using SACA.i18n;
using SACA.Models;
using SACA.Repositories;
using SACA.Repositories.Interfaces;
using SACA.Services;
using SACA.Services.Interfaces;
using SACA.Transactions;
using SACA.Utilities;
using System;

namespace SACA
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        private const string _defaultCorsPolicyName = "localhost";

        //private string _connection = null;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //var builder = new SqlConnectionStringBuilder(Configuration.GetConnectionString("ConnectionStrings__DefaultConnection"));
            //builder.ConnectionString = Configuration["ConnectionStrings__DefaultConnection"];

            //_connection = builder.ConnectionString;

            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IImageService, ImageService>();

            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<ICategoryRepository, CategoryRepository>();
            services.AddTransient<IUserCategoryRepository, UserCategoryRepository>();
            services.AddTransient<IImageRepository, ImageRepository>();

            services.AddTransient<IUnityOfWork, UnityOfWork>();

            services.AddCustomCors(_defaultCorsPolicyName);

            services.AddDbContext<ApplicationDbContext>();
            services.AddControllers();

            services.AddJwtSecurity(Configuration);

            services.AddAuthorization(options =>
            {
                options.AddPolicy(Constants.All, policy => policy.RequireRole(Constants.AllRoles));
                options.AddPolicy(Constants.Administrador, policy => policy.RequireRole(Constants.Administrador));
                options.AddPolicy(Constants.Usuario, policy => policy.RequireRole(Constants.Usuario));
            });

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

            //services.AddSwagger();
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

            app.UseCors(_defaultCorsPolicyName);

            //app.Run(async (context) =>
            //{
            //    await context.Response.WriteAsync($"ConnectionString: {_connection}");
            //});

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            //app.UseSwagger();
            //app.ConfigureSwagger();

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
