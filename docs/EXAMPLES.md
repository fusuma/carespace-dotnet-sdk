# Carespace .NET SDK - Examples and Use Cases

This document provides comprehensive examples and common use cases for the Carespace .NET SDK.

## Table of Contents

- [Basic Examples](#basic-examples)
- [Authentication Examples](#authentication-examples)
- [User Management Examples](#user-management-examples)
- [Client Management Examples](#client-management-examples)
- [Program Management Examples](#program-management-examples)
- [Error Handling Examples](#error-handling-examples)
- [Advanced Use Cases](#advanced-use-cases)
- [Integration Patterns](#integration-patterns)
- [Best Practices](#best-practices)

## Basic Examples

### Quick Start - Console Application

```csharp
using CarespaceSDK;
using CarespaceSDK.Models;

class Program
{
    static async Task Main(string[] args)
    {
        // Create client for development environment
        using var client = CarespaceClient.CreateForDevelopment("your-api-key");

        // Perform health check
        var isHealthy = await client.HealthCheckAsync();
        Console.WriteLine($"API Status: {(isHealthy ? "Healthy" : "Unhealthy")}");

        // Get users
        var users = await client.QuickGetUsersAsync(limit: 10);
        Console.WriteLine($"Found {users.Data?.Count ?? 0} users");

        // Get clients
        var clients = await client.QuickGetClientsAsync(limit: 5);
        Console.WriteLine($"Found {clients.Data?.Count ?? 0} clients");
    }
}
```

### Basic Web API Integration

```csharp
using CarespaceSDK.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Register Carespace SDK
builder.Services.AddCarespaceSDKForDevelopment("your-api-key");

var app = builder.Build();

// Health check endpoint
app.MapGet("/health", async (CarespaceClient carespace) =>
{
    var isHealthy = await carespace.HealthCheckAsync();
    return Results.Ok(new { healthy = isHealthy });
});

app.Run();
```

## Authentication Examples

### Basic Login

```csharp
using var client = CarespaceClient.CreateForDevelopment("your-api-key");

try
{
    var loginResponse = await client.Auth.LoginAsync("user@example.com", "password");
    
    if (loginResponse.Success && loginResponse.Data != null)
    {
        Console.WriteLine($"Login successful!");
        Console.WriteLine($"User: {loginResponse.Data.User.Name}");
        Console.WriteLine($"Access Token: {loginResponse.Data.AccessToken}");
        Console.WriteLine($"Expires At: {loginResponse.Data.ExpiresAt}");
        
        // Store refresh token for later use
        var refreshToken = loginResponse.Data.RefreshToken;
    }
}
catch (CarespaceAuthenticationException ex)
{
    Console.WriteLine($"Login failed: {ex.Message}");
}
```

### Token Refresh

```csharp
public class TokenManager
{
    private readonly CarespaceClient _client;
    private string? _accessToken;
    private string? _refreshToken;
    private DateTime _tokenExpiry;

    public TokenManager(CarespaceClient client)
    {
        _client = client;
    }

    public async Task<string> GetValidTokenAsync()
    {
        // Check if current token is still valid
        if (_accessToken != null && DateTime.UtcNow < _tokenExpiry.AddMinutes(-5))
        {
            return _accessToken;
        }

        // Refresh token if available
        if (_refreshToken != null)
        {
            var refreshResponse = await _client.Auth.RefreshTokenAsync(_refreshToken);
            if (refreshResponse.Success && refreshResponse.Data != null)
            {
                _accessToken = refreshResponse.Data.AccessToken;
                _refreshToken = refreshResponse.Data.RefreshToken;
                _tokenExpiry = refreshResponse.Data.ExpiresAt;
                
                return _accessToken;
            }
        }

        throw new InvalidOperationException("Unable to obtain valid token");
    }
}
```

### Password Change

```csharp
public async Task ChangeUserPasswordAsync(string currentPassword, string newPassword)
{
    try
    {
        var response = await client.Auth.ChangePasswordAsync(currentPassword, newPassword);
        
        if (response.Success)
        {
            Console.WriteLine("Password changed successfully");
        }
    }
    catch (CarespaceValidationException ex)
    {
        Console.WriteLine("Password validation failed:");
        foreach (var error in ex.ValidationErrors)
        {
            Console.WriteLine($"- {error.Key}: {string.Join(", ", error.Value)}");
        }
    }
    catch (CarespaceAuthenticationException ex)
    {
        Console.WriteLine($"Current password is incorrect: {ex.Message}");
    }
}
```

## User Management Examples

### Creating Users with Different Roles

```csharp
public async Task CreateUsersExampleAsync()
{
    // Create a patient
    var patient = new CreateUserRequest
    {
        Email = "patient@example.com",
        Name = "John Patient",
        FirstName = "John",
        LastName = "Patient",
        Password = "SecurePass123!",
        Role = UserRole.Patient,
        Profile = new UserProfile
        {
            Phone = "+1-555-0123",
            DateOfBirth = DateTime.Parse("1985-03-15"),
            Address = new Address
            {
                Street = "123 Main St",
                City = "Anytown",
                State = "CA",
                PostalCode = "12345",
                Country = "USA"
            }
        }
    };

    var patientResponse = await client.Users.CreateUserAsync(patient);

    // Create a healthcare provider
    var provider = new CreateUserRequest
    {
        Email = "provider@example.com",
        Name = "Dr. Sarah Provider",
        FirstName = "Sarah",
        LastName = "Provider",
        Password = "SecurePass123!",
        Role = UserRole.Provider,
        Profile = new UserProfile
        {
            Phone = "+1-555-0456",
            Credentials = new[] { "MD", "PT" },
            Specializations = new[] { "Physical Therapy", "Rehabilitation" }
        }
    };

    var providerResponse = await client.Users.CreateUserAsync(provider);
}
```

### User Search and Filtering

```csharp
public async Task SearchUsersExampleAsync()
{
    // Search by name or email
    var searchResults = await client.Users.GetUsersAsync(
        page: 1,
        limit: 20,
        search: "john",
        role: null,
        isActive: true
    );

    // Filter by role
    var providers = await client.Users.GetUsersAsync(
        page: 1,
        limit: 50,
        role: UserRole.Provider
    );

    // Get inactive users
    var inactiveUsers = await client.Users.GetUsersAsync(
        page: 1,
        limit: 10,
        isActive: false
    );

    Console.WriteLine($"Search results: {searchResults.Data?.Count ?? 0}");
    Console.WriteLine($"Providers: {providers.Data?.Count ?? 0}");
    Console.WriteLine($"Inactive users: {inactiveUsers.Data?.Count ?? 0}");
}
```

### Bulk User Operations

```csharp
public async Task BulkUserOperationsAsync()
{
    var userUpdates = new[]
    {
        new { UserId = "user1", Name = "Updated Name 1" },
        new { UserId = "user2", Name = "Updated Name 2" },
        new { UserId = "user3", Name = "Updated Name 3" }
    };

    var tasks = userUpdates.Select(async update =>
    {
        var updateRequest = new UpdateUserRequest { Name = update.Name };
        return await client.Users.UpdateUserAsync(update.UserId, updateRequest);
    });

    var results = await Task.WhenAll(tasks);
    
    var successCount = results.Count(r => r.Success);
    Console.WriteLine($"Successfully updated {successCount} out of {results.Length} users");
}
```

## Client Management Examples

### Creating Clients with Medical Information

```csharp
public async Task CreateClientWithMedicalInfoAsync()
{
    var client = new CreateClientRequest
    {
        Name = "Jane Doe",
        Email = "jane.doe@example.com",
        Phone = "+1-555-0789",
        DateOfBirth = DateTime.Parse("1990-07-20"),
        Gender = "Female",
        EmergencyContact = new EmergencyContact
        {
            Name = "John Doe",
            Relationship = "Spouse",
            Phone = "+1-555-0790"
        },
        MedicalInfo = new MedicalInfo
        {
            Allergies = new[] { "Peanuts", "Shellfish", "Latex" },
            Conditions = new[] { "Hypertension", "Type 2 Diabetes" },
            Medications = new[] { "Lisinopril 10mg", "Metformin 500mg" },
            Notes = "Patient is recovering from ACL surgery. Cleared for physical therapy.",
            BloodType = "O+",
            Height = 165, // cm
            Weight = 70   // kg
        },
        InsuranceInfo = new InsuranceInfo
        {
            Provider = "Blue Cross Blue Shield",
            PolicyNumber = "BC123456789",
            GroupNumber = "GRP001"
        }
    };

    var response = await client.Clients.CreateClientAsync(client);
    
    if (response.Success && response.Data != null)
    {
        Console.WriteLine($"Created client: {response.Data.Id}");
        Console.WriteLine($"Name: {response.Data.Name}");
        Console.WriteLine($"Allergies: {string.Join(", ", response.Data.MedicalInfo?.Allergies ?? Array.Empty<string>())}");
    }
}
```

### Client Progress Tracking

```csharp
public async Task TrackClientProgressAsync(string clientId)
{
    // Get client statistics
    var statsResponse = await client.Clients.GetClientStatsAsync(clientId);
    
    if (statsResponse.Success && statsResponse.Data != null)
    {
        var stats = statsResponse.Data;
        
        Console.WriteLine($"Client Progress Report:");
        Console.WriteLine($"Total Sessions: {stats.TotalSessions}");
        Console.WriteLine($"Completed Sessions: {stats.CompletedSessions}");
        Console.WriteLine($"Completion Rate: {stats.CompletionRate:P2}");
        Console.WriteLine($"Average Session Duration: {stats.AverageSessionDuration} minutes");
        Console.WriteLine($"Last Activity: {stats.LastActivityDate:yyyy-MM-dd}");
        
        // Get assigned programs
        var programsResponse = await client.Clients.GetClientProgramsAsync(clientId);
        if (programsResponse.Success && programsResponse.Data != null)
        {
            Console.WriteLine($"\nAssigned Programs: {programsResponse.Data.Count}");
            foreach (var program in programsResponse.Data)
            {
                Console.WriteLine($"- {program.Name} ({program.Category})");
            }
        }
    }
}
```

### Client Search and Management

```csharp
public async Task ManageClientsAsync()
{
    // Search clients by name
    var searchResults = await client.Clients.GetClientsAsync(
        page: 1,
        limit: 20,
        search: "doe"
    );

    Console.WriteLine($"Found {searchResults.Data?.Count ?? 0} clients matching 'doe'");

    // Get clients for a specific provider
    var providerClients = await client.Clients.GetClientsAsync(
        page: 1,
        limit: 50,
        providerId: "provider-123"
    );

    // Update client information
    if (searchResults.Data?.Any() == true)
    {
        var firstClient = searchResults.Data.First();
        var updateRequest = new UpdateClientRequest
        {
            Phone = "+1-555-NEW-PHONE",
            MedicalInfo = new MedicalInfo
            {
                Allergies = firstClient.MedicalInfo?.Allergies?.Concat(new[] { "New Allergy" }).ToArray(),
                Notes = $"{firstClient.MedicalInfo?.Notes} Updated on {DateTime.Now:yyyy-MM-dd}"
            }
        };

        var updateResponse = await client.Clients.UpdateClientAsync(firstClient.Id, updateRequest);
        Console.WriteLine($"Client update: {(updateResponse.Success ? "Success" : "Failed")}");
    }
}
```

## Program Management Examples

### Creating Comprehensive Rehabilitation Programs

```csharp
public async Task CreateRehabilitationProgramAsync()
{
    // Create the main program
    var program = new CreateProgramRequest
    {
        Name = "Post-Surgical Knee Rehabilitation",
        Description = "Comprehensive 8-week rehabilitation program for patients recovering from knee surgery",
        Category = ProgramCategory.Rehabilitation,
        Difficulty = ProgramDifficulty.Beginner,
        Duration = 56, // 8 weeks
        IsTemplate = true,
        IsPublic = true,
        Tags = new[] { "knee", "surgery", "rehabilitation", "post-op", "beginner" },
        Objectives = new[]
        {
            "Restore range of motion",
            "Strengthen quadriceps and hamstrings",
            "Improve balance and proprioception",
            "Return to daily activities"
        }
    };

    var programResponse = await client.Programs.CreateProgramAsync(program);
    
    if (programResponse.Success && programResponse.Data != null)
    {
        var programId = programResponse.Data.Id;
        Console.WriteLine($"Created program: {programId}");

        // Add exercises to the program
        await AddExercisesToProgramAsync(programId);
    }
}

private async Task AddExercisesToProgramAsync(string programId)
{
    var exercises = new[]
    {
        new CreateExerciseRequest
        {
            Name = "Ankle Pumps",
            Description = "Simple ankle movement to promote circulation",
            Instructions = "Lying down, flex and point your foot repeatedly",
            Duration = 120, // 2 minutes
            Repetitions = 20,
            Sets = 3,
            RestTime = 30,
            BodyParts = new[] { "Ankle", "Calf" },
            Equipment = new[] { "None" },
            Order = 1,
            VideoUrl = "https://example.com/videos/ankle-pumps",
            ImageUrl = "https://example.com/images/ankle-pumps.jpg"
        },
        new CreateExerciseRequest
        {
            Name = "Quad Sets",
            Description = "Isometric quadriceps strengthening",
            Instructions = "Lying down, tighten your thigh muscle and hold",
            Duration = 10, // 10 seconds hold
            Repetitions = 10,
            Sets = 3,
            RestTime = 30,
            BodyParts = new[] { "Quadriceps", "Knee" },
            Equipment = new[] { "Exercise mat" },
            Order = 2,
            Difficulty = ExerciseDifficulty.Easy
        },
        new CreateExerciseRequest
        {
            Name = "Heel Slides",
            Description = "Progressive knee flexion exercise",
            Instructions = "Lying down, slowly slide your heel toward your buttocks",
            Duration = 60,
            Repetitions = 15,
            Sets = 2,
            RestTime = 45,
            BodyParts = new[] { "Knee", "Hamstring" },
            Equipment = new[] { "Exercise mat", "Towel" },
            Order = 3,
            Precautions = new[] { "Stop if you feel sharp pain", "Don't force the movement" }
        }
    };

    foreach (var exercise in exercises)
    {
        var exerciseResponse = await client.Programs.AddExerciseToProgramAsync(programId, exercise);
        if (exerciseResponse.Success)
        {
            Console.WriteLine($"Added exercise: {exercise.Name}");
        }
    }
}
```

### Program Assignment and Tracking

```csharp
public async Task ManageProgramAssignmentsAsync()
{
    var clientId = "client-123";
    var programId = "program-456";

    // Assign program to client
    var assignResponse = await client.Clients.AssignProgramAsync(clientId, programId);
    
    if (assignResponse.Success)
    {
        Console.WriteLine("Program assigned successfully");

        // Track program progress
        var progressResponse = await client.Clients.GetProgramProgressAsync(clientId, programId);
        
        if (progressResponse.Success && progressResponse.Data != null)
        {
            var progress = progressResponse.Data;
            Console.WriteLine($"Program Progress:");
            Console.WriteLine($"- Started: {progress.StartDate:yyyy-MM-dd}");
            Console.WriteLine($"- Completion: {progress.CompletionPercentage:P2}");
            Console.WriteLine($"- Current Phase: {progress.CurrentPhase}");
            Console.WriteLine($"- Next Session: {progress.NextSessionDate:yyyy-MM-dd}");
        }

        // Get session history
        var sessionsResponse = await client.Clients.GetProgramSessionsAsync(clientId, programId);
        
        if (sessionsResponse.Success && sessionsResponse.Data != null)
        {
            Console.WriteLine($"\nRecent Sessions: {sessionsResponse.Data.Count}");
            foreach (var session in sessionsResponse.Data.Take(5))
            {
                Console.WriteLine($"- {session.Date:MM/dd} - Duration: {session.Duration}min - Completed: {session.CompletedExercises}/{session.TotalExercises}");
            }
        }
    }
}
```

### Custom Program Creation Workflow

```csharp
public class ProgramBuilder
{
    private readonly CarespaceClient _client;
    private readonly CreateProgramRequest _program;
    private readonly List<CreateExerciseRequest> _exercises;

    public ProgramBuilder(CarespaceClient client, string name)
    {
        _client = client;
        _program = new CreateProgramRequest { Name = name };
        _exercises = new List<CreateExerciseRequest>();
    }

    public ProgramBuilder WithDescription(string description)
    {
        _program.Description = description;
        return this;
    }

    public ProgramBuilder WithCategory(ProgramCategory category)
    {
        _program.Category = category;
        return this;
    }

    public ProgramBuilder WithDifficulty(ProgramDifficulty difficulty)
    {
        _program.Difficulty = difficulty;
        return this;
    }

    public ProgramBuilder WithDuration(int days)
    {
        _program.Duration = days;
        return this;
    }

    public ProgramBuilder AddExercise(string name, string description, int duration, int reps, int sets)
    {
        _exercises.Add(new CreateExerciseRequest
        {
            Name = name,
            Description = description,
            Duration = duration,
            Repetitions = reps,
            Sets = sets,
            Order = _exercises.Count + 1
        });
        return this;
    }

    public async Task<Program?> BuildAsync()
    {
        // Create the program
        var programResponse = await _client.Programs.CreateProgramAsync(_program);
        
        if (!programResponse.Success || programResponse.Data == null)
        {
            return null;
        }

        var program = programResponse.Data;

        // Add exercises
        foreach (var exercise in _exercises)
        {
            await _client.Programs.AddExerciseToProgramAsync(program.Id, exercise);
        }

        return program;
    }
}

// Usage
public async Task UseCustomProgramBuilderAsync()
{
    var program = await new ProgramBuilder(client, "Custom Shoulder Program")
        .WithDescription("Targeted shoulder rehabilitation")
        .WithCategory(ProgramCategory.Rehabilitation)
        .WithDifficulty(ProgramDifficulty.Intermediate)
        .WithDuration(42) // 6 weeks
        .AddExercise("Pendulum Swings", "Gentle shoulder mobility", 60, 10, 2)
        .AddExercise("Wall Slides", "Shoulder blade strengthening", 45, 15, 3)
        .AddExercise("Resistance Band Pulls", "Rotator cuff strengthening", 90, 12, 3)
        .BuildAsync();

    if (program != null)
    {
        Console.WriteLine($"Created custom program: {program.Name} (ID: {program.Id})");
    }
}
```

## Error Handling Examples

### Comprehensive Error Handling

```csharp
public async Task<bool> RobustApiCallAsync()
{
    try
    {
        var response = await client.Users.GetUsersAsync();
        
        if (response.Success && response.Data != null)
        {
            Console.WriteLine($"Retrieved {response.Data.Count} users");
            return true;
        }
        else
        {
            Console.WriteLine($"API call failed: {response.Error?.Message}");
            return false;
        }
    }
    catch (CarespaceRateLimitException ex)
    {
        Console.WriteLine($"Rate limit exceeded. Retry after: {ex.RetryAfter}");
        
        if (ex.RetryAfter.HasValue)
        {
            await Task.Delay(ex.RetryAfter.Value);
            return await RobustApiCallAsync(); // Retry once
        }
        return false;
    }
    catch (CarespaceAuthenticationException ex)
    {
        Console.WriteLine($"Authentication failed: {ex.Message}");
        // Attempt to refresh token or re-authenticate
        return false;
    }
    catch (CarespaceNotFoundException ex)
    {
        Console.WriteLine($"Resource not found: {ex.Message}");
        return false;
    }
    catch (CarespaceValidationException ex)
    {
        Console.WriteLine("Validation errors:");
        foreach (var error in ex.ValidationErrors)
        {
            Console.WriteLine($"- {error.Key}: {string.Join(", ", error.Value)}");
        }
        return false;
    }
    catch (CarespaceServerException ex)
    {
        Console.WriteLine($"Server error: {ex.Message} (Status: {ex.StatusCode})");
        // Log for investigation, potentially retry with backoff
        return false;
    }
    catch (CarespaceException ex)
    {
        Console.WriteLine($"Carespace API error: {ex.Message}");
        return false;
    }
    catch (HttpRequestException ex)
    {
        Console.WriteLine($"Network error: {ex.Message}");
        return false;
    }
    catch (TaskCanceledException ex)
    {
        Console.WriteLine($"Request timeout: {ex.Message}");
        return false;
    }
}
```

### Retry Logic with Exponential Backoff

```csharp
public class RetryHelper
{
    public static async Task<T> RetryAsync<T>(
        Func<Task<T>> operation,
        int maxRetries = 3,
        TimeSpan? baseDelay = null)
    {
        var delay = baseDelay ?? TimeSpan.FromSeconds(1);
        var lastException = default(Exception);

        for (int i = 0; i <= maxRetries; i++)
        {
            try
            {
                return await operation();
            }
            catch (CarespaceRateLimitException ex) when (i < maxRetries)
            {
                var retryDelay = ex.RetryAfter ?? TimeSpan.FromSeconds(Math.Pow(2, i) * delay.TotalSeconds);
                await Task.Delay(retryDelay);
                lastException = ex;
            }
            catch (CarespaceServerException ex) when (i < maxRetries && ex.StatusCode >= HttpStatusCode.InternalServerError)
            {
                var retryDelay = TimeSpan.FromSeconds(Math.Pow(2, i) * delay.TotalSeconds);
                await Task.Delay(retryDelay);
                lastException = ex;
            }
            catch (HttpRequestException ex) when (i < maxRetries)
            {
                var retryDelay = TimeSpan.FromSeconds(Math.Pow(2, i) * delay.TotalSeconds);
                await Task.Delay(retryDelay);
                lastException = ex;
            }
        }

        throw lastException ?? new InvalidOperationException("Retry operation failed");
    }
}

// Usage
public async Task UseRetryLogicAsync()
{
    var users = await RetryHelper.RetryAsync(async () =>
    {
        var response = await client.Users.GetUsersAsync();
        if (!response.Success)
            throw new InvalidOperationException("Failed to get users");
        return response.Data;
    });
}
```

## Advanced Use Cases

### Bulk Data Processing

```csharp
public async Task ProcessLargeDatasetAsync()
{
    var allUsers = new List<User>();
    var pageSize = 100;
    var page = 1;
    
    // Fetch all users with pagination
    while (true)
    {
        var response = await client.Users.GetUsersAsync(page, pageSize);
        
        if (!response.Success || response.Data == null || !response.Data.Any())
            break;
            
        allUsers.AddRange(response.Data);
        
        Console.WriteLine($"Fetched page {page}, total users: {allUsers.Count}");
        
        if (!response.HasMore)
            break;
            
        page++;
        
        // Rate limiting consideration
        await Task.Delay(100);
    }
    
    Console.WriteLine($"Total users retrieved: {allUsers.Count}");
    
    // Process users in batches
    var batchSize = 10;
    var batches = allUsers.Chunk(batchSize);
    
    foreach (var batch in batches)
    {
        await ProcessUserBatchAsync(batch);
    }
}

private async Task ProcessUserBatchAsync(IEnumerable<User> users)
{
    var tasks = users.Select(async user =>
    {
        // Example: Update user profile
        var updateRequest = new UpdateUserRequest
        {
            Profile = new UserProfile
            {
                LastActivityDate = DateTime.UtcNow
            }
        };
        
        return await client.Users.UpdateUserAsync(user.Id, updateRequest);
    });
    
    var results = await Task.WhenAll(tasks);
    var successCount = results.Count(r => r.Success);
    
    Console.WriteLine($"Batch processed: {successCount}/{results.Length} successful");
}
```

### Real-time Progress Monitoring

```csharp
public class ProgressMonitor
{
    private readonly CarespaceClient _client;
    private readonly Timer _timer;
    private readonly Dictionary<string, ClientProgress> _lastProgress;

    public ProgressMonitor(CarespaceClient client)
    {
        _client = client;
        _lastProgress = new Dictionary<string, ClientProgress>();
        _timer = new Timer(CheckProgress, null, TimeSpan.Zero, TimeSpan.FromMinutes(5));
    }

    private async void CheckProgress(object? state)
    {
        try
        {
            var clients = await _client.Clients.GetClientsAsync(limit: 100);
            
            if (clients.Success && clients.Data != null)
            {
                var tasks = clients.Data.Select(CheckClientProgressAsync);
                await Task.WhenAll(tasks);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error checking progress: {ex.Message}");
        }
    }

    private async Task CheckClientProgressAsync(Client client)
    {
        try
        {
            var statsResponse = await _client.Clients.GetClientStatsAsync(client.Id);
            
            if (statsResponse.Success && statsResponse.Data != null)
            {
                var currentProgress = new ClientProgress
                {
                    ClientId = client.Id,
                    CompletionRate = statsResponse.Data.CompletionRate,
                    TotalSessions = statsResponse.Data.TotalSessions,
                    LastActivity = statsResponse.Data.LastActivityDate
                };

                if (_lastProgress.TryGetValue(client.Id, out var lastProgress))
                {
                    // Check for significant changes
                    if (currentProgress.CompletionRate > lastProgress.CompletionRate + 0.1) // 10% increase
                    {
                        Console.WriteLine($"🎉 {client.Name} made significant progress! Completion: {currentProgress.CompletionRate:P2}");
                    }
                    
                    if (currentProgress.LastActivity > lastProgress.LastActivity)
                    {
                        Console.WriteLine($"📈 {client.Name} had recent activity on {currentProgress.LastActivity:yyyy-MM-dd}");
                    }
                }

                _lastProgress[client.Id] = currentProgress;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error checking progress for {client.Name}: {ex.Message}");
        }
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}

public class ClientProgress
{
    public string ClientId { get; set; } = "";
    public double CompletionRate { get; set; }
    public int TotalSessions { get; set; }
    public DateTime LastActivity { get; set; }
}
```

## Integration Patterns

### Background Service Integration

```csharp
public class CarespaceBackgroundService : BackgroundService
{
    private readonly CarespaceClient _carespace;
    private readonly ILogger<CarespaceBackgroundService> _logger;

    public CarespaceBackgroundService(CarespaceClient carespace, ILogger<CarespaceBackgroundService> logger)
    {
        _carespace = carespace;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await SyncDataAsync();
                await GenerateReportsAsync();
                await SendNotificationsAsync();

                // Wait for next cycle
                await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in background service");
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
            }
        }
    }

    private async Task SyncDataAsync()
    {
        _logger.LogInformation("Starting data synchronization");
        
        // Sync recent changes
        var recentUsers = await _carespace.Users.GetUsersAsync(
            page: 1, 
            limit: 100
            // Add date filter for recent changes
        );
        
        if (recentUsers.Success && recentUsers.Data != null)
        {
            _logger.LogInformation("Synced {Count} users", recentUsers.Data.Count);
        }
    }

    private async Task GenerateReportsAsync()
    {
        _logger.LogInformation("Generating daily reports");
        
        // Generate summary reports
        var clients = await _carespace.Clients.GetClientsAsync(limit: 1000);
        
        if (clients.Success && clients.Data != null)
        {
            var activeClients = clients.Data.Count(c => c.IsActive);
            var totalSessions = clients.Data.Sum(c => c.Stats?.TotalSessions ?? 0);
            
            _logger.LogInformation("Daily Report - Active Clients: {Active}, Total Sessions: {Sessions}", 
                activeClients, totalSessions);
        }
    }

    private async Task SendNotificationsAsync()
    {
        _logger.LogInformation("Checking for notifications");
        
        // Implementation for sending notifications
        // based on client progress, missed sessions, etc.
    }
}

// Registration in Program.cs
builder.Services.AddHostedService<CarespaceBackgroundService>();
```

### Custom Middleware for Web Applications

```csharp
public class CarespaceAuthMiddleware
{
    private readonly RequestDelegate _next;
    private readonly CarespaceClient _carespace;

    public CarespaceAuthMiddleware(RequestDelegate next, CarespaceClient carespace)
    {
        _next = next;
        _carespace = carespace;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var token = context.Request.Headers["Authorization"]
            .FirstOrDefault()?.Split(" ").Last();

        if (token != null)
        {
            try
            {
                // Validate token with Carespace API
                var userResponse = await _carespace.Auth.ValidateTokenAsync(token);
                
                if (userResponse.Success && userResponse.Data != null)
                {
                    // Add user info to context
                    context.Items["User"] = userResponse.Data;
                    context.Items["UserId"] = userResponse.Data.Id;
                }
            }
            catch (CarespaceAuthenticationException)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Invalid token");
                return;
            }
        }

        await _next(context);
    }
}

// Usage in Program.cs
app.UseMiddleware<CarespaceAuthMiddleware>();
```

## Best Practices

### Resource Management

```csharp
// Good - Use using statements for proper disposal
using var client = new CarespaceClient(configuration);
await client.Users.GetUsersAsync();

// Good - Reuse client instances
public class UserService
{
    private readonly CarespaceClient _client;

    public UserService(CarespaceClient client)
    {
        _client = client; // Injected singleton
    }

    public async Task<List<User>> GetActiveUsersAsync()
    {
        var response = await _client.Users.GetUsersAsync(isActive: true);
        return response.Data ?? new List<User>();
    }
}

// Bad - Creating multiple client instances
var client1 = new CarespaceClient(config);
await client1.Users.GetUsersAsync();
client1.Dispose();

var client2 = new CarespaceClient(config);
await client2.Clients.GetClientsAsync();
client2.Dispose();
```

### Configuration Management

```csharp
// Good - Environment-based configuration
public static class CarespaceConfigurationFactory
{
    public static CarespaceConfiguration Create(IConfiguration configuration)
    {
        var section = configuration.GetSection("Carespace");
        
        return new CarespaceConfiguration
        {
            ApiKey = section["ApiKey"] ?? throw new InvalidOperationException("Carespace:ApiKey is required"),
            BaseUrl = section["BaseUrl"] ?? GetDefaultBaseUrl(),
            Timeout = TimeSpan.FromSeconds(section.GetValue<int>("TimeoutSeconds", 30)),
            MaxRetryAttempts = section.GetValue<int>("MaxRetryAttempts", 3),
            EnableLogging = section.GetValue<bool>("EnableLogging", false)
        };
    }

    private static string GetDefaultBaseUrl()
    {
        return Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") switch
        {
            "Production" => "https://api.carespace.ai",
            "Staging" => "https://api-staging.carespace.ai",
            _ => "https://api-dev.carespace.ai"
        };
    }
}
```

### Error Handling Best Practices

```csharp
public class UserService
{
    private readonly CarespaceClient _client;
    private readonly ILogger<UserService> _logger;

    public UserService(CarespaceClient client, ILogger<UserService> logger)
    {
        _client = client;
        _logger = logger;
    }

    public async Task<ServiceResult<User>> CreateUserAsync(CreateUserRequest request)
    {
        try
        {
            var response = await _client.Users.CreateUserAsync(request);
            
            if (response.Success && response.Data != null)
            {
                _logger.LogInformation("User created successfully: {UserId}", response.Data.Id);
                return ServiceResult<User>.Success(response.Data);
            }
            else
            {
                _logger.LogWarning("Failed to create user: {Error}", response.Error?.Message);
                return ServiceResult<User>.Failure(response.Error?.Message ?? "Unknown error");
            }
        }
        catch (CarespaceValidationException ex)
        {
            _logger.LogWarning("User creation validation failed: {Errors}", 
                string.Join(", ", ex.ValidationErrors.SelectMany(e => e.Value)));
            return ServiceResult<User>.ValidationFailure(ex.ValidationErrors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error creating user");
            return ServiceResult<User>.Failure("An unexpected error occurred");
        }
    }
}

public class ServiceResult<T>
{
    public bool Success { get; private set; }
    public T? Data { get; private set; }
    public string? ErrorMessage { get; private set; }
    public Dictionary<string, string[]>? ValidationErrors { get; private set; }

    private ServiceResult() { }

    public static ServiceResult<T> Success(T data) => new() { Success = true, Data = data };
    public static ServiceResult<T> Failure(string error) => new() { Success = false, ErrorMessage = error };
    public static ServiceResult<T> ValidationFailure(Dictionary<string, string[]> errors) => 
        new() { Success = false, ValidationErrors = errors };
}
```

These examples demonstrate comprehensive usage patterns for the Carespace .NET SDK, covering everything from basic operations to advanced integration scenarios. Use these as starting points for your own implementations and adapt them to your specific requirements.