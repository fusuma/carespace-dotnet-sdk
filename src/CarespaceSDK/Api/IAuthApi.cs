using CarespaceSDK.Models;

namespace CarespaceSDK.Api;

public interface IAuthApi
{
    Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task<ApiResponse<LoginResponse>> LoginAsync(string email, string password, CancellationToken cancellationToken = default);
    Task<ApiResponse<LoginResponse>> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default);
    Task<ApiResponse<LoginResponse>> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task<ApiResponse<object>> LogoutAsync(LogoutRequest? request = null, CancellationToken cancellationToken = default);
    Task<ApiResponse<object>> ForgotPasswordAsync(ForgotPasswordRequest request, CancellationToken cancellationToken = default);
    Task<ApiResponse<object>> ForgotPasswordAsync(string email, CancellationToken cancellationToken = default);
    Task<ApiResponse<object>> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellationToken = default);
    Task<ApiResponse<object>> ResetPasswordAsync(string token, string newPassword, CancellationToken cancellationToken = default);
    Task<ApiResponse<object>> ChangePasswordAsync(ChangePasswordRequest request, CancellationToken cancellationToken = default);
    Task<ApiResponse<object>> ChangePasswordAsync(string currentPassword, string newPassword, CancellationToken cancellationToken = default);
}