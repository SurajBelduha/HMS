using HMS.Application.DTOs.Phase2;
using HMS.Common.Models;
using HMS.Domain.Entities;

namespace HMS.Application.Contracts;

public interface IAppointmentService
{
    Task<Result<Appointment>> BookAppointmentAsync(BookAppointmentRequest request, CancellationToken cancellationToken = default);
    Task<Result<Appointment>> RescheduleAppointmentAsync(RescheduleAppointmentRequest request, CancellationToken cancellationToken = default);
    Task<Result> CancelAppointmentAsync(Guid appointmentId, string? reason = null, CancellationToken cancellationToken = default);
    Task<Result<List<Appointment>>> GetDoctorAppointmentsAsync(Guid doctorId, DateTime? date = null, CancellationToken cancellationToken = default);
    Task<Result<List<Appointment>>> GetPatientAppointmentsAsync(Guid patientId, CancellationToken cancellationToken = default);
}

