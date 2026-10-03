using HMS.Application.Contracts;
using HMS.Application.DTOs.Phase2;
using HMS.Common.Models;
using HMS.Domain.Entities;
using HMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HMS.Infrastructure.Services;

public class AppointmentService : IAppointmentService
{
    private readonly HmsDbContext _dbContext;
    private readonly ICurrentTenantService _tenantService;
    private readonly ISequenceGeneratorService _sequenceGenerator;
    private readonly IAuditService _auditService;

    public AppointmentService(
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

    public async Task<Result<Appointment>> BookAppointmentAsync(BookAppointmentRequest request, CancellationToken cancellationToken = default)
    {
        var patient = await _dbContext.Patients.FirstOrDefaultAsync(p => p.Id == request.PatientId, cancellationToken);
        if (patient == null)
            return Result.Failure<Appointment>("Patient not found.");

        var doctor = await _dbContext.Doctors.FirstOrDefaultAsync(d => d.Id == request.DoctorId, cancellationToken);
        if (doctor == null)
            return Result.Failure<Appointment>("Doctor not found.");

        var appointmentDate = request.AppointmentDate.Date;

        var schedule = await _dbContext.DoctorSchedules
            .FirstOrDefaultAsync(s => s.DoctorId == request.DoctorId && s.DayOfWeek == appointmentDate.DayOfWeek && s.IsAvailable, cancellationToken);

        if (schedule == null)
            return Result.Failure<Appointment>($"Doctor is not available on {appointmentDate.DayOfWeek}.");

        var existingCount = await _dbContext.Appointments
            .CountAsync(a => a.DoctorId == request.DoctorId && a.AppointmentDate.Date == appointmentDate && a.Status != "Cancelled", cancellationToken);

        if (existingCount >= schedule.MaxPatients)
            return Result.Failure<Appointment>($"Maximum patient limit of {schedule.MaxPatients} reached for Doctor on {appointmentDate:yyyy-MM-dd}.");

        var tokenNumber = existingCount + 1;
        var slotEndTime = request.SlotStartTime.Add(TimeSpan.FromMinutes(schedule.SlotDurationMinutes));

        var appointmentNo = await _sequenceGenerator.GenerateSequenceNumberAsync<Appointment>(
            "APT",
            q => q.Where(a => a.TenantId == _tenantService.TenantId),
            padLeft: 5,
            includeYear: true,
            cancellationToken);

        var appointment = new Appointment
        {
            TenantId = _tenantService.TenantId,
            BranchId = _tenantService.BranchId,
            AppointmentNo = appointmentNo,
            PatientId = request.PatientId,
            DoctorId = request.DoctorId,
            DepartmentId = request.DepartmentId,
            ServiceId = request.ServiceId,
            AppointmentDate = appointmentDate,
            SlotStartTime = request.SlotStartTime,
            SlotEndTime = slotEndTime,
            TokenNumber = tokenNumber,
            Status = "Scheduled",
            Reason = request.Reason,
            ConsultationFee = doctor.ConsultationFee
        };

        _dbContext.Appointments.Add(appointment);
        await _dbContext.SaveChangesAsync(cancellationToken);

        await _auditService.LogAsync("BookAppointment", "Clinical", "Appointment", appointment.Id.ToString(), null, new { appointment.AppointmentNo, appointment.TokenNumber }, cancellationToken);

        return Result.Success(appointment);
    }

    public async Task<Result<Appointment>> RescheduleAppointmentAsync(RescheduleAppointmentRequest request, CancellationToken cancellationToken = default)
    {
        var appointment = await _dbContext.Appointments.FirstOrDefaultAsync(a => a.Id == request.AppointmentId, cancellationToken);
        if (appointment == null)
            return Result.Failure<Appointment>("Appointment not found.");

        if (appointment.Status == "Cancelled" || appointment.Status == "Completed")
            return Result.Failure<Appointment>($"Cannot reschedule appointment in '{appointment.Status}' status.");

        var oldValues = new { appointment.AppointmentDate, appointment.SlotStartTime, appointment.TokenNumber };

        var newDate = request.NewAppointmentDate.Date;
        var existingCount = await _dbContext.Appointments
            .CountAsync(a => a.DoctorId == appointment.DoctorId && a.AppointmentDate.Date == newDate && a.Status != "Cancelled", cancellationToken);

        appointment.AppointmentDate = newDate;
        appointment.SlotStartTime = request.NewSlotStartTime;
        appointment.SlotEndTime = request.NewSlotStartTime.Add(TimeSpan.FromMinutes(15));
        appointment.TokenNumber = existingCount + 1;
        appointment.Status = "Rescheduled";

        await _dbContext.SaveChangesAsync(cancellationToken);

        await _auditService.LogAsync("RescheduleAppointment", "Clinical", "Appointment", appointment.Id.ToString(), oldValues, appointment, cancellationToken);

        return Result.Success(appointment);
    }

    public async Task<Result> CancelAppointmentAsync(Guid appointmentId, string? reason = null, CancellationToken cancellationToken = default)
    {
        var appointment = await _dbContext.Appointments.FirstOrDefaultAsync(a => a.Id == appointmentId, cancellationToken);
        if (appointment == null)
            return Result.Failure("Appointment not found.");

        appointment.Status = "Cancelled";
        appointment.Reason = reason ?? appointment.Reason;

        await _dbContext.SaveChangesAsync(cancellationToken);

        await _auditService.LogAsync("CancelAppointment", "Clinical", "Appointment", appointment.Id.ToString(), null, new { Status = "Cancelled", Reason = reason }, cancellationToken);

        return Result.Success();
    }

    public async Task<Result<List<Appointment>>> GetDoctorAppointmentsAsync(Guid doctorId, DateTime? date = null, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Appointments
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            .Where(a => a.DoctorId == doctorId);

        if (date.HasValue)
        {
            var targetDate = date.Value.Date;
            query = query.Where(a => a.AppointmentDate.Date == targetDate);
        }

        var list = await query.OrderBy(a => a.TokenNumber).ToListAsync(cancellationToken);
        return Result.Success(list);
    }

    public async Task<Result<List<Appointment>>> GetPatientAppointmentsAsync(Guid patientId, CancellationToken cancellationToken = default)
    {
        var list = await _dbContext.Appointments
            .Include(a => a.Doctor)
            .Where(a => a.PatientId == patientId)
            .OrderByDescending(a => a.AppointmentDate)
            .ToListAsync(cancellationToken);

        return Result.Success(list);
    }
}

