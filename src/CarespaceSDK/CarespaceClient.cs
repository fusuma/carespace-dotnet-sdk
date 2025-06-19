using Microsoft.Extensions.Logging;
using CarespaceSDK.Api;
using CarespaceSDK.Configuration;
using CarespaceSDK.Http;
using CarespaceSDK.Models;

namespace CarespaceSDK;

public class CarespaceClient : IDisposable
{
    private readonly IHttpClient _httpClient;
    private readonly CarespaceConfiguration _configuration;
    private bool _disposed;

    public IAuthApi Auth { get; }
    public IUsersApi Users { get; }
    public IClientsApi Clients { get; }
    public IProgramsApi Programs { get; }

    public CarespaceClient(CarespaceConfiguration? configuration = null, ILogger<CarespaceHttpClient>? logger = null)
    {
        _configuration = configuration ?? CarespaceConfiguration.ForDevelopment();
        _configuration.Validate();

        var httpClient = new HttpClient();
        _httpClient = new CarespaceHttpClient(httpClient, _configuration, logger);

        Auth = new AuthApi(_httpClient);
        Users = new UsersApi(_httpClient);
        Clients = new ClientsApi(_httpClient);
        Programs = new ProgramsApi(_httpClient);
    }

    public CarespaceClient(string apiKey, string? baseUrl = null, ILogger<CarespaceHttpClient>? logger = null)
        : this(new CarespaceConfiguration 
        { 
            ApiKey = apiKey, 
            BaseUrl = baseUrl ?? "https://api-dev.carespace.ai" 
        }, logger)
    {
    }

    public void SetApiKey(string apiKey)
    {
        _httpClient.SetApiKey(apiKey);
    }

    public void SetDefaultHeader(string name, string value)
    {
        _httpClient.SetDefaultHeader(name, value);
    }

    public void RemoveDefaultHeader(string name)
    {
        _httpClient.RemoveDefaultHeader(name);
    }

    public async Task<LoginResponse> LoginAndSetTokenAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var response = await Auth.LoginAsync(email, password, cancellationToken);
        
        if (response.Success && response.Data != null)
        {
            SetApiKey(response.Data.AccessToken);
            return response.Data;
        }

        throw new InvalidOperationException($"Login failed: {response.Error ?? response.Message}");
    }

    public async Task<PaginatedResponse<User>> QuickGetUsersAsync(int limit = 20, string? search = null, CancellationToken cancellationToken = default)
    {
        return await Users.GetUsersAsync(limit: limit, search: search, cancellationToken: cancellationToken);
    }

    public async Task<PaginatedResponse<Client>> QuickGetClientsAsync(int limit = 20, string? search = null, CancellationToken cancellationToken = default)
    {
        return await Clients.GetClientsAsync(limit: limit, search: search, cancellationToken: cancellationToken);
    }

    public async Task<PaginatedResponse<Program>> QuickGetProgramsAsync(int limit = 20, ProgramCategory? category = null, CancellationToken cancellationToken = default)
    {
        return await Programs.GetProgramsAsync(limit: limit, category: category, cancellationToken: cancellationToken);
    }

    public async Task<bool> HealthCheckAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (!string.IsNullOrEmpty(_configuration.ApiKey))
            {
                await Users.GetUserProfileAsync(cancellationToken);
                return true;
            }
            else
            {
                await Users.GetUsersAsync(limit: 1, cancellationToken: cancellationToken);
                return true;
            }
        }
        catch
        {
            return false;
        }
    }

    public static CarespaceClient CreateForDevelopment(string? apiKey = null, ILogger<CarespaceHttpClient>? logger = null)
    {
        return new CarespaceClient(CarespaceConfiguration.ForDevelopment(apiKey), logger);
    }

    public static CarespaceClient CreateForProduction(string apiKey, ILogger<CarespaceHttpClient>? logger = null)
    {
        return new CarespaceClient(CarespaceConfiguration.ForProduction(apiKey), logger);
    }

    public static CarespaceClient CreateForStaging(string apiKey, ILogger<CarespaceHttpClient>? logger = null)
    {
        return new CarespaceClient(CarespaceConfiguration.ForStaging(apiKey), logger);
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _httpClient?.Dispose();
            _disposed = true;
        }
    }
}