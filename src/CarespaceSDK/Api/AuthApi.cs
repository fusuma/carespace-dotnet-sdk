using CarespaceSDK.Http;
using CarespaceSDK.Models;

namespace CarespaceSDK.Api;

public class AuthApi : IAuthApi
{
    private readonly IHttpClient _httpClient;

    public AuthApi(IHttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));
        return await _httpClient.PostAsync<ApiResponse<LoginResponse>>("/auth/login", request, cancellationToken);
    }

    public async Task<ApiResponse<LoginResponse>> LoginAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var request = new LoginRequest { Email = email, Password = password };
        return await LoginAsync(request, cancellationToken);
    }

    public async Task<ApiResponse<LoginResponse>> RefreshTokenAsync(RefreshTokenRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));
        return await _httpClient.PostAsync<ApiResponse<LoginResponse>>("/auth/refresh", request, cancellationToken);
    }

    public async Task<ApiResponse<LoginResponse>> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        var request = new RefreshTokenRequest { RefreshToken = refreshToken };
        return await RefreshTokenAsync(request, cancellationToken);
    }

    public async Task<ApiResponse<object>> LogoutAsync(LogoutRequest? request = null, CancellationToken cancellationToken = default)
    {
        return await _httpClient.PostAsync<ApiResponse<object>>("/auth/logout", request, cancellationToken);
    }

    public async Task<ApiResponse<object>> ForgotPasswordAsync(ForgotPasswordRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));
        return await _httpClient.PostAsync<ApiResponse<object>>("/auth/forgot-password", request, cancellationToken);
    }

    public async Task<ApiResponse<object>> ForgotPasswordAsync(string email, CancellationToken cancellationToken = default)
    {
        var request = new ForgotPasswordRequest { Email = email };
        return await ForgotPasswordAsync(request, cancellationToken);
    }

    public async Task<ApiResponse<object>> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));
        return await _httpClient.PostAsync<ApiResponse<object>>("/auth/reset-password", request, cancellationToken);
    }

    public async Task<ApiResponse<object>> ResetPasswordAsync(string token, string newPassword, CancellationToken cancellationToken = default)
    {
        var request = new ResetPasswordRequest { Token = token, NewPassword = newPassword };
        return await ResetPasswordAsync(request, cancellationToken);
    }

    public async Task<ApiResponse<object>> ChangePasswordAsync(ChangePasswordRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));
        return await _httpClient.PostAsync<ApiResponse<object>>("/auth/change-password", request, cancellationToken);
    }

    public async Task<ApiResponse<object>> ChangePasswordAsync(string currentPassword, string newPassword, CancellationToken cancellationToken = default)
    {
        var request = new ChangePasswordRequest { CurrentPassword = currentPassword, NewPassword = newPassword };
        return await ChangePasswordAsync(request, cancellationToken);
    }
}