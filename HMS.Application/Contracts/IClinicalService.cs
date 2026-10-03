using HMS.Application.DTOs.Phase3And4;
using HMS.Common.Models;
using HMS.Domain.Entities;

namespace HMS.Application.Contracts;

public interface IClinicalService
{
    Task<Result<Consultation>> CreateConsultationAsync(CreateConsultationRequest request, CancellationToken cancellationToken = default);
    Task<Result<Admission>> AdmitPatientAsync(AdmitPatientRequest request, CancellationToken cancellationToken = default);
    Task<Result<Vital>> RecordVitalsAsync(Vital vital, CancellationToken cancellationToken = default);
    Task<Result<List<Consultation>>> GetPatientConsultationsAsync(Guid patientId, CancellationToken cancellationToken = default);
}

