using HMS.Domain.Contracts;

namespace HMS.Domain.Entities;

public class Doctor : ITenantScoped, IBranchScoped, IAuditableEntity, ISoftDelete
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public Guid? BranchId { get; set; }
    public Guid? UserId { get; set; }
    public string DoctorNo { get; set; } = string.Empty; // DOC-001
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;
    public string Qualification { get; set; } = string.Empty;
    public string RegistrationNumber { get; set; } = string.Empty;
    public decimal ConsultationFee { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Status { get; set; } = "Active";

    // Auditability & Soft Delete
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }

    public string FullName => $"Dr. {FirstName} {LastName}".Trim();

    // Navigations
    public Tenant Tenant { get; set; } = null!;
    public Branch? Branch { get; set; }
    public User? User { get; set; }
    public ICollection<DoctorDepartment> DoctorDepartments { get; set; } = new List<DoctorDepartment>();
    public ICollection<DoctorSchedule> Schedules { get; set; } = new List<DoctorSchedule>();
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}

