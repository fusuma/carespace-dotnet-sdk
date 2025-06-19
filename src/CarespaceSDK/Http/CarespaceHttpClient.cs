using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using CarespaceSDK.Configuration;
using CarespaceSDK.Exceptions;

namespace CarespaceSDK.Http;

public class CarespaceHttpClient : IHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly CarespaceConfiguration _configuration;
    private readonly ILogger<CarespaceHttpClient>? _logger;
    private readonly JsonSerializerOptions _jsonOptions;
    private bool _disposed;

    public CarespaceHttpClient(HttpClient httpClient, CarespaceConfiguration configuration, ILogger<CarespaceHttpClient>? logger = null)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger;

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };

        ConfigureHttpClient();
    }

    private void ConfigureHttpClient()
    {
        _httpClient.BaseAddress = new Uri(_configuration.BaseUrl);
        _httpClient.Timeout = _configuration.Timeout;
        _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(_configuration.UserAgent);
        
        if (!string.IsNullOrEmpty(_configuration.ApiKey))
        {
            SetApiKey(_configuration.ApiKey);
        }

        foreach (var header in _configuration.DefaultHeaders)
        {
            _httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
        }
    }

    public void SetApiKey(string apiKey)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;
            return;
        }

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
    }

    public void SetDefaultHeader(string name, string value)
    {
        _httpClient.DefaultRequestHeaders.Remove(name);
        _httpClient.DefaultRequestHeaders.Add(name, value);
    }

    public void RemoveDefaultHeader(string name)
    {
        _httpClient.DefaultRequestHeaders.Remove(name);
    }

    public async Task<T> GetAsync<T>(string endpoint, CancellationToken cancellationToken = default)
    {
        return await ExecuteWithRetryAsync(async () =>
        {
            var response = await _httpClient.GetAsync(endpoint, cancellationToken);
            return await ProcessResponseAsync<T>(response);
        });
    }

    public async Task<T> PostAsync<T>(string endpoint, object? data = null, CancellationToken cancellationToken = default)
    {
        return await ExecuteWithRetryAsync(async () =>
        {
            var content = CreateJsonContent(data);
            var response = await _httpClient.PostAsync(endpoint, content, cancellationToken);
            return await ProcessResponseAsync<T>(response);
        });
    }

    public async Task<T> PutAsync<T>(string endpoint, object? data = null, CancellationToken cancellationToken = default)
    {
        return await ExecuteWithRetryAsync(async () =>
        {
            var content = CreateJsonContent(data);
            var response = await _httpClient.PutAsync(endpoint, content, cancellationToken);
            return await ProcessResponseAsync<T>(response);
        });
    }

    public async Task<T> PatchAsync<T>(string endpoint, object? data = null, CancellationToken cancellationToken = default)
    {
        return await ExecuteWithRetryAsync(async () =>
        {
            var content = CreateJsonContent(data);
            var response = await _httpClient.PatchAsync(endpoint, content, cancellationToken);
            return await ProcessResponseAsync<T>(response);
        });
    }

    public async Task<T> DeleteAsync<T>(string endpoint, CancellationToken cancellationToken = default)
    {
        return await ExecuteWithRetryAsync(async () =>
        {
            var response = await _httpClient.DeleteAsync(endpoint, cancellationToken);
            return await ProcessResponseAsync<T>(response);
        });
    }

    public async Task DeleteAsync(string endpoint, CancellationToken cancellationToken = default)
    {
        await ExecuteWithRetryAsync(async () =>
        {
            var response = await _httpClient.DeleteAsync(endpoint, cancellationToken);
            await ProcessResponseAsync(response);
            return true;
        });
    }

    private HttpContent? CreateJsonContent(object? data)
    {
        if (data == null) return null;

        var json = JsonSerializer.Serialize(data, _jsonOptions);
        return new StringContent(json, Encoding.UTF8, "application/json");
    }

    private async Task<T> ProcessResponseAsync<T>(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();

        if (response.IsSuccessStatusCode)
        {
            _logger?.LogDebug("HTTP {Method} {Uri} succeeded with status {StatusCode}", 
                response.RequestMessage?.Method, response.RequestMessage?.RequestUri, response.StatusCode);

            if (typeof(T) == typeof(string))
            {
                return (T)(object)content;
            }

            if (string.IsNullOrEmpty(content))
            {
                return default(T)!;
            }

            return JsonSerializer.Deserialize<T>(content, _jsonOptions) ?? default(T)!;
        }

        await HandleErrorResponseAsync(response, content);
        return default(T)!;
    }

    private async Task ProcessResponseAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            _logger?.LogDebug("HTTP {Method} {Uri} succeeded with status {StatusCode}", 
                response.RequestMessage?.Method, response.RequestMessage?.RequestUri, response.StatusCode);
            return;
        }

        var content = await response.Content.ReadAsStringAsync();
        await HandleErrorResponseAsync(response, content);
    }

    private async Task HandleErrorResponseAsync(HttpResponseMessage response, string content)
    {
        _logger?.LogError("HTTP {Method} {Uri} failed with status {StatusCode}. Response: {Content}", 
            response.RequestMessage?.Method, response.RequestMessage?.RequestUri, response.StatusCode, content);

        var exception = response.StatusCode switch
        {
            HttpStatusCode.Unauthorized => new CarespaceAuthenticationException("Authentication failed. Please check your API key."),
            HttpStatusCode.Forbidden => new CarespaceAuthorizationException("Access denied. You don't have permission to access this resource."),
            HttpStatusCode.NotFound => new CarespaceNotFoundException("The requested resource was not found."),
            HttpStatusCode.BadRequest => new CarespaceValidationException("Bad request. Please check your request data."),
            HttpStatusCode.TooManyRequests => CreateRateLimitException(response),
            HttpStatusCode.InternalServerError => new CarespaceServerException("Internal server error occurred."),
            _ => new CarespaceException($"HTTP request failed with status {response.StatusCode}", response.StatusCode)
        };

        throw exception;
    }

    private CarespaceRateLimitException CreateRateLimitException(HttpResponseMessage response)
    {
        TimeSpan? retryAfter = null;
        
        if (response.Headers.RetryAfter?.Delta.HasValue == true)
        {
            retryAfter = response.Headers.RetryAfter.Delta.Value;
        }
        else if (response.Headers.RetryAfter?.Date.HasValue == true)
        {
            retryAfter = response.Headers.RetryAfter.Date.Value - DateTimeOffset.UtcNow;
        }

        return new CarespaceRateLimitException("Rate limit exceeded. Please try again later.", retryAfter);
    }

    private async Task<T> ExecuteWithRetryAsync<T>(Func<Task<T>> operation)
    {
        if (!_configuration.EnableRetry)
        {
            return await operation();
        }

        var attempt = 0;
        while (true)
        {
            try
            {
                return await operation();
            }
            catch (Exception ex) when (ShouldRetry(ex, attempt))
            {
                attempt++;
                var delay = TimeSpan.FromMilliseconds(_configuration.RetryDelay.TotalMilliseconds * Math.Pow(2, attempt - 1));
                
                _logger?.LogWarning("Request failed (attempt {Attempt}/{MaxAttempts}). Retrying in {Delay}ms. Error: {Error}", 
                    attempt, _configuration.MaxRetryAttempts, delay.TotalMilliseconds, ex.Message);
                
                await Task.Delay(delay);
            }
        }
    }

    private bool ShouldRetry(Exception exception, int attempt)
    {
        if (attempt >= _configuration.MaxRetryAttempts)
        {
            return false;
        }

        return exception switch
        {
            HttpRequestException => true,
            TaskCanceledException => true,
            CarespaceException { StatusCode: HttpStatusCode.InternalServerError } => true,
            CarespaceException { StatusCode: HttpStatusCode.BadGateway } => true,
            CarespaceException { StatusCode: HttpStatusCode.ServiceUnavailable } => true,
            CarespaceException { StatusCode: HttpStatusCode.GatewayTimeout } => true,
            _ => false
        };
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