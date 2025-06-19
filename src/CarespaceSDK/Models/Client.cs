using System.Text.Json.Serialization;

namespace CarespaceSDK.Models;

public record Client : EntityBase
{
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; init; } = string.Empty;

    [JsonPropertyName("phone")]
    public string? Phone { get; init; }

    [JsonPropertyName("date_of_birth")]
    public DateTime? DateOfBirth { get; init; }

    [JsonPropertyName("gender")]
    public string? Gender { get; init; }

    [JsonPropertyName("address")]
    public Address? Address { get; init; }

    [JsonPropertyName("emergency_contact")]
    public EmergencyContact? EmergencyContact { get; init; }

    [JsonPropertyName("medical_info")]
    public MedicalInfo? MedicalInfo { get; init; }

    [JsonPropertyName("assigned_programs")]
    public IReadOnlyList<string> AssignedPrograms { get; init; } = Array.Empty<string>();

    [JsonPropertyName("provider_id")]
    public string? ProviderId { get; init; }

    [JsonPropertyName("is_active")]
    public bool IsActive { get; init; }

    [JsonPropertyName("last_session")]
    public DateTime? LastSession { get; init; }

    [JsonPropertyName("total_sessions")]
    public int TotalSessions { get; init; }

    [JsonPropertyName("notes")]
    public string? Notes { get; init; }
}

public record CreateClientRequest
{
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("email")]
    public string Email { get; init; } = string.Empty;

    [JsonPropertyName("phone")]
    public string? Phone { get; init; }

    [JsonPropertyName("date_of_birth")]
    public DateTime? DateOfBirth { get; init; }

    [JsonPropertyName("gender")]
    public string? Gender { get; init; }

    [JsonPropertyName("address")]
    public Address? Address { get; init; }

    [JsonPropertyName("emergency_contact")]
    public EmergencyContact? EmergencyContact { get; init; }

    [JsonPropertyName("medical_info")]
    public MedicalInfo? MedicalInfo { get; init; }

    [JsonPropertyName("provider_id")]
    public string? ProviderId { get; init; }

    [JsonPropertyName("notes")]
    public string? Notes { get; init; }
}

public record UpdateClientRequest
{
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [JsonPropertyName("email")]
    public string? Email { get; init; }

    [JsonPropertyName("phone")]
    public string? Phone { get; init; }

    [JsonPropertyName("date_of_birth")]
    public DateTime? DateOfBirth { get; init; }

    [JsonPropertyName("gender")]
    public string? Gender { get; init; }

    [JsonPropertyName("address")]
    public Address? Address { get; init; }

    [JsonPropertyName("emergency_contact")]
    public EmergencyContact? EmergencyContact { get; init; }

    [JsonPropertyName("medical_info")]
    public MedicalInfo? MedicalInfo { get; init; }

    [JsonPropertyName("is_active")]
    public bool? IsActive { get; init; }

    [JsonPropertyName("notes")]
    public string? Notes { get; init; }
}

public record ClientStats
{
    [JsonPropertyName("total_sessions")]
    public int TotalSessions { get; init; }

    [JsonPropertyName("total_exercises")]
    public int TotalExercises { get; init; }

    [JsonPropertyName("total_time")]
    public int TotalTime { get; init; }

    [JsonPropertyName("last_session")]
    public DateTime? LastSession { get; init; }

    [JsonPropertyName("average_score")]
    public double? AverageScore { get; init; }

    [JsonPropertyName("completion_rate")]
    public double CompletionRate { get; init; }

    [JsonPropertyName("progress_trend")]
    public string ProgressTrend { get; init; } = string.Empty;
}