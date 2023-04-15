using FluentValidation.AspNetCore;
using Hellang.Middleware.ProblemDetails;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using SACA;
using SACA.Authorization;
using SACA.Data;
using SACA.Database;
using SACA.Extensions;
using SACA.Interfaces;
using SACA.Middlewares;
using SACA.Options;
using SACA.Repositories;
using SACA.Repositories.Interfaces;
using SACA.Services;
using System.Reflection;

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ApplicationDbContext>(options => options
    .UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
    .UseSnakeCaseNamingConvention());

// builder.Services.AddSingleton<IDbConnectionFactory, NpgsqlConnectionFactory>();

builder.Services.AddScoped<MapperlyMapper>();

builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IImageRepository, ImageRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUnityOfWork, UnityOfWork>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<IS3Service, S3Service>();

builder.Services.AddTransient<ExceptionHandlerMiddleware>();

builder.Services.AddIdentityCoreConfiguration();
builder.Services.AddDataProtection();

builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

builder.Services.AddCustomCors();

builder.Services.AddOptions<AppOptions>().Bind(builder.Configuration.GetSection(nameof(AppOptions)));
builder.Services.AddOptions<AWSOptions>().Bind(builder.Configuration.GetSection(nameof(AWSOptions)));
builder.Services.AddOptions<CloudinaryOptions>().Bind(builder.Configuration.GetSection(nameof(CloudinaryOptions)));

builder.Services.AddJwtSecurity();

builder.Services.AddProblemDetails(configure => { configure.IncludeExceptionDetails = (_, _) => true; });

builder.Services.AddHealthChecks();

builder.Services.AddSwagger();

builder.Services.AddRouting(options => options.LowercaseUrls = true);

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddHttpContextAccessor();

builder.Services.AddControllers()
    .AddFluentValidation(configureExpression =>
    {
        configureExpression.LocalizationEnabled = true;
        configureExpression.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly());
    });

var app = builder.Build();

#if !DEBUG
app.SeedDatabaseAsync().GetAwaiter().GetResult();
#endif

if (app.Environment.IsDevelopment())
{
    app.UseCustomSwagger();

    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();
}

app.UseProblemDetails();

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

app.Run();