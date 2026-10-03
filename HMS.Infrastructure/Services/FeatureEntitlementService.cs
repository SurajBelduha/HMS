using HMS.Application.Contracts;
using HMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HMS.Infrastructure.Services;

public class FeatureEntitlementService : IFeatureEntitlementService
{
    private readonly HmsDbContext _dbContext;

    public FeatureEntitlementService(HmsDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> CanCreateBranchAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var limit = await GetFeatureLimitAsync(tenantId, "MaxBranches", cancellationToken);
        if (limit == -1) return true; // Unlimited

        var currentCount = await _dbContext.Branches.CountAsync(b => b.TenantId == tenantId, cancellationToken);
        return currentCount < limit;
    }

    public async Task<bool> CanCreateUserAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var limit = await GetFeatureLimitAsync(tenantId, "MaxUsers", cancellationToken);
        if (limit == -1) return true;

        var currentCount = await _dbContext.Users.CountAsync(u => u.TenantId == tenantId, cancellationToken);
        return currentCount < limit;
    }

    public async Task<bool> CanCreateDoctorAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var limit = await GetFeatureLimitAsync(tenantId, "MaxDoctors", cancellationToken);
        if (limit == -1) return true;

        var currentCount = await _dbContext.Doctors.CountAsync(d => d.TenantId == tenantId, cancellationToken);
        return currentCount < limit;
    }

    public async Task<bool> CanCreatePatientAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        var limit = await GetFeatureLimitAsync(tenantId, "MaxPatients", cancellationToken);
        if (limit == -1) return true;

        var currentCount = await _dbContext.Patients.CountAsync(p => p.TenantId == tenantId, cancellationToken);
        return currentCount < limit;
    }

    public async Task<bool> IsFeatureEnabledAsync(Guid tenantId, string featureCode, CancellationToken cancellationToken = default)
    {
        var activeSub = await _dbContext.TenantSubscriptions
            .Include(s => s.Plan)
                .ThenInclude(p => p.Features)
            .FirstOrDefaultAsync(s => s.TenantId == tenantId && s.Status == "Active", cancellationToken);

        if (activeSub == null) return true; // Default fallback for development/evaluation

        var feature = activeSub.Plan.Features.FirstOrDefault(f => f.FeatureCode.Equals(featureCode, StringComparison.OrdinalIgnoreCase));
        if (feature == null) return false;

        return feature.LimitType == "Boolean" ? feature.LimitValue.Equals("true", StringComparison.OrdinalIgnoreCase) : true;
    }

    private async Task<int> GetFeatureLimitAsync(Guid tenantId, string featureCode, CancellationToken cancellationToken)
    {
        var activeSub = await _dbContext.TenantSubscriptions
            .Include(s => s.Plan)
                .ThenInclude(p => p.Features)
            .FirstOrDefaultAsync(s => s.TenantId == tenantId && s.Status == "Active", cancellationToken);

        if (activeSub == null) return 100; // Default limit if unconfigured

        var feature = activeSub.Plan.Features.FirstOrDefault(f => f.FeatureCode.Equals(featureCode, StringComparison.OrdinalIgnoreCase));
        if (feature == null) return 100;

        if (feature.LimitType == "Unlimited" || feature.LimitValue == "-1") return -1;
        return int.TryParse(feature.LimitValue, out var val) ? val : 100;
    }
}

