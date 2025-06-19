using CarespaceSDK.Http;
using CarespaceSDK.Models;

namespace CarespaceSDK.Api;

public class ClientsApi : IClientsApi
{
    private readonly IHttpClient _httpClient;

    public ClientsApi(IHttpClient httpClient)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<PaginatedResponse<Client>> GetClientsAsync(
        int page = 1, 
        int limit = 20, 
        string? search = null, 
        string? providerId = null, 
        bool? isActive = null,
        CancellationToken cancellationToken = default)
    {
        var queryParams = new List<string>();
        queryParams.Add($"page={page}");
        queryParams.Add($"limit={limit}");
        
        if (!string.IsNullOrEmpty(search))
            queryParams.Add($"search={Uri.EscapeDataString(search)}");
        
        if (!string.IsNullOrEmpty(providerId))
            queryParams.Add($"provider_id={providerId}");
        
        if (isActive.HasValue)
            queryParams.Add($"is_active={isActive.Value}");

        var query = string.Join("&", queryParams);
        var endpoint = $"/clients?{query}";

        return await _httpClient.GetAsync<PaginatedResponse<Client>>(endpoint, cancellationToken);
    }

    public async Task<ApiResponse<Client>> GetClientAsync(string clientId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(clientId)) throw new ArgumentException("Client ID is required", nameof(clientId));
        return await _httpClient.GetAsync<ApiResponse<Client>>($"/clients/{clientId}", cancellationToken);
    }

    public async Task<ApiResponse<Client>> CreateClientAsync(CreateClientRequest request, CancellationToken cancellationToken = default)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));
        return await _httpClient.PostAsync<ApiResponse<Client>>("/clients", request, cancellationToken);
    }

    public async Task<ApiResponse<Client>> UpdateClientAsync(string clientId, UpdateClientRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(clientId)) throw new ArgumentException("Client ID is required", nameof(clientId));
        if (request == null) throw new ArgumentNullException(nameof(request));
        return await _httpClient.PutAsync<ApiResponse<Client>>($"/clients/{clientId}", request, cancellationToken);
    }

    public async Task DeleteClientAsync(string clientId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(clientId)) throw new ArgumentException("Client ID is required", nameof(clientId));
        await _httpClient.DeleteAsync($"/clients/{clientId}", cancellationToken);
    }

    public async Task<ApiResponse<ClientStats>> GetClientStatsAsync(string clientId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(clientId)) throw new ArgumentException("Client ID is required", nameof(clientId));
        return await _httpClient.GetAsync<ApiResponse<ClientStats>>($"/clients/{clientId}/stats", cancellationToken);
    }

    public async Task<PaginatedResponse<Program>> GetClientProgramsAsync(string clientId, int page = 1, int limit = 20, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(clientId)) throw new ArgumentException("Client ID is required", nameof(clientId));
        
        var queryParams = new List<string>();
        queryParams.Add($"page={page}");
        queryParams.Add($"limit={limit}");
        var query = string.Join("&", queryParams);
        
        return await _httpClient.GetAsync<PaginatedResponse<Program>>($"/clients/{clientId}/programs?{query}", cancellationToken);
    }

    public async Task<ApiResponse<object>> AssignProgramAsync(string clientId, string programId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(clientId)) throw new ArgumentException("Client ID is required", nameof(clientId));
        if (string.IsNullOrWhiteSpace(programId)) throw new ArgumentException("Program ID is required", nameof(programId));
        
        var request = new { program_id = programId };
        return await _httpClient.PostAsync<ApiResponse<object>>($"/clients/{clientId}/programs", request, cancellationToken);
    }

    public async Task<ApiResponse<object>> UnassignProgramAsync(string clientId, string programId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(clientId)) throw new ArgumentException("Client ID is required", nameof(clientId));
        if (string.IsNullOrWhiteSpace(programId)) throw new ArgumentException("Program ID is required", nameof(programId));
        
        return await _httpClient.DeleteAsync<ApiResponse<object>>($"/clients/{clientId}/programs/{programId}", cancellationToken);
    }

    public async Task<ApiResponse<object>> DeactivateClientAsync(string clientId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(clientId)) throw new ArgumentException("Client ID is required", nameof(clientId));
        return await _httpClient.PostAsync<ApiResponse<object>>($"/clients/{clientId}/deactivate", null, cancellationToken);
    }

    public async Task<ApiResponse<object>> ActivateClientAsync(string clientId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(clientId)) throw new ArgumentException("Client ID is required", nameof(clientId));
        return await _httpClient.PostAsync<ApiResponse<object>>($"/clients/{clientId}/activate", null, cancellationToken);
    }
}