using System.Text.Json.Serialization;

namespace CarespaceSDK.Models;

public record ApiResponse<T>
{
    [JsonPropertyName("success")]
    public bool Success { get; init; }

    [JsonPropertyName("data")]
    public T? Data { get; init; }

    [JsonPropertyName("error")]
    public string? Error { get; init; }

    [JsonPropertyName("message")]
    public string? Message { get; init; }
}

public record PaginatedResponse<T> : ApiResponse<IReadOnlyList<T>>
{
    [JsonPropertyName("page")]
    public int Page { get; init; }

    [JsonPropertyName("limit")]
    public int Limit { get; init; }

    [JsonPropertyName("total")]
    public int Total { get; init; }

    [JsonPropertyName("total_pages")]
    public int TotalPages { get; init; }

    [JsonPropertyName("has_next")]
    public bool HasNext { get; init; }

    [JsonPropertyName("has_previous")]
    public bool HasPrevious { get; init; }
}

public record EntityBase
{
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;

    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; init; }

    [JsonPropertyName("updated_at")]
    public DateTime UpdatedAt { get; init; }
}

public enum UserRole
{
    [JsonPropertyName("admin")]
    Admin,
    
    [JsonPropertyName("provider")]
    Provider,
    
    [JsonPropertyName("patient")]
    Patient
}

public enum ProgramCategory
{
    [JsonPropertyName("rehabilitation")]
    Rehabilitation,
    
    [JsonPropertyName("fitness")]
    Fitness,
    
    [JsonPropertyName("therapy")]
    Therapy,
    
    [JsonPropertyName("wellness")]
    Wellness
}

public enum ProgramDifficulty
{
    [JsonPropertyName("beginner")]
    Beginner,
    
    [JsonPropertyName("intermediate")]
    Intermediate,
    
    [JsonPropertyName("advanced")]
    Advanced
}