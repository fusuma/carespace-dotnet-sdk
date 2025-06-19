# Carespace .NET SDK - API Reference

This document provides detailed API reference for the Carespace .NET SDK.

## Table of Contents

- [Authentication API](#authentication-api)
- [Users API](#users-api)
- [Clients API](#clients-api)
- [Programs API](#programs-api)
- [Error Handling](#error-handling)
- [Response Models](#response-models)

## Authentication API

### IAuthApi Interface

The Authentication API provides methods for user authentication, token management, and password operations.

#### Methods

##### LoginAsync
```csharp
Task<ApiResponse<LoginResponse>> LoginAsync(string email, string password, CancellationToken cancellationToken = default)
```

Authenticates a user with email and password.

**Parameters:**
- `email` (string): User's email address
- `password` (string): User's password
- `cancellationToken` (CancellationToken): Optional cancellation token

**Returns:** `ApiResponse<LoginResponse>` containing authentication tokens and user information

**Example:**
```csharp
var response = await client.Auth.LoginAsync("user@example.com", "password");
if (response.Success)
{
    var token = response.Data.AccessToken;
    var user = response.Data.User;
}
```

##### RefreshTokenAsync
```csharp
Task<ApiResponse<RefreshTokenResponse>> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
```

Refreshes an expired access token using a refresh token.

**Parameters:**
- `refreshToken` (string): Valid refresh token
- `cancellationToken` (CancellationToken): Optional cancellation token

**Returns:** `ApiResponse<RefreshTokenResponse>` containing new tokens

##### ChangePasswordAsync
```csharp
Task<ApiResponse<object>> ChangePasswordAsync(string currentPassword, string newPassword, CancellationToken cancellationToken = default)
```

Changes the current user's password.

**Parameters:**
- `currentPassword` (string): Current password
- `newPassword` (string): New password
- `cancellationToken` (CancellationToken): Optional cancellation token

**Returns:** `ApiResponse<object>` indicating success or failure

##### LogoutAsync
```csharp
Task<ApiResponse<object>> LogoutAsync(CancellationToken cancellationToken = default)
```

Logs out the current user and invalidates tokens.

**Returns:** `ApiResponse<object>` indicating success or failure

## Users API

### IUsersApi Interface

The Users API provides methods for managing users in the system.

#### Methods

##### GetUsersAsync
```csharp
Task<ApiResponse<List<User>>> GetUsersAsync(
    int page = 1, 
    int limit = 20, 
    string? search = null, 
    UserRole? role = null, 
    bool? isActive = null,
    CancellationToken cancellationToken = default)
```

Retrieves a paginated list of users with optional filtering.

**Parameters:**
- `page` (int): Page number (default: 1)
- `limit` (int): Items per page (default: 20, max: 100)
- `search` (string?): Search query for user name or email
- `role` (UserRole?): Filter by user role
- `isActive` (bool?): Filter by active status
- `cancellationToken` (CancellationToken): Optional cancellation token

**Returns:** `ApiResponse<List<User>>` containing users and pagination metadata

**Example:**
```csharp
var response = await client.Users.GetUsersAsync(
    page: 1, 
    limit: 50, 
    search: "john", 
    role: UserRole.Patient,
    isActive: true);
```

##### GetUserAsync
```csharp
Task<ApiResponse<User>> GetUserAsync(string userId, CancellationToken cancellationToken = default)
```

Retrieves a specific user by ID.

**Parameters:**
- `userId` (string): Unique user identifier
- `cancellationToken` (CancellationToken): Optional cancellation token

**Returns:** `ApiResponse<User>` containing user details

##### CreateUserAsync
```csharp
Task<ApiResponse<User>> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
```

Creates a new user in the system.

**Parameters:**
- `request` (CreateUserRequest): User creation data
- `cancellationToken` (CancellationToken): Optional cancellation token

**Returns:** `ApiResponse<User>` containing created user details

**Example:**
```csharp
var newUser = new CreateUserRequest
{
    Email = "user@example.com",
    Name = "John Doe",
    FirstName = "John",
    LastName = "Doe",
    Password = "SecurePassword123!",
    Role = UserRole.Patient,
    Profile = new UserProfile
    {
        Phone = "+1-555-0123",
        DateOfBirth = DateTime.Parse("1990-01-01")
    }
};

var response = await client.Users.CreateUserAsync(newUser);
```

##### UpdateUserAsync
```csharp
Task<ApiResponse<User>> UpdateUserAsync(string userId, UpdateUserRequest request, CancellationToken cancellationToken = default)
```

Updates an existing user.

**Parameters:**
- `userId` (string): User ID to update
- `request` (UpdateUserRequest): Updated user data
- `cancellationToken` (CancellationToken): Optional cancellation token

**Returns:** `ApiResponse<User>` containing updated user details

##### DeleteUserAsync
```csharp
Task<ApiResponse<object>> DeleteUserAsync(string userId, CancellationToken cancellationToken = default)
```

Deletes a user from the system.

**Parameters:**
- `userId` (string): User ID to delete
- `cancellationToken` (CancellationToken): Optional cancellation token

**Returns:** `ApiResponse<object>` indicating success or failure

## Clients API

### IClientsApi Interface

The Clients API provides methods for managing healthcare clients/patients.

#### Methods

##### GetClientsAsync
```csharp
Task<ApiResponse<List<Client>>> GetClientsAsync(
    int page = 1, 
    int limit = 20, 
    string? search = null, 
    string? providerId = null,
    CancellationToken cancellationToken = default)
```

Retrieves a paginated list of clients.

**Parameters:**
- `page` (int): Page number (default: 1)
- `limit` (int): Items per page (default: 20, max: 100)
- `search` (string?): Search query for client name or email
- `providerId` (string?): Filter by healthcare provider ID
- `cancellationToken` (CancellationToken): Optional cancellation token

**Returns:** `ApiResponse<List<Client>>` containing clients and pagination metadata

##### GetClientAsync
```csharp
Task<ApiResponse<Client>> GetClientAsync(string clientId, CancellationToken cancellationToken = default)
```

Retrieves a specific client by ID.

##### CreateClientAsync
```csharp
Task<ApiResponse<Client>> CreateClientAsync(CreateClientRequest request, CancellationToken cancellationToken = default)
```

Creates a new client in the system.

**Example:**
```csharp
var newClient = new CreateClientRequest
{
    Name = "Jane Doe",
    Email = "jane@example.com",
    Phone = "+1-555-0456",
    DateOfBirth = DateTime.Parse("1985-05-15"),
    Gender = "Female",
    MedicalInfo = new MedicalInfo
    {
        Allergies = new[] { "Peanuts", "Shellfish" },
        Conditions = new[] { "Hypertension" },
        Notes = "Patient recovering from knee surgery"
    }
};

var response = await client.Clients.CreateClientAsync(newClient);
```

##### UpdateClientAsync
```csharp
Task<ApiResponse<Client>> UpdateClientAsync(string clientId, UpdateClientRequest request, CancellationToken cancellationToken = default)
```

Updates an existing client.

##### GetClientStatsAsync
```csharp
Task<ApiResponse<ClientStats>> GetClientStatsAsync(string clientId, CancellationToken cancellationToken = default)
```

Retrieves statistics for a specific client.

**Returns:** `ApiResponse<ClientStats>` containing session counts, completion rates, etc.

##### AssignProgramAsync
```csharp
Task<ApiResponse<object>> AssignProgramAsync(string clientId, string programId, CancellationToken cancellationToken = default)
```

Assigns a rehabilitation program to a client.

##### UnassignProgramAsync
```csharp
Task<ApiResponse<object>> UnassignProgramAsync(string clientId, string programId, CancellationToken cancellationToken = default)
```

Removes a program assignment from a client.

## Programs API

### IProgramsApi Interface

The Programs API provides methods for managing rehabilitation programs and exercises.

#### Methods

##### GetProgramsAsync
```csharp
Task<ApiResponse<List<Program>>> GetProgramsAsync(
    int page = 1, 
    int limit = 20, 
    string? search = null, 
    ProgramCategory? category = null, 
    ProgramDifficulty? difficulty = null,
    CancellationToken cancellationToken = default)
```

Retrieves a paginated list of programs.

**Parameters:**
- `page` (int): Page number (default: 1)
- `limit` (int): Items per page (default: 20, max: 100)
- `search` (string?): Search query for program name or description
- `category` (ProgramCategory?): Filter by program category
- `difficulty` (ProgramDifficulty?): Filter by difficulty level
- `cancellationToken` (CancellationToken): Optional cancellation token

##### CreateProgramAsync
```csharp
Task<ApiResponse<Program>> CreateProgramAsync(CreateProgramRequest request, CancellationToken cancellationToken = default)
```

Creates a new rehabilitation program.

**Example:**
```csharp
var newProgram = new CreateProgramRequest
{
    Name = "Post-Surgery Knee Rehabilitation",
    Description = "Comprehensive knee rehabilitation program",
    Category = ProgramCategory.Rehabilitation,
    Difficulty = ProgramDifficulty.Beginner,
    Duration = 30,
    IsTemplate = false,
    IsPublic = true,
    Tags = new[] { "knee", "surgery", "rehabilitation" }
};

var response = await client.Programs.CreateProgramAsync(newProgram);
```

##### GetProgramExercisesAsync
```csharp
Task<ApiResponse<List<Exercise>>> GetProgramExercisesAsync(
    string programId, 
    int page = 1, 
    int limit = 20,
    CancellationToken cancellationToken = default)
```

Retrieves exercises for a specific program.

##### AddExerciseToProgramAsync
```csharp
Task<ApiResponse<Exercise>> AddExerciseToProgramAsync(
    string programId, 
    CreateExerciseRequest request,
    CancellationToken cancellationToken = default)
```

Adds a new exercise to a program.

**Example:**
```csharp
var exercise = new CreateExerciseRequest
{
    Name = "Knee Flexion",
    Description = "Gentle knee bending exercise",
    Instructions = "Slowly bend your knee while lying down",
    Duration = 60,
    Repetitions = 10,
    Sets = 3,
    RestTime = 30,
    BodyParts = new[] { "Knee", "Quadriceps" },
    Equipment = new[] { "Exercise mat" },
    Order = 1
};

var response = await client.Programs.AddExerciseToProgramAsync(programId, exercise);
```

## Error Handling

The SDK provides specific exception types for different error scenarios:

### Exception Types

#### CarespaceException
Base exception class for all SDK errors.

**Properties:**
- `StatusCode` (HttpStatusCode): HTTP status code
- `ErrorCode` (string?): API-specific error code
- `ErrorDetails` (object?): Additional error information

#### CarespaceAuthenticationException
Thrown when authentication fails (401 Unauthorized).

#### CarespaceAuthorizationException
Thrown when user lacks permissions (403 Forbidden).

#### CarespaceNotFoundException
Thrown when requested resource is not found (404 Not Found).

#### CarespaceValidationException
Thrown when request validation fails (400 Bad Request).

**Properties:**
- `ValidationErrors` (Dictionary<string, string[]>): Field-specific validation errors

#### CarespaceRateLimitException
Thrown when rate limit is exceeded (429 Too Many Requests).

**Properties:**
- `RetryAfter` (TimeSpan?): Suggested retry delay

#### CarespaceServerException
Thrown for server errors (5xx status codes).

### Error Response Format

All API errors follow a consistent format:

```json
{
  "success": false,
  "error": {
    "code": "VALIDATION_ERROR",
    "message": "Validation failed",
    "details": {
      "email": ["Email is required"],
      "password": ["Password must be at least 8 characters"]
    }
  },
  "timestamp": "2023-12-01T10:30:00Z",
  "requestId": "req_123456789"
}
```

## Response Models

### ApiResponse<T>

Generic wrapper for all API responses.

**Properties:**
- `Success` (bool): Indicates if the request was successful
- `Data` (T?): Response data (null if unsuccessful)
- `Error` (ApiError?): Error details (null if successful)
- `Meta` (ResponseMeta?): Additional metadata
- `HasMore` (bool): Indicates if more data is available (pagination)

### ResponseMeta

Metadata included with API responses.

**Properties:**
- `Page` (int): Current page number
- `Limit` (int): Items per page
- `Total` (int): Total number of items
- `TotalPages` (int): Total number of pages

### Common Enums

#### UserRole
- `Admin`
- `Provider` 
- `Patient`
- `Caregiver`

#### ProgramCategory
- `Rehabilitation`
- `Prevention`
- `Maintenance`
- `Assessment`

#### ProgramDifficulty
- `Beginner`
- `Intermediate`
- `Advanced`

## Rate Limits

The Carespace API implements rate limiting to ensure fair usage:

- **Default Limit:** 1000 requests per hour per API key
- **Burst Limit:** 100 requests per minute
- **Rate Limit Headers:**
  - `X-RateLimit-Limit`: Total requests allowed
  - `X-RateLimit-Remaining`: Remaining requests
  - `X-RateLimit-Reset`: Unix timestamp when limit resets

When rate limits are exceeded, the API returns a `429 Too Many Requests` status with a `Retry-After` header indicating when to retry.

## Pagination

All list endpoints support pagination with consistent parameters:

- `page`: Page number (starting from 1)
- `limit`: Items per page (default: 20, max: 100)

Responses include pagination metadata in the `Meta` property:

```csharp
var response = await client.Users.GetUsersAsync(page: 2, limit: 50);
if (response.Success)
{
    var users = response.Data;
    var totalUsers = response.Meta?.Total;
    var hasMorePages = response.HasMore;
}
```

## API Versioning

The SDK targets API version 1.0. Version information is included in request headers automatically.

## Base URLs

- **Development:** `https://api-dev.carespace.ai`
- **Staging:** `https://api-staging.carespace.ai`
- **Production:** `https://api.carespace.ai`