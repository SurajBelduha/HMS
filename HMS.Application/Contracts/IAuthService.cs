using HMS.Application.DTOs.Auth;
using HMS.Common.Models;

namespace HMS.Application.Contracts;

public interface IAuthService
{
    Task<Result<LoginResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task<Result<LoginResponse>> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default);
    Task<Result> LogoutAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Result> ResetPasswordAsync(PasswordResetRequest request, CancellationToken cancellationToken = default);
    Task<Result<Guid>> RegisterTenantAsync(RegisterTenantRequest request, CancellationToken cancellationToken = default);
}

