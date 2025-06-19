using System.Text.Json.Serialization;

namespace CarespaceSDK.Models;

public record Program : EntityBase
{
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("description")]
    public string? Description { get; init; }

    [JsonPropertyName("category")]
    public ProgramCategory Category { get; init; }

    [JsonPropertyName("difficulty")]
    public ProgramDifficulty Difficulty { get; init; }

    [JsonPropertyName("duration")]
    public int Duration { get; init; }

    [JsonPropertyName("exercises")]
    public IReadOnlyList<Exercise> Exercises { get; init; } = Array.Empty<Exercise>();

    [JsonPropertyName("is_template")]
    public bool IsTemplate { get; init; }

    [JsonPropertyName("is_public")]
    public bool IsPublic { get; init; }

    [JsonPropertyName("creator_id")]
    public string CreatorId { get; init; } = string.Empty;

    [JsonPropertyName("tags")]
    public IReadOnlyList<string> Tags { get; init; } = Array.Empty<string>();

    [JsonPropertyName("thumbnail_url")]
    public string? ThumbnailUrl { get; init; }
}

public record Exercise : EntityBase
{
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("description")]
    public string? Description { get; init; }

    [JsonPropertyName("instructions")]
    public string? Instructions { get; init; }

    [JsonPropertyName("duration")]
    public int? Duration { get; init; }

    [JsonPropertyName("repetitions")]
    public int? Repetitions { get; init; }

    [JsonPropertyName("sets")]
    public int? Sets { get; init; }

    [JsonPropertyName("rest_time")]
    public int? RestTime { get; init; }

    [JsonPropertyName("body_parts")]
    public IReadOnlyList<string> BodyParts { get; init; } = Array.Empty<string>();

    [JsonPropertyName("equipment")]
    public IReadOnlyList<string> Equipment { get; init; } = Array.Empty<string>();

    [JsonPropertyName("media_url")]
    public string? MediaUrl { get; init; }

    [JsonPropertyName("thumbnail_url")]
    public string? ThumbnailUrl { get; init; }

    [JsonPropertyName("order")]
    public int Order { get; init; }
}

public record CreateProgramRequest
{
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("description")]
    public string? Description { get; init; }

    [JsonPropertyName("category")]
    public ProgramCategory Category { get; init; }

    [JsonPropertyName("difficulty")]
    public ProgramDifficulty Difficulty { get; init; }

    [JsonPropertyName("duration")]
    public int Duration { get; init; }

    [JsonPropertyName("is_template")]
    public bool IsTemplate { get; init; }

    [JsonPropertyName("is_public")]
    public bool IsPublic { get; init; }

    [JsonPropertyName("tags")]
    public IReadOnlyList<string>? Tags { get; init; }

    [JsonPropertyName("thumbnail_url")]
    public string? ThumbnailUrl { get; init; }
}

public record UpdateProgramRequest
{
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [JsonPropertyName("description")]
    public string? Description { get; init; }

    [JsonPropertyName("category")]
    public ProgramCategory? Category { get; init; }

    [JsonPropertyName("difficulty")]
    public ProgramDifficulty? Difficulty { get; init; }

    [JsonPropertyName("duration")]
    public int? Duration { get; init; }

    [JsonPropertyName("is_template")]
    public bool? IsTemplate { get; init; }

    [JsonPropertyName("is_public")]
    public bool? IsPublic { get; init; }

    [JsonPropertyName("tags")]
    public IReadOnlyList<string>? Tags { get; init; }

    [JsonPropertyName("thumbnail_url")]
    public string? ThumbnailUrl { get; init; }
}

public record CreateExerciseRequest
{
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("description")]
    public string? Description { get; init; }

    [JsonPropertyName("instructions")]
    public string? Instructions { get; init; }

    [JsonPropertyName("duration")]
    public int? Duration { get; init; }

    [JsonPropertyName("repetitions")]
    public int? Repetitions { get; init; }

    [JsonPropertyName("sets")]
    public int? Sets { get; init; }

    [JsonPropertyName("rest_time")]
    public int? RestTime { get; init; }

    [JsonPropertyName("body_parts")]
    public IReadOnlyList<string>? BodyParts { get; init; }

    [JsonPropertyName("equipment")]
    public IReadOnlyList<string>? Equipment { get; init; }

    [JsonPropertyName("media_url")]
    public string? MediaUrl { get; init; }

    [JsonPropertyName("thumbnail_url")]
    public string? ThumbnailUrl { get; init; }

    [JsonPropertyName("order")]
    public int Order { get; init; }
}