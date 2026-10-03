using HMS.Application.Contracts;
using HMS.Common.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace HMS.Infrastructure.Persistence;

public class TenantDbContextFactory : ITenantDbContextFactory<HmsDbContext>
{
    private readonly ICurrentTenantService _tenantService;
    private readonly IConfiguration _configuration;

    public TenantDbContextFactory(ICurrentTenantService tenantService, IConfiguration configuration)
    {
        _tenantService = tenantService;
        _configuration = configuration;
    }

    public HmsDbContext CreateDbContext()
    {
        var optionsBuilder = new DbContextOptionsBuilder<HmsDbContext>();

        string connectionString;
        if (_tenantService.IsolationStrategy == TenantIsolationStrategy.DedicatedDatabase
            && !string.IsNullOrWhiteSpace(_tenantService.DedicatedConnectionString))
        {
            connectionString = _tenantService.DedicatedConnectionString;
        }
        else
        {
            connectionString = _configuration.GetConnectionString("DefaultConnection")
                ?? "Server=localhost;Database=HMS;Trusted_Connection=True;TrustServerCertificate=True;";
        }

        optionsBuilder.UseSqlServer(connectionString);
        return new HmsDbContext(optionsBuilder.Options, _tenantService);
    }
}

