using HMS.Application.Contracts;
using HMS.Common.Enums;

namespace HMS.Infrastructure.Services;

public class CurrentTenantService : ICurrentTenantService
{
    public Guid TenantId { get; private set; }
    public Guid? BranchId { get; private set; }
    public Guid? UserId { get; private set; }
    public string? Role { get; private set; }
    public bool IsHostAdmin { get; private set; }
    public TenantIsolationStrategy IsolationStrategy { get; private set; } = TenantIsolationStrategy.SharedDatabase;
    public string? DedicatedConnectionString { get; private set; }

    public void SetTenantContext(
        Guid tenantId,
        Guid? branchId,
        Guid? userId = null,
        string? role = null,
        bool isHostAdmin = false,
        TenantIsolationStrategy isolationStrategy = TenantIsolationStrategy.SharedDatabase,
        string? dedicatedConnectionString = null)
    {
        TenantId = tenantId;
        BranchId = branchId;
        UserId = userId;
        Role = role;
        IsHostAdmin = isHostAdmin;
        IsolationStrategy = isolationStrategy;
        DedicatedConnectionString = dedicatedConnectionString;
    }
}

