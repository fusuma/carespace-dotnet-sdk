using Microsoft.Extensions.Logging;

namespace CarespaceSDK.Configuration;

public class CarespaceConfiguration
{
    public string BaseUrl { get; set; } = "https://api-dev.carespace.ai";
    public string? ApiKey { get; set; }
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
    public int MaxRetryAttempts { get; set; } = 3;
    public TimeSpan RetryDelay { get; set; } = TimeSpan.FromSeconds(1);
    public string UserAgent { get; set; } = "CarespaceSDK/1.0.0 (.NET)";
    public LogLevel LogLevel { get; set; } = LogLevel.Information;
    public bool EnableRetry { get; set; } = true;
    public bool EnableLogging { get; set; } = true;
    public IDictionary<string, string> DefaultHeaders { get; set; } = new Dictionary<string, string>();

    public static CarespaceConfiguration ForDevelopment(string? apiKey = null)
    {
        return new CarespaceConfiguration
        {
            BaseUrl = "https://api-dev.carespace.ai",
            ApiKey = apiKey,
            LogLevel = LogLevel.Debug
        };
    }

    public static CarespaceConfiguration ForProduction(string apiKey)
    {
        return new CarespaceConfiguration
        {
            BaseUrl = "https://api.carespace.ai",
            ApiKey = apiKey,
            LogLevel = LogLevel.Warning
        };
    }

    public static CarespaceConfiguration ForStaging(string apiKey)
    {
        return new CarespaceConfiguration
        {
            BaseUrl = "https://api-staging.carespace.ai",
            ApiKey = apiKey,
            LogLevel = LogLevel.Information
        };
    }

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(BaseUrl))
        {
            throw new ArgumentException("BaseUrl is required", nameof(BaseUrl));
        }

        if (!Uri.TryCreate(BaseUrl, UriKind.Absolute, out _))
        {
            throw new ArgumentException("BaseUrl must be a valid URI", nameof(BaseUrl));
        }

        if (Timeout <= TimeSpan.Zero)
        {
            throw new ArgumentException("Timeout must be greater than zero", nameof(Timeout));
        }

        if (MaxRetryAttempts < 0)
        {
            throw new ArgumentException("MaxRetryAttempts cannot be negative", nameof(MaxRetryAttempts));
        }

        if (RetryDelay < TimeSpan.Zero)
        {
            throw new ArgumentException("RetryDelay cannot be negative", nameof(RetryDelay));
        }
    }
}