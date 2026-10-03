using HMS.Domain.Contracts;

namespace HMS.Domain.Entities;

public class AuditLog : ITenantScoped, IBranchScoped
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public Guid? BranchId { get; set; }
    public Guid? UserId { get; set; }
    public string Action { get; set; } = string.Empty;     // Create, Update, Delete, Login, Logout, PermissionChange
    public string Module { get; set; } = string.Empty;     // Patient, Billing, Security, System
    public string EntityName { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string? OldValues { get; set; }                 // JSON formatted
    public string? NewValues { get; set; }                 // JSON formatted
    public string? IPAddress { get; set; }
    public string? UserAgent { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

