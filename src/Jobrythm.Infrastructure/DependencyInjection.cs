using Jobrythm.Application.Interfaces;
using Jobrythm.Infrastructure.Options;
using Jobrythm.Infrastructure.Persistence;
using Jobrythm.Infrastructure.Persistence.Repositories;
using Jobrythm.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Jobrythm.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<JobrythmDbContext>(options =>
            options.UseNpgsql(connectionString,
                b => b.MigrationsAssembly(typeof(JobrythmDbContext).Assembly.FullName)));

        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));
        services.Configure<StripeOptions>(configuration.GetSection("Stripe"));
        services.Configure<ResendOptions>(configuration.GetSection("Resend"));
        services.Configure<AppConfig>(configuration.GetSection("AppConfig"));

        services.AddScoped<IJobRepository, JobRepository>();
        services.AddScoped<IClientRepository, ClientRepository>();
        services.AddScoped<IQuoteRepository, QuoteRepository>();
        services.AddScoped<IInvoiceRepository, InvoiceRepository>();
        services.AddScoped<ILineItemRepository, LineItemRepository>();
        services.AddScoped<INumberSequenceRepository, NumberSequenceRepository>();

        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IStripeService, StripeService>();
        services.AddScoped<IPdfService, QuestPdfService>();
        services.AddScoped<IFileStorageService, LocalFileStorageService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<NumberSequenceService>();

        services.AddHttpClient<IEmailService, ResendEmailService>();

        services.AddHttpContextAccessor();

        return services;
    }
}
