using HMS.Application.Contracts;
using HMS.Application.DTOs.Phase2;
using HMS.Common.Models;
using HMS.Domain.Entities;
using HMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HMS.Infrastructure.Services;

public class PatientService : IPatientService
{
    private readonly HmsDbContext _dbContext;
    private readonly ICurrentTenantService _tenantService;
    private readonly IFeatureEntitlementService _entitlementService;
    private readonly ISequenceGeneratorService _sequenceGenerator;
    private readonly IAuditService _auditService;

    public PatientService(
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

    public async Task<Result<Patient>> RegisterPatientAsync(RegisterPatientRequest request, CancellationToken cancellationToken = default)
    {
        var canCreate = await _entitlementService.CanCreatePatientAsync(_tenantService.TenantId, cancellationToken);
        if (!canCreate)
            return Result.Failure<Patient>("Patient creation limit reached for your current subscription plan.");

        // Clean sequence number generation using centralized SequenceGeneratorService
        var mrn = await _sequenceGenerator.GenerateSequenceNumberAsync<Patient>(
            "PAT",
            q => q.Where(p => p.TenantId == _tenantService.TenantId),
            padLeft: 5,
            includeYear: true,
            cancellationToken);

        var patient = new Patient
        {
            TenantId = _tenantService.TenantId,
            BranchId = _tenantService.BranchId,
            MRN = mrn,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Gender = request.Gender,
            DOB = request.DOB,
            Phone = request.Phone,
            Email = request.Email,
            BloodGroup = request.BloodGroup,
            Status = "Active"
        };

        if (!string.IsNullOrWhiteSpace(request.AddressLine1))
        {
            patient.Addresses.Add(new PatientAddress
            {
                AddressType = "Home",
                AddressLine1 = request.AddressLine1,
                City = request.City ?? "City",
                State = request.State ?? "State",
                Country = "USA",
                PinCode = request.PinCode ?? "000000"
            });
        }

        if (!string.IsNullOrWhiteSpace(request.EmergencyContactName))
        {
            patient.Contacts.Add(new PatientContact
            {
                ContactName = request.EmergencyContactName,
                Relationship = "Emergency Contact",
                Phone = request.EmergencyContactPhone ?? request.Phone,
                IsEmergencyContact = true
            });
        }

        _dbContext.Patients.Add(patient);
        await _dbContext.SaveChangesAsync(cancellationToken);

        await _auditService.LogAsync("RegisterPatient", "Clinical", "Patient", patient.Id.ToString(), null, new { patient.MRN, patient.FullName }, cancellationToken);

        return Result.Success(patient);
    }

    public async Task<Result<Patient>> GetPatientByIdAsync(Guid patientId, CancellationToken cancellationToken = default)
    {
        var patient = await _dbContext.Patients
            .Include(p => p.Addresses)
            .Include(p => p.Contacts)
            .Include(p => p.Documents)
            .FirstOrDefaultAsync(p => p.Id == patientId, cancellationToken);

        if (patient == null)
            return Result.Failure<Patient>("Patient not found.");

        return Result.Success(patient);
    }

    public async Task<Result<PagedResult<Patient>>> SearchPatientsAsync(PagedRequest request, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Patients.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.SearchTerm))
        {
            var search = request.SearchTerm.ToLower();
            query = query.Where(p => p.MRN.ToLower().Contains(search) ||
                                     p.FirstName.ToLower().Contains(search) ||
                                     p.LastName.ToLower().Contains(search) ||
                                     p.Phone.Contains(search));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var pagedResult = new PagedResult<Patient>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = request.PageNumber,
            PageSize = request.PageSize
        };

        return Result.Success(pagedResult);
    }
}

