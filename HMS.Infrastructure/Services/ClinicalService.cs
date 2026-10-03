using HMS.Application.Contracts;
using HMS.Application.DTOs.Phase3And4;
using HMS.Common.Models;
using HMS.Domain.Entities;
using HMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HMS.Infrastructure.Services;

public class ClinicalService : IClinicalService
{
    private readonly HmsDbContext _dbContext;
    private readonly ICurrentTenantService _tenantService;
    private readonly ISequenceGeneratorService _sequenceGenerator;
    private readonly IAuditService _auditService;

    public ClinicalService(
        HmsDbContext dbContext,
        ICurrentTenantService tenantService,
        ISequenceGeneratorService sequenceGenerator,
        IAuditService auditService)
    {
        _dbContext = dbContext;
        _tenantService = tenantService;
        _sequenceGenerator = sequenceGenerator;
        _auditService = auditService;
    }

    public async Task<Result<Consultation>> CreateConsultationAsync(CreateConsultationRequest request, CancellationToken cancellationToken = default)
    {
        var patient = await _dbContext.Patients.FirstOrDefaultAsync(p => p.Id == request.PatientId, cancellationToken);
        if (patient == null)
            return Result.Failure<Consultation>("Patient not found.");

        var doctor = await _dbContext.Doctors.FirstOrDefaultAsync(d => d.Id == request.DoctorId, cancellationToken);
        if (doctor == null)
            return Result.Failure<Consultation>("Doctor not found.");

        var consultation = new Consultation
        {
            TenantId = _tenantService.TenantId,
            BranchId = _tenantService.BranchId,
            AppointmentId = request.AppointmentId,
            PatientId = request.PatientId,
            DoctorId = request.DoctorId,
            ChiefComplaints = request.ChiefComplaints,
            ClinicalNotes = request.ClinicalNotes,
            Advice = request.Advice,
            FollowUpDate = request.FollowUpDate,
            Status = "Completed"
        };

        foreach (var diag in request.Diagnoses)
        {
            consultation.Diagnoses.Add(new Diagnosis
            {
                ICD10Code = diag.ICD10Code,
                DiagnosisName = diag.DiagnosisName,
                Type = diag.Type
            });
        }

        if (request.Prescriptions.Any())
        {
            var prescription = new Prescription
            {
                TenantId = _tenantService.TenantId,
                PatientId = request.PatientId,
                DoctorId = request.DoctorId,
                ConsultationId = consultation.Id
            };

            foreach (var rx in request.Prescriptions)
            {
                prescription.Items.Add(new PrescriptionItem
                {
                    MedicineName = rx.MedicineName,
                    Dosage = rx.Dosage,
                    Frequency = rx.Frequency,
                    DurationDays = rx.DurationDays,
                    Instructions = rx.Instructions
                });
            }

            consultation.Prescriptions.Add(prescription);
        }

        if (request.AppointmentId.HasValue)
        {
            var appointment = await _dbContext.Appointments.FirstOrDefaultAsync(a => a.Id == request.AppointmentId.Value, cancellationToken);
            if (appointment != null)
            {
                appointment.Status = "Completed";
            }
        }

        _dbContext.Consultations.Add(consultation);
        await _dbContext.SaveChangesAsync(cancellationToken);

        await _auditService.LogAsync("CreateConsultation", "Clinical", "Consultation", consultation.Id.ToString(), null, new { consultation.PatientId, consultation.DoctorId }, cancellationToken);

        return Result.Success(consultation);
    }

    public async Task<Result<Admission>> AdmitPatientAsync(AdmitPatientRequest request, CancellationToken cancellationToken = default)
    {
        var patient = await _dbContext.Patients.FirstOrDefaultAsync(p => p.Id == request.PatientId, cancellationToken);
        if (patient == null)
            return Result.Failure<Admission>("Patient not found.");

        var bed = await _dbContext.Beds.FirstOrDefaultAsync(b => b.Id == request.BedId, cancellationToken);
        if (bed == null || bed.Status != "Available")
            return Result.Failure<Admission>("Bed is not available for admission.");

        var admissionNo = await _sequenceGenerator.GenerateSequenceNumberAsync<Admission>(
            "ADM",
            q => q.Where(a => a.TenantId == _tenantService.TenantId),
            padLeft: 5,
            includeYear: true,
            cancellationToken);

        var admission = new Admission
        {
            TenantId = _tenantService.TenantId,
            BranchId = _tenantService.BranchId,
            AdmissionNo = admissionNo,
            PatientId = request.PatientId,
            AttendingDoctorId = request.AttendingDoctorId,
            BedId = request.BedId,
            AdmissionDate = DateTime.UtcNow,
            Status = "Admitted",
            ReasonForAdmission = request.ReasonForAdmission
        };

        bed.Status = "Occupied";

        _dbContext.Admissions.Add(admission);
        await _dbContext.SaveChangesAsync(cancellationToken);

        await _auditService.LogAsync("AdmitPatient", "IPD", "Admission", admission.Id.ToString(), null, new { admission.AdmissionNo, admission.BedId }, cancellationToken);

        return Result.Success(admission);
    }

    public async Task<Result<Vital>> RecordVitalsAsync(Vital vital, CancellationToken cancellationToken = default)
    {
        vital.TenantId = _tenantService.TenantId;
        vital.RecordedAt = DateTime.UtcNow;

        _dbContext.Vitals.Add(vital);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(vital);
    }

    public async Task<Result<List<Consultation>>> GetPatientConsultationsAsync(Guid patientId, CancellationToken cancellationToken = default)
    {
        var consultations = await _dbContext.Consultations
            .Include(c => c.Doctor)
            .Include(c => c.Diagnoses)
            .Include(c => c.Prescriptions)
                .ThenInclude(p => p.Items)
            .Where(c => c.PatientId == patientId)
            .OrderByDescending(c => c.ConsultationDate)
            .ToListAsync(cancellationToken);

        return Result.Success(consultations);
    }
}

