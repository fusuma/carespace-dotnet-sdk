using System.Text.Json.Serialization;

namespace CarespaceSDK.Models;

public record LoginRequest
{
    [JsonPropertyName("email")]
    public string Email { get; init; } = string.Empty;

    [JsonPropertyName("password")]
    public string Password { get; init; } = string.Empty;
}

public record LoginResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; init; } = string.Empty;

    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; init; } = string.Empty;

    [JsonPropertyName("token_type")]
    public string TokenType { get; init; } = "Bearer";

    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; init; }

    [JsonPropertyName("user")]
    public User User { get; init; } = new();
}

public record RefreshTokenRequest
{
    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; init; } = string.Empty;
}

public record LogoutRequest
{
    [JsonPropertyName("refresh_token")]
    public string? RefreshToken { get; init; }
}

public record ForgotPasswordRequest
{
    [JsonPropertyName("email")]
    public string Email { get; init; } = string.Empty;
}

public record ResetPasswordRequest
{
    [JsonPropertyName("token")]
    public string Token { get; init; } = string.Empty;

    [JsonPropertyName("new_password")]
    public string NewPassword { get; init; } = string.Empty;
}

public record ChangePasswordRequest
{
    [JsonPropertyName("current_password")]
    public string CurrentPassword { get; init; } = string.Empty;

    [JsonPropertyName("new_password")]
    public string NewPassword { get; init; } = string.Empty;
}