# Carespace .NET SDK

[![NuGet](https://img.shields.io/nuget/v/Carespace.SDK)](https://www.nuget.org/packages/Carespace.SDK)
[![.NET](https://img.shields.io/badge/.NET-6.0%2B-blue)](https://dotnet.microsoft.com/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

Official .NET SDK for the Carespace API - Build powerful healthcare and rehabilitation applications with ease.

## Features

- 🔒 **Type-safe** - Full C# type definitions with nullable reference types
- ⚡ **Async/Await** - Modern async programming support
- 🛡️ **Error Handling** - Comprehensive exception types
- 🔄 **Retry Logic** - Built-in retry mechanisms with exponential backoff
- 📦 **Dependency Injection** - Full DI container support
- 🧪 **Well Tested** - Comprehensive unit test coverage
- 📚 **IntelliSense** - Rich IDE support with XML documentation

## Installation

### Package Manager
```bash
Install-Package Carespace.SDK
```

### .NET CLI
```bash
dotnet add package Carespace.SDK
```

### PackageReference
```xml
<PackageReference Include="Carespace.SDK" Version="1.0.0" />
```

## Quick Start

### Basic Usage

```csharp
using CarespaceSDK;
using CarespaceSDK.Models;

// Create client
using var client = CarespaceClient.CreateForDevelopment("your-api-key");

// Authenticate
var loginResponse = await client.LoginAndSetTokenAsync("user@example.com", "password");
Console.WriteLine($"Logged in as: {loginResponse.User.Name}");

// Get users
var users = await client.QuickGetUsersAsync(limit: 10);
Console.WriteLine($"Found {users.Data?.Count} users");

// Create a client
var newClient = new CreateClientRequest
{
    Name = "John Doe",
    Email = "john@example.com",
    Phone = "+1-555-0123"
};

var createdClient = await client.Clients.CreateClientAsync(newClient);
Console.WriteLine($"Created client: {createdClient.Data?.Id}");
```

### Dependency Injection

```csharp
using CarespaceSDK.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

// Register Carespace SDK
builder.Services.AddCarespaceSDKForDevelopment("your-api-key");

var host = builder.Build();

// Use in your services
public class MyService
{
    private readonly CarespaceClient _carespace;

    public MyService(CarespaceClient carespace)
    {
        _carespace = carespace;
    }

    public async Task DoSomethingAsync()
    {
        var users = await _carespace.Users.GetUsersAsync();
        // Use users...
    }
}
```

### Configuration

```csharp
using CarespaceSDK.Configuration;

// Custom configuration
var configuration = new CarespaceConfiguration
{
    BaseUrl = "https://api.carespace.ai",
    ApiKey = "your-api-key",
    Timeout = TimeSpan.FromSeconds(60),
    MaxRetryAttempts = 5,
    EnableLogging = true
};

using var client = new CarespaceClient(configuration);

// Or use predefined configurations
using var devClient = CarespaceClient.CreateForDevelopment("api-key");
using var prodClient = CarespaceClient.CreateForProduction("api-key");
using var stagingClient = CarespaceClient.CreateForStaging("api-key");
```

## API Reference

### Authentication

```csharp
// Login
var loginResponse = await client.Auth.LoginAsync("user@example.com", "password");

// Refresh token
var refreshResponse = await client.Auth.RefreshTokenAsync(refreshToken);

// Change password
await client.Auth.ChangePasswordAsync("currentPassword", "newPassword");

// Logout
await client.Auth.LogoutAsync();
```

### Users

```csharp
// Get users with pagination and filtering
var users = await client.Users.GetUsersAsync(
    page: 1, 
    limit: 20, 
    search: "john", 
    role: UserRole.Patient,
    isActive: true
);

// Get specific user
var user = await client.Users.GetUserAsync("user-id");

// Create user
var newUser = new CreateUserRequest
{
    Email = "user@example.com",
    Name = "John Doe",
    FirstName = "John",
    LastName = "Doe",
    Password = "SecurePassword123!",
    Role = UserRole.Patient
};
var createdUser = await client.Users.CreateUserAsync(newUser);

// Update user
var updateUser = new UpdateUserRequest { Name = "Jane Doe" };
await client.Users.UpdateUserAsync("user-id", updateUser);
```

### Clients

```csharp
// Get clients
var clients = await client.Clients.GetClientsAsync(
    page: 1,
    limit: 20,
    search: "doe",
    providerId: "provider-id"
);

// Create client
var newClient = new CreateClientRequest
{
    Name = "Jane Doe",
    Email = "jane@example.com",
    Phone = "+1-555-0456",
    DateOfBirth = DateTime.Parse("1990-01-01"),
    MedicalInfo = new MedicalInfo
    {
        Allergies = new[] { "Peanuts" },
        Conditions = new[] { "Hypertension" }
    }
};
var createdClient = await client.Clients.CreateClientAsync(newClient);

// Get client statistics
var stats = await client.Clients.GetClientStatsAsync("client-id");
Console.WriteLine($"Total sessions: {stats.Data?.TotalSessions}");

// Assign program to client
await client.Clients.AssignProgramAsync("client-id", "program-id");
```

### Programs

```csharp
// Get programs
var programs = await client.Programs.GetProgramsAsync(
    page: 1,
    limit: 20,
    category: ProgramCategory.Rehabilitation,
    difficulty: ProgramDifficulty.Beginner
);

// Create program
var newProgram = new CreateProgramRequest
{
    Name = "Post-Surgery Knee Rehabilitation",
    Description = "Comprehensive knee rehabilitation program",
    Category = ProgramCategory.Rehabilitation,
    Difficulty = ProgramDifficulty.Beginner,
    Duration = 30,
    IsPublic = true
};
var createdProgram = await client.Programs.CreateProgramAsync(newProgram);

// Add exercise to program
var exercise = new CreateExerciseRequest
{
    Name = "Knee Flexion",
    Description = "Gentle knee bending exercise",
    Duration = 60,
    Repetitions = 10,
    Sets = 3,
    Order = 1
};
await client.Programs.AddExerciseToProgramAsync("program-id", exercise);

// Get program exercises
var exercises = await client.Programs.GetProgramExercisesAsync("program-id");
```

## Error Handling

The SDK provides specific exception types for different error scenarios:

```csharp
try
{
    var user = await client.Users.GetUserAsync("invalid-id");
}
catch (CarespaceNotFoundException ex)
{
    Console.WriteLine("User not found");
}
catch (CarespaceAuthenticationException ex)
{
    Console.WriteLine("Authentication failed");
}
catch (CarespaceValidationException ex)
{
    Console.WriteLine($"Validation failed: {ex.ErrorDetails}");
}
catch (CarespaceRateLimitException ex)
{
    Console.WriteLine($"Rate limit exceeded. Retry after: {ex.RetryAfter}");
}
catch (CarespaceException ex)
{
    Console.WriteLine($"API error: {ex.Message} (Status: {ex.StatusCode})");
}
```

## Configuration Options

```csharp
var configuration = new CarespaceConfiguration
{
    BaseUrl = "https://api.carespace.ai",           // API base URL
    ApiKey = "your-api-key",                        // API key for authentication
    Timeout = TimeSpan.FromSeconds(30),             // Request timeout
    MaxRetryAttempts = 3,                           // Maximum retry attempts
    RetryDelay = TimeSpan.FromSeconds(1),           // Initial retry delay
    EnableRetry = true,                             // Enable automatic retries
    EnableLogging = true,                           // Enable request/response logging
    UserAgent = "MyApp/1.0.0 (.NET)",              // Custom user agent
    DefaultHeaders = new Dictionary<string, string> // Default headers
    {
        ["X-Custom-Header"] = "custom-value"
    }
};
```

## Environments

### Development
```csharp
using var client = CarespaceClient.CreateForDevelopment("api-key");
// Base URL: https://api-dev.carespace.ai
```

### Staging
```csharp
using var client = CarespaceClient.CreateForStaging("api-key");
// Base URL: https://api-staging.carespace.ai
```

### Production
```csharp
using var client = CarespaceClient.CreateForProduction("api-key");
// Base URL: https://api.carespace.ai
```

## Examples

Check out the [examples directory](./examples/) for complete sample applications:

- **[Console Application](./examples/ConsoleApp/)** - Basic console app demonstrating SDK usage
- **[Web API](./examples/WebApi/)** - ASP.NET Core Web API using dependency injection

### Running Examples

```bash
# Console application
cd examples/ConsoleApp
dotnet run

# Web API
cd examples/WebApi
dotnet run
```

## Testing

```bash
# Run all tests
dotnet test

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test project
dotnet test tests/CarespaceSDK.Tests/
```

## Building from Source

```bash
# Clone the repository
git clone https://github.com/carespace/sdk-monorepo.git
cd carespace-sdk/dotnet

# Restore dependencies
dotnet restore

# Build the solution
dotnet build

# Run tests
dotnet test

# Create NuGet package
dotnet pack src/CarespaceSDK/CarespaceSDK.csproj -c Release
```

## Contributing

We welcome contributions! Please see our [Contributing Guide](../CONTRIBUTING.md) for details.

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests
5. Submit a pull request

## Requirements

- .NET 6.0 or later
- C# 10.0 or later

## Dependencies

- System.Text.Json (7.0.0+)
- Microsoft.Extensions.Http (7.0.0+)
- Microsoft.Extensions.Logging.Abstractions (7.0.0+)
- Microsoft.Extensions.DependencyInjection.Abstractions (7.0.0+)

## License

This project is licensed under the MIT License - see the [LICENSE](../LICENSE) file for details.

## Support

- 📧 Email: [support@carespace.ai](mailto:support@carespace.ai)
- 💬 [Discord Community](https://discord.gg/carespace)
- 🐛 [Report Issues](https://github.com/carespace/sdk-monorepo/issues)
- 📖 [API Documentation](https://docs.carespace.ai/api)

---

<p align="center">
  <strong>Built with ❤️ by the Carespace Team</strong><br>
  <a href="https://carespace.ai">🌐 Website</a> •
  <a href="https://docs.carespace.ai">📖 Docs</a> •
  <a href="https://github.com/carespace">🔗 GitHub</a>
</p>