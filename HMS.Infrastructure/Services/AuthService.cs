using System.Security.Cryptography;
using System.Text;
using HMS.Application.Contracts;
using HMS.Application.DTOs.Auth;
using HMS.Common.Models;
using HMS.Domain.Entities;
using HMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace HMS.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly HmsDbContext _dbContext;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(HmsDbContext dbContext, IJwtTokenService jwtTokenService, ILogger<AuthService> logger)
    {
        _dbContext = dbContext;
        _jwtTokenService = jwtTokenService;
        _logger = logger;
    }

    public async Task<Result<LoginResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        // Ignore tenant query filters during authentication lookup
        var user = await _dbContext.Users
            .IgnoreQueryFilters()
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                    .ThenInclude(r => r.RolePermissions)
                        .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower() && !u.IsDeleted, cancellationToken);

        if (user == null)
            return Result.Failure<LoginResponse>("Invalid email or password.");

        if (user.LockoutEnd.HasValue && user.LockoutEnd.Value > DateTime.UtcNow)
            return Result.Failure<LoginResponse>($"Account is locked out until {user.LockoutEnd.Value:g}.");

        if (user.Status != "Active")
            return Result.Failure<LoginResponse>("User account is inactive or suspended.");

        if (!VerifyPassword(request.Password, user.PasswordHash))
        {
            user.AccessFailedCount++;
            if (user.AccessFailedCount >= 5)
            {
                user.LockoutEnd = DateTime.UtcNow.AddMinutes(15);
                _logger.LogWarning("User {Email} locked out due to 5 consecutive failed login attempts.", user.Email);
            }
            await _dbContext.SaveChangesAsync(cancellationToken);
            return Result.Failure<LoginResponse>("Invalid email or password.");
        }

        // Reset lockout on successful login
        user.AccessFailedCount = 0;
        user.LockoutEnd = null;
        user.LastLoginAt = DateTime.UtcNow;

        var roles = user.UserRoles.Select(ur => ur.Role.Code).Distinct().ToList();
        var permissions = user.UserRoles
            .SelectMany(ur => ur.Role.RolePermissions)
            .Select(rp => rp.Permission.Code)
            .Distinct()
            .ToList();

        var accessToken = _jwtTokenService.GenerateAccessToken(user, roles, permissions);
        var refreshTokenValue = _jwtTokenService.GenerateRefreshToken();

        var refreshToken = new RefreshToken
        {
            UserId = user.Id,
            Token = refreshTokenValue,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            IsRevoked = false
        };

        _dbContext.RefreshTokens.Add(refreshToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshTokenValue,
            ExpiresAt = DateTime.UtcNow.AddHours(8),
            TenantId = user.TenantId,
            BranchId = user.BranchId,
            UserFullName = user.FullName,
            Email = user.Email,
            Roles = roles,
            Permissions = permissions
        });
    }

    public async Task<Result<LoginResponse>> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default)
    {
        var tokenEntity = await _dbContext.RefreshTokens
            .Include(t => t.User)
                .ThenInclude(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                        .ThenInclude(r => r.RolePermissions)
                            .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(t => t.Token == request.RefreshToken && !t.IsRevoked, cancellationToken);

        if (tokenEntity == null || tokenEntity.ExpiresAt <= DateTime.UtcNow)
            return Result.Failure<LoginResponse>("Invalid or expired refresh token.");

        var user = tokenEntity.User;
        var roles = user.UserRoles.Select(ur => ur.Role.Code).Distinct().ToList();
        var permissions = user.UserRoles
            .SelectMany(ur => ur.Role.RolePermissions)
            .Select(rp => rp.Permission.Code)
            .Distinct()
            .ToList();

        var newAccessToken = _jwtTokenService.GenerateAccessToken(user, roles, permissions);
        var newRefreshTokenValue = _jwtTokenService.GenerateRefreshToken();

        tokenEntity.IsRevoked = true;
        _dbContext.RefreshTokens.Add(new RefreshToken
        {
            UserId = user.Id,
            Token = newRefreshTokenValue,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        });

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result.Success(new LoginResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshTokenValue,
            ExpiresAt = DateTime.UtcNow.AddHours(8),
            TenantId = user.TenantId,
            BranchId = user.BranchId,
            UserFullName = user.FullName,
            Email = user.Email,
            Roles = roles,
            Permissions = permissions
        });
    }

    public async Task<Result> LogoutAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var activeTokens = await _dbContext.RefreshTokens
            .Where(t => t.UserId == userId && !t.IsRevoked)
            .ToListAsync(cancellationToken);

        foreach (var token in activeTokens)
        {
            token.IsRevoked = true;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result> ResetPasswordAsync(PasswordResetRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.Users
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower() && !u.IsDeleted, cancellationToken);

        if (user == null)
            return Result.Failure("User not found.");

        user.PasswordHash = HashPassword(request.NewPassword);
        user.AccessFailedCount = 0;
        user.LockoutEnd = null;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }

    public async Task<Result<Guid>> RegisterTenantAsync(RegisterTenantRequest request, CancellationToken cancellationToken = default)
    {
        var existingTenant = await _dbContext.Tenants
            .IgnoreQueryFilters()
            .AnyAsync(t => t.TenantCode.ToLower() == request.TenantCode.ToLower(), cancellationToken);

        if (existingTenant)
            return Result.Failure<Guid>("Tenant code already exists.");

        var tenant = new Tenant
        {
            TenantCode = request.TenantCode,
            Name = request.TenantName,
            LegalName = request.LegalName,
            Email = request.AdminEmail,
            Phone = "1234567890",
            Status = "Active"
        };

        var tenantSetting = new TenantSetting
        {
            TenantId = tenant.Id,
            InvoicePrefix = "INV-",
            PatientPrefix = "PAT-"
        };

        var branch = new Branch
        {
            TenantId = tenant.Id,
            BranchCode = "MAIN-01",
            Name = string.IsNullOrWhiteSpace(request.PrimaryBranchName) ? "Main Branch" : request.PrimaryBranchName,
            Address = "Main Address",
            City = "Default City",
            State = "Default State",
            Country = "USA",
            PinCode = "000000",
            Phone = "1234567890",
            Email = request.AdminEmail
        };

        var hospitalAdminRole = new Role
        {
            TenantId = tenant.Id,
            Name = "Hospital Owner",
            Code = "HOSPITAL_OWNER",
            IsSystemRole = true
        };

        var user = new User
        {
            TenantId = tenant.Id,
            BranchId = branch.Id,
            Email = request.AdminEmail,
            FirstName = request.AdminFirstName,
            LastName = request.AdminLastName,
            PasswordHash = HashPassword(request.AdminPassword),
            Status = "Active",
            EmailVerified = true
        };

        var userRole = new UserRole
        {
            UserId = user.Id,
            RoleId = hospitalAdminRole.Id
        };

        _dbContext.Tenants.Add(tenant);
        _dbContext.TenantSettings.Add(tenantSetting);
        _dbContext.Branches.Add(branch);
        _dbContext.Roles.Add(hospitalAdminRole);
        _dbContext.Users.Add(user);
        _dbContext.UserRoles.Add(userRole);

        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success(tenant.Id);
    }

    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }

    private static bool VerifyPassword(string password, string hash)
    {
        return HashPassword(password) == hash;
    }
}

