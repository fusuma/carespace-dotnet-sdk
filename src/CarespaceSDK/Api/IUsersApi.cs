using CarespaceSDK.Models;

namespace CarespaceSDK.Api;

public interface IUsersApi
{
    Task<PaginatedResponse<User>> GetUsersAsync(
        int page = 1, 
        int limit = 20, 
        string? search = null, 
        UserRole? role = null, 
        bool? isActive = null,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<User>> GetUserAsync(string userId, CancellationToken cancellationToken = default);
    Task<ApiResponse<User>> GetUserProfileAsync(CancellationToken cancellationToken = default);
    Task<ApiResponse<User>> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default);
    Task<ApiResponse<User>> UpdateUserAsync(string userId, UpdateUserRequest request, CancellationToken cancellationToken = default);
    Task<ApiResponse<User>> UpdateUserProfileAsync(UpdateUserRequest request, CancellationToken cancellationToken = default);
    Task DeleteUserAsync(string userId, CancellationToken cancellationToken = default);
    Task<ApiResponse<object>> DeactivateUserAsync(string userId, CancellationToken cancellationToken = default);
    Task<ApiResponse<object>> ActivateUserAsync(string userId, CancellationToken cancellationToken = default);
}