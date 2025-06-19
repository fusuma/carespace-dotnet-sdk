using CarespaceSDK.Http;
using CarespaceSDK.Models;

namespace CarespaceSDK.Api;

public class UsersApi : IUsersApi
{
    private readonly IHttpClient _httpClient;

    public UsersApi(IHttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<PaginatedResponse<User>> GetUsersAsync(
        int page = 1, 
        int limit = 20, 
        string? search = null, 
        UserRole? role = null, 
        bool? isActive = null,
        CancellationToken cancellationToken = default)
    {
        var queryParams = new List<string>();
        queryParams.Add($"page={page}");
        queryParams.Add($"limit={limit}");
        
        if (!string.IsNullOrEmpty(search))
            queryParams.Add($"search={Uri.EscapeDataString(search)}");
        
        if (role.HasValue)
            queryParams.Add($"role={role.Value}");
        
        if (isActive.HasValue)
            queryParams.Add($"is_active={isActive.Value}");

        var query = string.Join("&", queryParams);
        var endpoint = $"/users?{query}";

        return await _httpClient.GetAsync<PaginatedResponse<User>>(endpoint, cancellationToken);
    }

    public async Task<ApiResponse<User>> GetUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("User ID is required", nameof(userId));
        return await _httpClient.GetAsync<ApiResponse<User>>($"/users/{userId}", cancellationToken);
    }

    public async Task<ApiResponse<User>> GetUserProfileAsync(CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetAsync<ApiResponse<User>>("/users/profile", cancellationToken);
    }

    public async Task<ApiResponse<User>> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));
        return await _httpClient.PostAsync<ApiResponse<User>>("/users", request, cancellationToken);
    }

    public async Task<ApiResponse<User>> UpdateUserAsync(string userId, UpdateUserRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("User ID is required", nameof(userId));
        if (request == null) throw new ArgumentNullException(nameof(request));
        return await _httpClient.PutAsync<ApiResponse<User>>($"/users/{userId}", request, cancellationToken);
    }

    public async Task<ApiResponse<User>> UpdateUserProfileAsync(UpdateUserRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));
        return await _httpClient.PutAsync<ApiResponse<User>>("/users/profile", request, cancellationToken);
    }

    public async Task DeleteUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("User ID is required", nameof(userId));
        await _httpClient.DeleteAsync($"/users/{userId}", cancellationToken);
    }

    public async Task<ApiResponse<object>> DeactivateUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("User ID is required", nameof(userId));
        return await _httpClient.PostAsync<ApiResponse<object>>($"/users/{userId}/deactivate", null, cancellationToken);
    }

    public async Task<ApiResponse<object>> ActivateUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userId)) throw new ArgumentException("User ID is required", nameof(userId));
        return await _httpClient.PostAsync<ApiResponse<object>>($"/users/{userId}/activate", null, cancellationToken);
    }
}