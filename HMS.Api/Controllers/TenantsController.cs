using HMS.Application.Contracts;
using HMS.Common.Models;
using HMS.Domain.Entities;
using HMS.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HMS.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class TenantsController : ControllerBase
{
    private readonly HmsDbContext _dbContext;
    private readonly ICurrentTenantService _tenantService;
    private readonly IAuditService _auditService;

    public TenantsController(HmsDbContext dbContext, ICurrentTenantService tenantService, IAuditService auditService)
    {
        _dbContext = dbContext;
        _tenantService = tenantService;
        _auditService = auditService;
    }

    [HttpGet("current")]
    public async Task<ActionResult<Result<Tenant>>> GetCurrentTenant(CancellationToken cancellationToken)
    {
        var tenant = await _dbContext.Tenants
            .Include(t => t.Setting)
            .Include(t => t.Branches)
            .FirstOrDefaultAsync(t => t.Id == _tenantService.TenantId, cancellationToken);

        if (tenant == null)
            return NotFound(Result.Failure<Tenant>("Tenant not found."));

        return Ok(Result.Success(tenant));
    }

    [HttpGet("settings")]
    public async Task<ActionResult<Result<TenantSetting>>> GetTenantSettings(CancellationToken cancellationToken)
    {
        var settings = await _dbContext.TenantSettings
            .FirstOrDefaultAsync(s => s.TenantId == _tenantService.TenantId, cancellationToken);

        if (settings == null)
            return NotFound(Result.Failure<TenantSetting>("Tenant settings not found."));

        return Ok(Result.Success(settings));
    }

    [HttpPut("settings")]
    public async Task<ActionResult<Result>> UpdateTenantSettings([FromBody] TenantSetting updatedSetting, CancellationToken cancellationToken)
    {
        var settings = await _dbContext.TenantSettings
            .FirstOrDefaultAsync(s => s.TenantId == _tenantService.TenantId, cancellationToken);

        if (settings == null)
            return NotFound(Result.Failure("Tenant settings not found."));

        var oldValues = new { settings.DateFormat, settings.TimeZone, settings.Currency, settings.InvoicePrefix, settings.PatientPrefix };

        settings.DateFormat = updatedSetting.DateFormat;
        settings.TimeZone = updatedSetting.TimeZone;
        settings.Currency = updatedSetting.Currency;
        settings.Language = updatedSetting.Language;
        settings.TaxNumber = updatedSetting.TaxNumber;
        settings.LogoUrl = updatedSetting.LogoUrl;
        settings.InvoicePrefix = updatedSetting.InvoicePrefix;
        settings.PatientPrefix = updatedSetting.PatientPrefix;
        settings.BillingSettingsJson = updatedSetting.BillingSettingsJson;
        settings.NotificationSettingsJson = updatedSetting.NotificationSettingsJson;
        settings.SecuritySettingsJson = updatedSetting.SecuritySettingsJson;
        settings.FeatureSettingsJson = updatedSetting.FeatureSettingsJson;

        await _dbContext.SaveChangesAsync(cancellationToken);

        await _auditService.LogAsync("UpdateSettings", "System", "TenantSetting", settings.Id.ToString(), oldValues, settings, cancellationToken);

        return Ok(Result.Success());
    }
}

