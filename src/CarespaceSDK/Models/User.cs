using System.Text.Json.Serialization;

namespace CarespaceSDK.Models;

public record User : EntityBase
{
    [JsonPropertyName("email")]
    public string Email { get; init; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("first_name")]
    public string FirstName { get; init; } = string.Empty;

    [JsonPropertyName("last_name")]
    public string LastName { get; init; } = string.Empty;

    [JsonPropertyName("role")]
    public UserRole Role { get; init; }

    [JsonPropertyName("is_active")]
    public bool IsActive { get; init; }

    [JsonPropertyName("last_login")]
    public DateTime? LastLogin { get; init; }

    [JsonPropertyName("profile")]
    public UserProfile? Profile { get; init; }
}

public record UserProfile
{
    [JsonPropertyName("phone")]
    public string? Phone { get; init; }

    [JsonPropertyName("date_of_birth")]
    public DateTime? DateOfBirth { get; init; }

    [JsonPropertyName("address")]
    public Address? Address { get; init; }

    [JsonPropertyName("emergency_contact")]
    public EmergencyContact? EmergencyContact { get; init; }

    [JsonPropertyName("medical_info")]
    public MedicalInfo? MedicalInfo { get; init; }
}

public record Address
{
    [JsonPropertyName("street")]
    public string Street { get; init; } = string.Empty;

    [JsonPropertyName("city")]
    public string City { get; init; } = string.Empty;

    [JsonPropertyName("state")]
    public string State { get; init; } = string.Empty;

    [JsonPropertyName("postal_code")]
    public string PostalCode { get; init; } = string.Empty;

    [JsonPropertyName("country")]
    public string Country { get; init; } = string.Empty;
}

public record EmergencyContact
{
    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("phone")]
    public string Phone { get; init; } = string.Empty;

    [JsonPropertyName("relationship")]
    public string Relationship { get; init; } = string.Empty;
}

public record MedicalInfo
{
    [JsonPropertyName("allergies")]
    public IReadOnlyList<string> Allergies { get; init; } = Array.Empty<string>();

    [JsonPropertyName("medications")]
    public IReadOnlyList<string> Medications { get; init; } = Array.Empty<string>();

    [JsonPropertyName("conditions")]
    public IReadOnlyList<string> Conditions { get; init; } = Array.Empty<string>();

    [JsonPropertyName("notes")]
    public string? Notes { get; init; }
}

public record CreateUserRequest
{
    [JsonPropertyName("email")]
    public string Email { get; init; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; init; } = string.Empty;

    [JsonPropertyName("first_name")]
    public string FirstName { get; init; } = string.Empty;

    [JsonPropertyName("last_name")]
    public string LastName { get; init; } = string.Empty;

    [JsonPropertyName("password")]
    public string Password { get; init; } = string.Empty;

    [JsonPropertyName("role")]
    public UserRole Role { get; init; }

    [JsonPropertyName("profile")]
    public UserProfile? Profile { get; init; }
}

public record UpdateUserRequest
{
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    [JsonPropertyName("first_name")]
    public string? FirstName { get; init; }

    [JsonPropertyName("last_name")]
    public string? LastName { get; init; }

    [JsonPropertyName("is_active")]
    public bool? IsActive { get; init; }

    [JsonPropertyName("profile")]
    public UserProfile? Profile { get; init; }
}