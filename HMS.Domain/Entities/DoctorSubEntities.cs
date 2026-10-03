using HMS.Domain.Contracts;

namespace HMS.Domain.Entities;

public class DoctorDepartment
{
    public Guid DoctorId { get; set; }
    public Doctor Doctor { get; set; } = null!;

    public Guid DepartmentId { get; set; }
    public Department Department { get; set; } = null!;
}

public class DoctorSchedule : ITenantScoped, IBranchScoped
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public Guid? BranchId { get; set; }
    public Guid DoctorId { get; set; }
    public DayOfWeek DayOfWeek { get; set; } // Monday, Tuesday, etc.
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public int SlotDurationMinutes { get; set; } = 15;
    public int MaxPatients { get; set; } = 20;
    public bool IsAvailable { get; set; } = true;

    public Doctor Doctor { get; set; } = null!;
}

