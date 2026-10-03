using HMS.Domain.Contracts;

namespace HMS.Domain.Entities;

public class HospitalService : ITenantScoped, IBranchScoped, IAuditableEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public Guid? BranchId { get; set; }
    public Guid? DepartmentId { get; set; }
    public string ServiceCode { get; set; } = string.Empty; // Consultation, ECG, X-Ray, BloodTest
    public string ServiceName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public bool IsActive { get; set; } = true;

    // Auditability
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    public Department? Department { get; set; }
}

