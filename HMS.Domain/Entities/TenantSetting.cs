using HMS.Domain.Contracts;

namespace HMS.Domain.Entities;

public class TenantSetting : ITenantScoped, IAuditableEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public string DateFormat { get; set; } = "yyyy-MM-dd";
    public string TimeZone { get; set; } = "UTC";
    public string Currency { get; set; } = "USD";
    public string Language { get; set; } = "en";
    public string? TaxNumber { get; set; }
    public string? LogoUrl { get; set; }
    public string InvoicePrefix { get; set; } = "INV-";
    public string PatientPrefix { get; set; } = "PAT-";

    // Additional configuration JSON strings for modular settings
    public string? BillingSettingsJson { get; set; }
    public string? NotificationSettingsJson { get; set; }
    public string? SecuritySettingsJson { get; set; }
    public string? FeatureSettingsJson { get; set; }

    // Auditability
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    // Navigation
    public Tenant Tenant { get; set; } = null!;
}

