using System.Security.Claims;
using HMS.Application.Contracts;
using HMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace HMS.Api.Middleware;

public class TenantResolverMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<TenantResolverMiddleware> _logger;

    public TenantResolverMiddleware(RequestDelegate next, ILogger<TenantResolverMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, ICurrentTenantService tenantService, HmsDbContext dbContext)
    {
        Guid tenantId = Guid.Empty;
        Guid? branchId = null;
        Guid? userId = null;
        string? role = null;
        bool isHostAdmin = false;

        // 1. Resolve from Authenticated JWT Claims
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var tenantClaim = context.User.FindFirst("TenantId")?.Value;
            var branchClaim = context.User.FindFirst("BranchId")?.Value;
            var userClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? context.User.FindFirst("sub")?.Value;
            role = context.User.FindFirst(ClaimTypes.Role)?.Value;

            if (Guid.TryParse(tenantClaim, out var parsedTenantId))
            {
                tenantId = parsedTenantId;
            }

            if (Guid.TryParse(branchClaim, out var parsedBranchId))
            {
                branchId = parsedBranchId;
            }

            if (Guid.TryParse(userClaim, out var parsedUserId))
            {
                userId = parsedUserId;
            }

            if (role == "PlatformSuperAdmin" || role == "PlatformSupport")
            {
                isHostAdmin = true;
            }
        }

        // 2. Resolve from X-Tenant-Id Header if not authenticated or for tenant discovery
        if (tenantId == Guid.Empty && context.Request.Headers.TryGetValue("X-Tenant-Id", out var tenantHeaderValues))
        {
            var headerTenant = tenantHeaderValues.FirstOrDefault();
            if (Guid.TryParse(headerTenant, out var headerTenantId))
            {
                tenantId = headerTenantId;
            }
        }

        // 3. Resolve from Subdomain / Host
        if (tenantId == Guid.Empty)
        {
            var host = context.Request.Host.Host;
            var parts = host.Split('.');
            if (parts.Length > 2)
            {
                var subdomain = parts[0];
                var tenant = await dbContext.Tenants
                    .IgnoreQueryFilters()
                    .FirstOrDefaultAsync(t => (t.Subdomain != null && t.Subdomain.ToLower() == subdomain.ToLower())
                                              || t.TenantCode.ToLower() == subdomain.ToLower());

                if (tenant != null)
                {
                    tenantId = tenant.Id;
                }
            }
        }

        // 4. Set Tenant Context on Scoped Service
        if (tenantId != Guid.Empty || isHostAdmin)
        {
            tenantService.SetTenantContext(tenantId, branchId, userId, role, isHostAdmin);
        }

        await _next(context);
    }
}

