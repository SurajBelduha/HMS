using System.Text.Json;
using HMS.Application.Contracts;
using HMS.Domain.Entities;
using HMS.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;

namespace HMS.Infrastructure.Services;

public class AuditService : IAuditService
{
    private readonly HmsDbContext _dbContext;
    private readonly ICurrentTenantService _tenantService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuditService(
        HmsDbContext dbContext,
        ICurrentTenantService tenantService,
        IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _tenantService = tenantService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task LogAsync(
        string action,
        string module,
        string entityName,
        string entityId,
        object? oldValues = null,
        object? newValues = null,
        CancellationToken cancellationToken = default)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        var ipAddress = httpContext?.Connection.RemoteIpAddress?.ToString() ?? "Local";
        var userAgent = httpContext?.Request.Headers["User-Agent"].ToString() ?? "Unknown";

        var auditLog = new AuditLog
        {
            TenantId = _tenantService.TenantId,
            BranchId = _tenantService.BranchId,
            UserId = _tenantService.UserId,
            Action = action,
            Module = module,
            EntityName = entityName,
            EntityId = entityId,
            OldValues = oldValues != null ? JsonSerializer.Serialize(oldValues) : null,
            NewValues = newValues != null ? JsonSerializer.Serialize(newValues) : null,
            IPAddress = ipAddress,
            UserAgent = userAgent,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.AuditLogs.Add(auditLog);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}

