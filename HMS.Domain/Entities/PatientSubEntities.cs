using HMS.Domain.Contracts;

namespace HMS.Domain.Entities;

public class PatientAddress
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid PatientId { get; set; }
    public string AddressType { get; set; } = "Home"; // Home, Work, Permanent
    public string AddressLine1 { get; set; } = string.Empty;
    public string? AddressLine2 { get; set; }
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string PinCode { get; set; } = string.Empty;

    public Patient Patient { get; set; } = null!;
}

public class PatientDocument : ITenantScoped
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public Guid PatientId { get; set; }
    public string DocumentType { get; set; } = string.Empty; // IDProof, Insurance, Report
    public string DocumentName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    public Patient Patient { get; set; } = null!;
}

public class PatientContact
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid PatientId { get; set; }
    public string ContactName { get; set; } = string.Empty;
    public string Relationship { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public bool IsEmergencyContact { get; set; } = false;

    public Patient Patient { get; set; } = null!;
}

