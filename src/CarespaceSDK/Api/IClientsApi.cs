using CarespaceSDK.Models;

namespace CarespaceSDK.Api;

public interface IClientsApi
{
    Task<PaginatedResponse<Client>> GetClientsAsync(
        int page = 1, 
        int limit = 20, 
        string? search = null, 
        string? providerId = null, 
        bool? isActive = null,
        CancellationToken cancellationToken = default);

    Task<ApiResponse<Client>> GetClientAsync(string clientId, CancellationToken cancellationToken = default);
    Task<ApiResponse<Client>> CreateClientAsync(CreateClientRequest request, CancellationToken cancellationToken = default);
    Task<ApiResponse<Client>> UpdateClientAsync(string clientId, UpdateClientRequest request, CancellationToken cancellationToken = default);
    Task DeleteClientAsync(string clientId, CancellationToken cancellationToken = default);
    
    Task<ApiResponse<ClientStats>> GetClientStatsAsync(string clientId, CancellationToken cancellationToken = default);
    Task<PaginatedResponse<Program>> GetClientProgramsAsync(string clientId, int page = 1, int limit = 20, CancellationToken cancellationToken = default);
    Task<ApiResponse<object>> AssignProgramAsync(string clientId, string programId, CancellationToken cancellationToken = default);
    Task<ApiResponse<object>> UnassignProgramAsync(string clientId, string programId, CancellationToken cancellationToken = default);
    
    Task<ApiResponse<object>> DeactivateClientAsync(string clientId, CancellationToken cancellationToken = default);
    Task<ApiResponse<object>> ActivateClientAsync(string clientId, CancellationToken cancellationToken = default);
}