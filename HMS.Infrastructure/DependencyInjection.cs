using System.Text;
using Hangfire;
using Hangfire.MemoryStorage;
using HMS.Application.Contracts;
using HMS.Domain.Contracts;
using HMS.Infrastructure.Persistence;
using HMS.Infrastructure.Persistence.Repositories;
using HMS.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace HMS.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // DbContext Registration
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Server=localhost;Database=HMS;Trusted_Connection=True;TrustServerCertificate=True;";

        services.AddDbContext<HmsDbContext>(options =>
            options.UseSqlServer(connectionString));

        // Repositories & Dynamic Factory
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddTransient<ITenantDbContextFactory<HmsDbContext>, TenantDbContextFactory>();

        // Custom Application & Infrastructure Services
        services.AddScoped<ICurrentTenantService, CurrentTenantService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IJwtTokenService, JwtTokenService>();
        services.AddScoped<IAuditService, AuditService>();
        services.AddScoped<ISequenceGeneratorService, SequenceGeneratorService>();
        services.AddScoped<IFeatureEntitlementService, FeatureEntitlementService>();
        services.AddScoped<IPatientService, PatientService>();
        services.AddScoped<IDoctorService, DoctorService>();
        services.AddScoped<IAppointmentService, AppointmentService>();
        services.AddScoped<IClinicalService, ClinicalService>();
        services.AddScoped<IBillingService, BillingService>();
        services.AddSingleton<IRedisCacheService, RedisCacheService>();

        // Distributed Cache (Redis / Memory fallback)
        var redisConnection = configuration.GetConnectionString("Redis");
        if (!string.IsNullOrEmpty(redisConnection))
        {
            services.AddStackExchangeRedisCache(options => options.Configuration = redisConnection);
        }
        else
        {
            services.AddDistributedMemoryCache();
        }

        // JWT Authentication
        var secretKey = configuration["Jwt:SecretKey"] ?? "SUPER_SECRET_KEY_FOR_HMS_SAAS_MULTITENANT_PRODUCT_2026";
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration["Jwt:Issuer"] ?? "HMS.Api",
                ValidAudience = configuration["Jwt:Audience"] ?? "HMS.Client",
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
            };
        });

        // Hangfire Background Job Storage
        services.AddHangfire(config => config.UseMemoryStorage());
        services.AddHangfireServer();

        return services;
    }
}

