namespace HMS.Domain.Contracts;

/// <summary>
/// Indicates that an entity belongs to a specific Tenant (Hospital Organization).
/// Automatically enforced by EF Core Global Query Filters.
/// </summary>
public interface ITenantScoped
{
    public Guid TenantId { get; set; }
}

