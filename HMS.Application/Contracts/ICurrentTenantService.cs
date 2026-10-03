using HMS.Common.Enums;

namespace HMS.Application.Contracts;

public interface ICurrentTenantService
{
    public Guid TenantId { get; }
    public Guid? BranchId { get; }
    public Guid? UserId { get; }
    public string? Role { get; }
    public bool IsHostAdmin { get; }
    public TenantIsolationStrategy IsolationStrategy { get; }
    public string? DedicatedConnectionString { get; }

    public void SetTenantContext(
        Guid tenantId,
        Guid? branchId,
        Guid? userId = null,
        string? role = null,
        bool isHostAdmin = false,
        TenantIsolationStrategy isolationStrategy = TenantIsolationStrategy.SharedDatabase,
        string? dedicatedConnectionString = null);
}

