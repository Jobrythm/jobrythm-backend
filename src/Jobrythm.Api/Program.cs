using System.Text;
using System.Text.Json.Serialization;
using FluentValidation;
using Microsoft.AspNetCore.RateLimiting;
using Jobrythm.Api.Middleware;
using Jobrythm.Application.Behaviors;
using Jobrythm.Domain.Entities;
using Jobrythm.Infrastructure;
using Jobrythm.Infrastructure.Options;
using Jobrythm.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Infrastructure
builder.Services.AddInfrastructure(builder.Configuration);

// Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequireUppercase = true;
        options.Password.RequiredLength = 8;
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<JobrythmDbContext>()
    .AddDefaultTokenProviders();

var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>() 
    ?? throw new InvalidOperationException("JwtSettings is missing");

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuers = jwtSettings.ValidIssuers,
            ValidAudiences = jwtSettings.ValidAudiences,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret))
        };
    });

// MediatR & Behaviors
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Jobrythm.Application.IApplicationMarker).Assembly);
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
});

// AutoMapper & FluentValidation
builder.Services.AddAutoMapper(cfg => cfg.AddMaps(typeof(Jobrythm.Application.IApplicationMarker).Assembly));
builder.Services.AddValidatorsFromAssembly(typeof(Jobrythm.Application.IApplicationMarker).Assembly);

// CORS
var appConfig = builder.Configuration.GetSection("AppConfig").Get<AppConfig>()
    ?? throw new InvalidOperationException("AppConfig is missing");

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy.WithOrigins(appConfig.ClientUrls)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Scalar / OpenAPI
builder.Services.AddOpenApi();

// Controllers
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });

builder.Services.AddMemoryCache();

// Rate Limiting
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("auth", opt =>
    {
        opt.PermitLimit = 10;
        opt.Window = TimeSpan.FromMinutes(1);
        opt.QueueLimit = 0;
    });
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

var app = builder.Build();

// Migrations and seed
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<JobrythmDbContext>();
    await db.Database.MigrateAsync();

    // Seed default admin user
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    const string adminEmail = "admin@example.com";
    if (await userManager.FindByEmailAsync(adminEmail) == null)
    {
        var admin = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            FullName = "Admin",
            EmailConfirmed = true
        };
        // Use PasswordHasher directly so the seed password bypasses complexity validators
        admin.PasswordHash = new PasswordHasher<ApplicationUser>()
            .HashPassword(admin, "adminpassword");
        await userManager.CreateAsync(admin);
    }
}

// Middleware
app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
});

app.UseExceptionHandler(errorApp => errorApp.Run(GlobalExceptionHandler.Handle));

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.Title = "Jobrythm API";
    });
}

app.UseStaticFiles();
app.UseCors("Frontend");
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapGet("/", () => Results.Content(@"
<html>
<head>
    <title>Jobrythm API Status</title>
    <style>
        body { font-family: sans-serif; display: flex; justify-content: center; align-items: center; height: 100vh; margin: 0; background-color: #f0f2f5; }
        .card { background: white; padding: 2rem; border-radius: 8px; box-shadow: 0 4px 6px rgba(0,0,0,0.1); text-align: center; }
        .status-up { color: #28a745; font-weight: bold; }
        h1 { margin-top: 0; }
    </style>
</head>
<body>
    <div class='card'>
        <h1>Jobrythm API</h1>
        <p>Status: <span class='status-up'>Online</span></p>
        <p>Version: 1.0.0</p>
        <p>Environment: " + app.Environment.EnvironmentName + @"</p>
        <p><a href='/scalar/v1'>API Documentation</a></p>
    </div>
</body>
</html>", "text/html"));

try
{
    Log.Information("Starting Jobrythm API...");
    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
