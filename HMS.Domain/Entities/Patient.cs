using HMS.Domain.Contracts;

namespace HMS.Domain.Entities;

public class Patient : ITenantScoped, IBranchScoped, IAuditableEntity, ISoftDelete
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public Guid? BranchId { get; set; }
    public string MRN { get; set; } = string.Empty; // Medical Record Number (PAT-2026-00001)
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty; // Male, Female, Other
    public DateTime DOB { get; set; }
    public string? BloodGroup { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string Status { get; set; } = "Active";

    // Auditability & Soft Delete
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }

    public string FullName => $"{FirstName} {LastName}".Trim();

    // Navigations
    public Tenant Tenant { get; set; } = null!;
    public Branch? Branch { get; set; }
    public ICollection<PatientAddress> Addresses { get; set; } = new List<PatientAddress>();
    public ICollection<PatientDocument> Documents { get; set; } = new List<PatientDocument>();
    public ICollection<PatientContact> Contacts { get; set; } = new List<PatientContact>();
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}

