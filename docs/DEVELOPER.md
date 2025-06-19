# Carespace .NET SDK - Developer Guide

This guide provides comprehensive information for developers working on or contributing to the Carespace .NET SDK.

## Table of Contents

- [Development Setup](#development-setup)
- [Project Structure](#project-structure)
- [Architecture Overview](#architecture-overview)
- [Building the SDK](#building-the-sdk)
- [Testing](#testing)
- [Code Style Guidelines](#code-style-guidelines)
- [Contributing](#contributing)
- [Release Process](#release-process)

## Development Setup

### Prerequisites

- **.NET 6.0 SDK** or later
- **Visual Studio 2022** or **VS Code** with C# extension
- **Git** for version control

### Getting Started

1. **Clone the repository:**
   ```bash
   git clone https://github.com/carespace/sdk-monorepo.git
   cd carespace-sdk/dotnet
   ```

2. **Restore dependencies:**
   ```bash
   dotnet restore
   ```

3. **Build the solution:**
   ```bash
   dotnet build
   ```

4. **Run the tests:**
   ```bash
   dotnet test
   ```

### Development Environment

The SDK supports development against different Carespace API environments:

- **Development:** `https://api-dev.carespace.ai`
- **Staging:** `https://api-staging.carespace.ai`
- **Production:** `https://api.carespace.ai`

For local development, use the development environment by default.

## Project Structure

```
src/CarespaceSDK/
├── Api/                          # API interface definitions and implementations
│   ├── IAuthApi.cs              # Authentication API interface
│   ├── AuthApi.cs               # Authentication API implementation
│   ├── IUsersApi.cs             # Users API interface
│   ├── UsersApi.cs              # Users API implementation
│   ├── IClientsApi.cs           # Clients API interface
│   ├── ClientsApi.cs            # Clients API implementation
│   ├── IProgramsApi.cs          # Programs API interface
│   └── ProgramsApi.cs           # Programs API implementation
├── Configuration/               # Configuration classes
│   └── CarespaceConfiguration.cs # Main configuration class
├── Exceptions/                  # Custom exception types
│   └── CarespaceException.cs    # Exception hierarchy
├── Extensions/                  # Extension methods
│   └── ServiceCollectionExtensions.cs # DI extensions
├── Http/                        # HTTP client abstraction
│   ├── IHttpClient.cs           # HTTP client interface
│   └── CarespaceHttpClient.cs   # HTTP client implementation
├── Models/                      # Data models
│   ├── Auth.cs                  # Authentication models
│   ├── Client.cs                # Client models
│   ├── Common.cs                # Common/shared models
│   ├── Program.cs               # Program models
│   └── User.cs                  # User models
└── CarespaceClient.cs           # Main SDK client

tests/CarespaceSDK.Tests/
├── Configuration/               # Configuration tests
├── Exceptions/                  # Exception tests
├── Models/                      # Model tests
├── CarespaceClientTests.cs      # Main client tests
└── CarespaceSDK.Tests.csproj    # Test project file

examples/
├── ConsoleApp/                  # Console application example
└── WebApi/                      # Web API example
```

## Architecture Overview

### Core Components

#### CarespaceClient
The main entry point for the SDK. Provides access to all API endpoints and manages configuration, authentication, and HTTP communication.

```csharp
public class CarespaceClient : IDisposable
{
    public IAuthApi Auth { get; }
    public IUsersApi Users { get; }
    public IClientsApi Clients { get; }
    public IProgramsApi Programs { get; }
}
```

#### API Interfaces
Each API area is defined by an interface (e.g., `IUsersApi`) and implemented by a concrete class (e.g., `UsersApi`). This allows for easy mocking in tests and potential future customization.

#### HTTP Client Abstraction
The `IHttpClient` interface abstracts HTTP communication, making the SDK testable and allowing for custom HTTP client implementations.

#### Configuration System
The `CarespaceConfiguration` class provides a centralized way to configure the SDK with type-safe properties and validation.

#### Exception Hierarchy
Custom exception types provide specific error handling for different API error scenarios.

### Design Patterns

#### Repository Pattern
Each API area follows the repository pattern, providing a clean abstraction over HTTP API calls.

#### Factory Pattern
The SDK provides factory methods for common configuration scenarios:
```csharp
CarespaceClient.CreateForDevelopment("api-key");
CarespaceClient.CreateForProduction("api-key");
```

#### Async/Await Throughout
All API methods are fully asynchronous with proper `CancellationToken` support.

## Building the SDK

### Build Commands

```bash
# Build the entire solution
dotnet build

# Build in Release mode
dotnet build -c Release

# Clean build artifacts
dotnet clean

# Restore NuGet packages
dotnet restore
```

### Creating NuGet Package

```bash
# Create package
dotnet pack src/CarespaceSDK/CarespaceSDK.csproj -c Release

# Package with specific version
dotnet pack src/CarespaceSDK/CarespaceSDK.csproj -c Release -p:PackageVersion=1.0.1
```

The package will be created in `src/CarespaceSDK/bin/Release/`.

## Testing

### Test Structure

The test project uses **xUnit** as the testing framework with the following structure:

- **Unit Tests:** Test individual classes and methods in isolation
- **Integration Tests:** Test API interactions (when appropriate)
- **Model Tests:** Test data model serialization/deserialization

### Running Tests

```bash
# Run all tests
dotnet test

# Run tests with detailed output
dotnet test -v normal

# Run tests with code coverage
dotnet test --collect:"XPlat Code Coverage"

# Run specific test project
dotnet test tests/CarespaceSDK.Tests/

# Run tests matching a filter
dotnet test --filter "TestCategory=Unit"
```

### Writing Tests

#### Unit Test Example
```csharp
[Fact]
public void Configuration_ShouldValidateApiKey()
{
    // Arrange
    var config = new CarespaceConfiguration
    {
        ApiKey = "",
        BaseUrl = "https://api.example.com"
    };

    // Act & Assert
    Assert.Throws<ArgumentException>(() => config.Validate());
}
```

#### Mock HTTP Client Example
```csharp
[Fact]
public async Task GetUsersAsync_ShouldReturnUsers()
{
    // Arrange
    var mockHttpClient = new Mock<IHttpClient>();
    var expectedUsers = new List<User> { new User { Id = "1", Name = "Test" } };
    var apiResponse = new ApiResponse<List<User>> { Success = true, Data = expectedUsers };
    
    mockHttpClient
        .Setup(x => x.GetAsync<List<User>>(It.IsAny<string>(), It.IsAny<CancellationToken>()))
        .ReturnsAsync(apiResponse);

    var usersApi = new UsersApi(mockHttpClient.Object);

    // Act
    var result = await usersApi.GetUsersAsync();

    // Assert
    Assert.True(result.Success);
    Assert.Equal(expectedUsers, result.Data);
}
```

### Test Coverage

Aim for:
- **80%+ line coverage** overall
- **90%+ coverage** for core business logic
- **100% coverage** for public API methods

## Code Style Guidelines

### General Guidelines

1. **Follow C# naming conventions:**
   - PascalCase for public members, classes, methods
   - camelCase for private fields and local variables
   - Use `_` prefix for private fields

2. **Use nullable reference types:** Enable nullable reference types and handle null cases appropriately

3. **Async/Await:** Use async/await throughout, never block on async calls

4. **XML Documentation:** Document all public APIs with XML comments

### Example Code Style

```csharp
/// <summary>
/// Retrieves a paginated list of users with optional filtering.
/// </summary>
/// <param name="page">Page number (default: 1)</param>
/// <param name="limit">Items per page (default: 20, max: 100)</param>
/// <param name="search">Search query for user name or email</param>
/// <param name="cancellationToken">Cancellation token</param>
/// <returns>API response containing users and pagination metadata</returns>
public async Task<ApiResponse<List<User>>> GetUsersAsync(
    int page = 1,
    int limit = 20,
    string? search = null,
    CancellationToken cancellationToken = default)
{
    var queryParams = new Dictionary<string, object>
    {
        ["page"] = page,
        ["limit"] = limit
    };

    if (!string.IsNullOrEmpty(search))
    {
        queryParams["search"] = search;
    }

    return await _httpClient.GetAsync<List<User>>(
        "users",
        queryParams,
        cancellationToken);
}
```

### EditorConfig

The project includes an `.editorconfig` file with coding standards:

```ini
root = true

[*]
charset = utf-8
end_of_line = crlf
insert_final_newline = true
indent_style = space
indent_size = 4

[*.cs]
csharp_new_line_before_open_brace = all
csharp_new_line_before_else = true
csharp_new_line_before_catch = true
csharp_new_line_before_finally = true
```

## Contributing

### Pull Request Process

1. **Fork the repository** and create a feature branch
2. **Make your changes** following the code style guidelines
3. **Add tests** for new functionality
4. **Update documentation** if needed
5. **Ensure all tests pass** and code coverage meets requirements
6. **Submit a pull request** with a clear description

### Commit Message Format

Use conventional commit messages:

```
type(scope): description

[optional body]

[optional footer]
```

Types:
- `feat`: New feature
- `fix`: Bug fix
- `docs`: Documentation changes
- `test`: Adding or updating tests
- `refactor`: Code refactoring
- `chore`: Maintenance tasks

Examples:
```
feat(auth): add support for refresh token rotation
fix(http): handle timeout exceptions properly
docs(api): update authentication examples
```

### Code Review Guidelines

- **Functionality:** Does the code work as intended?
- **Readability:** Is the code easy to understand?
- **Performance:** Are there any performance issues?
- **Security:** Are there any security concerns?
- **Tests:** Are there adequate tests?
- **Documentation:** Is the code properly documented?

## Release Process

### Version Management

The SDK follows [Semantic Versioning](https://semver.org/):

- **MAJOR:** Breaking changes
- **MINOR:** New features (backward compatible)
- **PATCH:** Bug fixes (backward compatible)

### Release Steps

1. **Update version** in `CarespaceSDK.csproj`
2. **Update CHANGELOG.md** with release notes
3. **Create a release branch** (`release/v1.0.1`)
4. **Run full test suite** and ensure all tests pass
5. **Create and test NuGet package**
6. **Submit pull request** for release
7. **Merge to main** after approval
8. **Create GitHub release** with tag
9. **Publish to NuGet.org**

### Automated Builds

The project uses GitHub Actions for:
- **Continuous Integration:** Build and test on every PR
- **Code Coverage:** Generate coverage reports
- **Security Scanning:** Scan for vulnerabilities
- **Package Publishing:** Publish to NuGet on releases

## Development Tools

### Recommended Extensions (VS Code)

- **C# for Visual Studio Code**
- **C# Extensions**
- **NuGet Package Manager**
- **GitLens**
- **EditorConfig for VS Code**

### Recommended Extensions (Visual Studio)

- **GitHub Extension for Visual Studio**
- **Code Coverage**
- **SonarLint**

### Useful Commands

```bash
# Watch tests (run tests on file changes)
dotnet watch test

# Format code
dotnet format

# List outdated packages
dotnet list package --outdated

# Analyze code for issues
dotnet analyze
```

## Debugging

### Local Development

Set environment variables for local testing:

```bash
export CARESPACE_API_KEY="your-dev-api-key"
export CARESPACE_BASE_URL="https://api-dev.carespace.ai"
```

Or use `appsettings.json` in examples:

```json
{
  "Carespace": {
    "ApiKey": "your-dev-api-key",
    "BaseUrl": "https://api-dev.carespace.ai"
  }
}
```

### Logging

Enable detailed logging for debugging:

```csharp
var configuration = new CarespaceConfiguration
{
    ApiKey = "your-api-key",
    EnableLogging = true,
    LogLevel = LogLevel.Debug,
    LogRequestBody = true,
    LogResponseBody = true
};
```

## Support

For development questions or issues:

- **GitHub Issues:** [Report bugs and request features](https://github.com/carespace/sdk-monorepo/issues)
- **Discussions:** [Ask questions and share ideas](https://github.com/carespace/sdk-monorepo/discussions)
- **Email:** [dev-support@carespace.ai](mailto:dev-support@carespace.ai)