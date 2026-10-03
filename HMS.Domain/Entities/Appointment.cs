using HMS.Domain.Contracts;

namespace HMS.Domain.Entities;

public class Appointment : ITenantScoped, IBranchScoped, IAuditableEntity, ISoftDelete
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public Guid? BranchId { get; set; }
    public string AppointmentNo { get; set; } = string.Empty; // APT-2026-00001
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }
    public Guid? DepartmentId { get; set; }
    public Guid? ServiceId { get; set; }
    public DateTime AppointmentDate { get; set; }
    public TimeSpan SlotStartTime { get; set; }
    public TimeSpan SlotEndTime { get; set; }
    public int TokenNumber { get; set; } // OPD Token #1, #2, #3
    public string Status { get; set; } = "Scheduled"; // Scheduled, Completed, Cancelled, Rescheduled, NoShow
    public string? Reason { get; set; }
    public decimal ConsultationFee { get; set; }

    // Auditability & Soft Delete
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }

    // Navigations
    public Tenant Tenant { get; set; } = null!;
    public Branch? Branch { get; set; }
    public Patient Patient { get; set; } = null!;
    public Doctor Doctor { get; set; } = null!;
    public Department? Department { get; set; }
    public HospitalService? Service { get; set; }
}

