using HMS.Domain.Contracts;

namespace HMS.Domain.Entities;

// ==========================================
// PHASE 3: CLINICAL OPERATIONS
// ==========================================

public class Consultation : ITenantScoped, IBranchScoped, IAuditableEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public Guid? BranchId { get; set; }
    public Guid? AppointmentId { get; set; }
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }
    public DateTime ConsultationDate { get; set; } = DateTime.UtcNow;
    public string? ChiefComplaints { get; set; }
    public string? ClinicalNotes { get; set; }
    public string? Advice { get; set; }
    public DateTime? FollowUpDate { get; set; }
    public string Status { get; set; } = "Completed";

    // Auditability
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    // Navigations
    public Patient Patient { get; set; } = null!;
    public Doctor Doctor { get; set; } = null!;
    public Appointment? Appointment { get; set; }
    public ICollection<Diagnosis> Diagnoses { get; set; } = new List<Diagnosis>();
    public ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
}

public class Diagnosis
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ConsultationId { get; set; }
    public string? ICD10Code { get; set; }
    public string DiagnosisName { get; set; } = string.Empty;
    public string Type { get; set; } = "Primary"; // Primary, Secondary

    public Consultation Consultation { get; set; } = null!;
}

public class Prescription : ITenantScoped
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public Guid ConsultationId { get; set; }
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }
    public DateTime PrescriptionDate { get; set; } = DateTime.UtcNow;
    public string? Instructions { get; set; }

    public Consultation Consultation { get; set; } = null!;
    public Patient Patient { get; set; } = null!;
    public Doctor Doctor { get; set; } = null!;
    public ICollection<PrescriptionItem> Items { get; set; } = new List<PrescriptionItem>();
}

public class PrescriptionItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid PrescriptionId { get; set; }
    public string MedicineName { get; set; } = string.Empty;
    public string Dosage { get; set; } = string.Empty; // e.g. 1-0-1
    public string Frequency { get; set; } = string.Empty; // Twice Daily
    public int DurationDays { get; set; }
    public string? Instructions { get; set; }

    public Prescription Prescription { get; set; } = null!;
}

public class Ward : ITenantScoped, IBranchScoped
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public Guid? BranchId { get; set; }
    public string WardName { get; set; } = string.Empty;
    public string WardType { get; set; } = "General"; // General, ICU, Private, SemiPrivate
    public string Floor { get; set; } = string.Empty;

    public ICollection<Room> Rooms { get; set; } = new List<Room>();
}

public class Room
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid WardId { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public string RoomType { get; set; } = string.Empty;

    public Ward Ward { get; set; } = null!;
    public ICollection<Bed> Beds { get; set; } = new List<Bed>();
}

public class Bed
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid RoomId { get; set; }
    public string BedNumber { get; set; } = string.Empty;
    public decimal DailyCharge { get; set; }
    public string Status { get; set; } = "Available"; // Available, Occupied, Maintenance

    public Room Room { get; set; } = null!;
}

public class Admission : ITenantScoped, IBranchScoped, IAuditableEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public Guid? BranchId { get; set; }
    public string AdmissionNo { get; set; } = string.Empty; // ADM-2026-00001
    public Guid PatientId { get; set; }
    public Guid AttendingDoctorId { get; set; }
    public Guid BedId { get; set; }
    public DateTime AdmissionDate { get; set; } = DateTime.UtcNow;
    public DateTime? DischargeDate { get; set; }
    public string Status { get; set; } = "Admitted"; // Admitted, Discharged
    public string? ReasonForAdmission { get; set; }

    // Auditability
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    public Patient Patient { get; set; } = null!;
    public Doctor AttendingDoctor { get; set; } = null!;
    public Bed Bed { get; set; } = null!;
}

public class Vital : ITenantScoped
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public Guid PatientId { get; set; }
    public Guid? AdmissionId { get; set; }
    public DateTime RecordedAt { get; set; } = DateTime.UtcNow;
    public string? BloodPressure { get; set; }
    public int? PulseRate { get; set; }
    public decimal? TemperatureDecimal { get; set; }
    public int? OxygenSaturation { get; set; }
    public int? RespiratoryRate { get; set; }

    public Patient Patient { get; set; } = null!;
    public Admission? Admission { get; set; }
}


// ==========================================
// PHASE 4: REVENUE, PHARMACY & LAB
// ==========================================

public class Invoice : ITenantScoped, IBranchScoped, IAuditableEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public Guid? BranchId { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty; // INV-2026-00001
    public Guid PatientId { get; set; }
    public decimal SubTotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public string Status { get; set; } = "Unpaid"; // Unpaid, PartiallyPaid, Paid

    public DateTime InvoiceDate { get; set; } = DateTime.UtcNow;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }

    public Patient Patient { get; set; } = null!;
    public ICollection<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}

public class InvoiceItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid InvoiceId { get; set; }
    public string ItemDescription { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; } = 1;
    public decimal TotalPrice { get; set; }

    public Invoice Invoice { get; set; } = null!;
}

public class Payment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid InvoiceId { get; set; }
    public string PaymentNumber { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = "Cash"; // Cash, Card, UPI, Insurance
    public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
    public string? TransactionRef { get; set; }

    public Invoice Invoice { get; set; } = null!;
}

public class Medicine : ITenantScoped
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public string MedicineCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? GenericName { get; set; }
    public string Category { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int StockQuantity { get; set; }
    public DateTime ExpiryDate { get; set; }
}

public class LabOrder : ITenantScoped
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }
    public string TestName { get; set; } = string.Empty;
    public string Status { get; set; } = "Ordered"; // Ordered, SampleCollected, Completed
    public string? ResultSummary { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;

    public Patient Patient { get; set; } = null!;
    public Doctor Doctor { get; set; } = null!;
}

