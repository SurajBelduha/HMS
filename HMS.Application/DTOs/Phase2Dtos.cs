namespace HMS.Application.DTOs.Phase2;

public class RegisterPatientRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty; // Male, Female, Other
    public DateTime DOB { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? BloodGroup { get; set; }
    public string? AddressLine1 { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PinCode { get; set; }
    public string? EmergencyContactName { get; set; }
    public string? EmergencyContactPhone { get; set; }
}

public class CreateDoctorRequest
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;
    public string Qualification { get; set; } = string.Empty;
    public string RegistrationNumber { get; set; } = string.Empty;
    public decimal ConsultationFee { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public List<Guid> DepartmentIds { get; set; } = new();
}

public class CreateDoctorScheduleRequest
{
    public Guid DoctorId { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public int SlotDurationMinutes { get; set; } = 15;
    public int MaxPatients { get; set; } = 20;
}

public class BookAppointmentRequest
{
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }
    public Guid? DepartmentId { get; set; }
    public Guid? ServiceId { get; set; }
    public DateTime AppointmentDate { get; set; }
    public TimeSpan SlotStartTime { get; set; }
    public string? Reason { get; set; }
}

public class RescheduleAppointmentRequest
{
    public Guid AppointmentId { get; set; }
    public DateTime NewAppointmentDate { get; set; }
    public TimeSpan NewSlotStartTime { get; set; }
}

