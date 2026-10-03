using HMS.Application.DTOs.Phase2;
using HMS.Common.Models;
using HMS.Domain.Entities;

namespace HMS.Application.Contracts;

public interface IDoctorService
{
    Task<Result<Doctor>> CreateDoctorAsync(CreateDoctorRequest request, CancellationToken cancellationToken = default);
    Task<Result<DoctorSchedule>> AddDoctorScheduleAsync(CreateDoctorScheduleRequest request, CancellationToken cancellationToken = default);
    Task<Result<List<Doctor>>> GetDoctorsAsync(CancellationToken cancellationToken = default);
    Task<Result<Doctor>> GetDoctorByIdAsync(Guid doctorId, CancellationToken cancellationToken = default);
    Task<Result<List<DoctorSchedule>>> GetDoctorSchedulesAsync(Guid doctorId, CancellationToken cancellationToken = default);
}

