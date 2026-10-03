using System.ComponentModel.DataAnnotations.Schema;
using HMS.Common.Enums;
using HMS.Domain.Contracts;

namespace HMS.Domain.Entities;

public class Tenant : IAuditableEntity, ISoftDelete
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string TenantCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string LegalName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string TimeZone { get; set; } = "UTC";
    public string Currency { get; set; } = "USD";
    public string Country { get; set; } = "USA";
    public string Status { get; set; } = "Active"; // Active, Suspended, Inactive
    public string? Subdomain { get; set; }
    public string? CustomDomain { get; set; }
    public string? DedicatedConnectionString { get; set; }
    public TenantIsolationStrategy IsolationStrategy { get; set; } = TenantIsolationStrategy.SharedDatabase;

    // Auditability & Soft Delete
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }

    // Navigation Properties
    public TenantSetting? Setting { get; set; }
    
    [NotMapped]
    public TenantSubscription? ActiveSubscription => Subscriptions.FirstOrDefault(s => s.Status == "Active");

    public ICollection<Branch> Branches { get; set; } = new List<Branch>();
    public ICollection<User> Users { get; set; } = new List<User>();
    public ICollection<Role> Roles { get; set; } = new List<Role>();
    public ICollection<TenantSubscription> Subscriptions { get; set; } = new List<TenantSubscription>();
}

