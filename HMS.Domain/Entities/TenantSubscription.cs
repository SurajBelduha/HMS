using HMS.Domain.Contracts;

namespace HMS.Domain.Entities;

public class TenantSubscription : ITenantScoped, IAuditableEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public Guid PlanId { get; set; }
    public string Status { get; set; } = "Active"; // Active, Trialing, PastDue, Cancelled, Expired
    public DateTime StartDate { get; set; } = DateTime.UtcNow;
    public DateTime EndDate { get; set; }
    public DateTime? TrialEndDate { get; set; }
    public DateTime? NextBillingDate { get; set; }
    public bool AutoRenew { get; set; } = true;

    // Auditability
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    // Navigation
    public Tenant Tenant { get; set; } = null!;
    public Plan Plan { get; set; } = null!;
    public ICollection<SubscriptionItem> Items { get; set; } = new List<SubscriptionItem>();
}

