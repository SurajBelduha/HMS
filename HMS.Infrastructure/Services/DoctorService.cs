using HMS.Application.Contracts;
using HMS.Application.DTOs.Phase2;
using HMS.Common.Models;
using HMS.Domain.Entities;
using HMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HMS.Infrastructure.Services;

public class DoctorService : IDoctorService
{
    private readonly HmsDbContext _dbContext;
    private readonly ICurrentTenantService _tenantService;
    private readonly IFeatureEntitlementService _entitlementService;
    private readonly ISequenceGeneratorService _sequenceGenerator;
    private readonly IAuditService _auditService;

    public DoctorService(
        HmsDbContext dbContext,
        ICurrentTenantService tenantService,
        IFeatureEntitlementService entitlementService,
        ISequenceGeneratorService sequenceGenerator,
        IAuditService auditService)
    {
        _dbContext = dbContext;
        _tenantService = tenantService;
        _entitlementService = entitlementService;
        _sequenceGenerator = sequenceGenerator;
        _auditService = auditService;
    }

    public async Task<Result<Doctor>> CreateDoctorAsync(CreateDoctorRequest request, CancellationToken cancellationToken = default)
    {
        var canCreate = await _entitlementService.CanCreateDoctorAsync(_tenantService.TenantId, cancellationToken);
        if (!canCreate)
            return Result.Failure<Doctor>("Doctor creation limit reached for your current subscription plan.");

        var doctorNo = await _sequenceGenerator.GenerateSequenceNumberAsync<Doctor>(
            "DOC",
            q => q.Where(d => d.TenantId == _tenantService.TenantId),
            padLeft: 3,
            includeYear: false,
            cancellationToken);

        var doctor = new Doctor
        {
            TenantId = _tenantService.TenantId,
            BranchId = _tenantService.BranchId,
            DoctorNo = doctorNo,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Specialization = request.Specialization,
            Qualification = request.Qualification,
            RegistrationNumber = request.RegistrationNumber,
            ConsultationFee = request.ConsultationFee,
            Phone = request.Phone,
            Email = request.Email,
            Status = "Active"
        };

        foreach (var deptId in request.DepartmentIds)
        {
            doctor.DoctorDepartments.Add(new DoctorDepartment
            {
                DoctorId = doctor.Id,
                DepartmentId = deptId
            });
        }

        _dbContext.Doctors.Add(doctor);
        await _dbContext.SaveChangesAsync(cancellationToken);

        await _auditService.LogAsync("CreateDoctor", "Clinical", "Doctor", doctor.Id.ToString(), null, new { doctor.DoctorNo, doctor.FullName }, cancellationToken);

        return Result.Success(doctor);
    }

    public async Task<Result<DoctorSchedule>> AddDoctorScheduleAsync(CreateDoctorScheduleRequest request, CancellationToken cancellationToken = default)
    {
        var doctor = await _dbContext.Doctors.FirstOrDefaultAsync(d => d.Id == request.DoctorId, cancellationToken);
        if (doctor == null)
            return Result.Failure<DoctorSchedule>("Doctor not found.");

        var schedule = new DoctorSchedule
        {
            TenantId = _tenantService.TenantId,
            BranchId = _tenantService.BranchId,
            DoctorId = request.DoctorId,
            DayOfWeek = request.DayOfWeek,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            SlotDurationMinutes = request.SlotDurationMinutes,
            MaxPatients = request.MaxPatients,
            IsAvailable = true
        };

        _dbContext.DoctorSchedules.Add(schedule);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(schedule);
    }

    public async Task<Result<List<Doctor>>> GetDoctorsAsync(CancellationToken cancellationToken = default)
    {
        var doctors = await _dbContext.Doctors
            .Include(d => d.DoctorDepartments)
                .ThenInclude(dd => dd.Department)
            .Include(d => d.Schedules)
            .ToListAsync(cancellationToken);

        return Result.Success(doctors);
    }

    public async Task<Result<Doctor>> GetDoctorByIdAsync(Guid doctorId, CancellationToken cancellationToken = default)
    {
        var doctor = await _dbContext.Doctors
            .Include(d => d.DoctorDepartments)
                .ThenInclude(dd => dd.Department)
            .Include(d => d.Schedules)
            .FirstOrDefaultAsync(d => d.Id == doctorId, cancellationToken);

        if (doctor == null)
            return Result.Failure<Doctor>("Doctor not found.");

        return Result.Success(doctor);
    }

    public async Task<Result<List<DoctorSchedule>>> GetDoctorSchedulesAsync(Guid doctorId, CancellationToken cancellationToken = default)
    {
        var schedules = await _dbContext.DoctorSchedules
            .Where(s => s.DoctorId == doctorId)
            .ToListAsync(cancellationToken);

        return Result.Success(schedules);
    }
}

