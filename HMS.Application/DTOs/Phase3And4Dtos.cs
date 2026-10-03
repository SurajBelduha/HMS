namespace HMS.Application.DTOs.Phase3And4;

public class CreateConsultationRequest
{
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }
    public Guid? AppointmentId { get; set; }
    public string? ChiefComplaints { get; set; }
    public string? ClinicalNotes { get; set; }
    public string? Advice { get; set; }
    public DateTime? FollowUpDate { get; set; }
    public List<CreateDiagnosisDto> Diagnoses { get; set; } = new();
    public List<CreatePrescriptionItemDto> Prescriptions { get; set; } = new();
}

public class CreateDiagnosisDto
{
    public string? ICD10Code { get; set; }
    public string DiagnosisName { get; set; } = string.Empty;
    public string Type { get; set; } = "Primary";
}

public class CreatePrescriptionItemDto
{
    public string MedicineName { get; set; } = string.Empty;
    public string Dosage { get; set; } = string.Empty;
    public string Frequency { get; set; } = string.Empty;
    public int DurationDays { get; set; }
    public string? Instructions { get; set; }
}

public class AdmitPatientRequest
{
    public Guid PatientId { get; set; }
    public Guid AttendingDoctorId { get; set; }
    public Guid BedId { get; set; }
    public string? ReasonForAdmission { get; set; }
}

public class GenerateInvoiceRequest
{
    public Guid PatientId { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public List<CreateInvoiceItemDto> Items { get; set; } = new();
}

public class CreateInvoiceItemDto
{
    public string ItemDescription { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; } = 1;
}

public class RecordPaymentRequest
{
    public Guid InvoiceId { get; set; }
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = "Cash";
    public string? TransactionRef { get; set; }
}

