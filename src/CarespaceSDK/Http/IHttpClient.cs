namespace CarespaceSDK.Http;

public interface IHttpClient : IDisposable
{
    Task<T> GetAsync<T>(string endpoint, CancellationToken cancellationToken = default);
    Task<T> PostAsync<T>(string endpoint, object? data = null, CancellationToken cancellationToken = default);
    Task<T> PutAsync<T>(string endpoint, object? data = null, CancellationToken cancellationToken = default);
    Task<T> PatchAsync<T>(string endpoint, object? data = null, CancellationToken cancellationToken = default);
    Task<T> DeleteAsync<T>(string endpoint, CancellationToken cancellationToken = default);
    Task DeleteAsync(string endpoint, CancellationToken cancellationToken = default);
    
    void SetApiKey(string apiKey);
    void SetDefaultHeader(string name, string value);
    void RemoveDefaultHeader(string name);
}