namespace HMS.Application.Contracts;

public interface IFeatureEntitlementService
{
    Task<bool> CanCreateBranchAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<bool> CanCreateUserAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<bool> CanCreateDoctorAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<bool> CanCreatePatientAsync(Guid tenantId, CancellationToken cancellationToken = default);
    Task<bool> IsFeatureEnabledAsync(Guid tenantId, string featureCode, CancellationToken cancellationToken = default);
}

