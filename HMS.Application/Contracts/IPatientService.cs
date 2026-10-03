using HMS.Application.DTOs.Phase2;
using HMS.Common.Models;
using HMS.Domain.Entities;

namespace HMS.Application.Contracts;

public interface IPatientService
{
    Task<Result<Patient>> RegisterPatientAsync(RegisterPatientRequest request, CancellationToken cancellationToken = default);
    Task<Result<Patient>> GetPatientByIdAsync(Guid patientId, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<Patient>>> SearchPatientsAsync(PagedRequest request, CancellationToken cancellationToken = default);
}

