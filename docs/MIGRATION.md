# Carespace .NET SDK - Migration Guide

This guide helps you migrate between different versions of the Carespace .NET SDK.

## Table of Contents

- [Version 1.0.0](#version-100)
- [General Migration Guidelines](#general-migration-guidelines)
- [Breaking Changes by Version](#breaking-changes-by-version)
- [Migration Tools and Utilities](#migration-tools-and-utilities)
- [Common Migration Scenarios](#common-migration-scenarios)
- [Troubleshooting](#troubleshooting)

## Version 1.0.0

### Initial Release

Version 1.0.0 is the initial release of the Carespace .NET SDK. If you're starting fresh, please refer to the [README.md](../README.md) for installation and setup instructions.

#### Key Features Introduced
- Full API coverage for Authentication, Users, Clients, and Programs
- Comprehensive error handling with specific exception types
- Built-in retry logic with exponential backoff
- Dependency injection support
- Complete async/await support
- Extensive unit test coverage

#### Requirements
- .NET 6.0 or later
- C# 10.0 or later

## General Migration Guidelines

### Before You Begin

1. **Backup your project** - Always create a backup before starting migration
2. **Review release notes** - Check the changelog for breaking changes
3. **Update tests** - Plan to update unit tests alongside your code
4. **Test thoroughly** - Test in a development environment first

### Migration Process

1. **Update package reference** in your `.csproj` file:
   ```xml
   <PackageReference Include="Carespace.SDK" Version="[NEW_VERSION]" />
   ```

2. **Clean and rebuild** your solution:
   ```bash
   dotnet clean
   dotnet build
   ```

3. **Address compilation errors** - Fix any breaking changes
4. **Update method calls** - Adapt to new API signatures if needed
5. **Test functionality** - Ensure your application works as expected

## Breaking Changes by Version

### Future Versions

*This section will be updated as new versions are released.*

#### Version 2.0.0 (Planned)

**Potential Breaking Changes (Subject to Change):**

- **Authentication Flow Updates**: May introduce new authentication methods
- **Response Model Changes**: Enhanced response models with additional metadata
- **Configuration Updates**: New configuration options and validation rules

**Migration Steps (When Available):**
1. Update authentication initialization
2. Update response handling code
3. Review and update configuration

#### Version 1.1.0 (Planned)

**Non-Breaking Changes:**
- New API endpoints for additional functionality
- Enhanced error messages and logging
- Performance improvements

**Migration Steps:**
- Simple package update, no code changes required
- Optional: Update to use new features

## Migration Tools and Utilities

### Configuration Migration Helper

```csharp
public static class ConfigurationMigrator
{
    /// <summary>
    /// Migrates old configuration format to new format
    /// </summary>
    public static CarespaceConfiguration MigrateConfiguration(LegacyConfiguration legacy)
    {
        return new CarespaceConfiguration
        {
            ApiKey = legacy.ApiKey,
            BaseUrl = legacy.BaseUrl,
            Timeout = TimeSpan.FromSeconds(legacy.TimeoutSeconds),
            MaxRetryAttempts = legacy.MaxRetries,
            EnableLogging = legacy.EnableLogging
        };
    }
}

// Usage
var oldConfig = GetLegacyConfiguration();
var newConfig = ConfigurationMigrator.MigrateConfiguration(oldConfig);
```

### Code Pattern Analyzer

```csharp
public static class ApiCallAnalyzer
{
    /// <summary>
    /// Analyzes and suggests updates for deprecated API patterns
    /// </summary>
    public static void AnalyzeApiUsage(string sourceCode)
    {
        // This would analyze your code for deprecated patterns
        // and suggest modern alternatives
        
        var deprecatedPatterns = new[]
        {
            "client.GetUsers()", // Example deprecated method
            "client.SyncMethod()" // Example sync method
        };
        
        foreach (var pattern in deprecatedPatterns)
        {
            if (sourceCode.Contains(pattern))
            {
                Console.WriteLine($"⚠️  Deprecated pattern found: {pattern}");
                Console.WriteLine($"💡 Consider updating to the async equivalent");
            }
        }
    }
}
```

## Common Migration Scenarios

### Scenario 1: Upgrading from Pre-1.0 Alpha/Beta

**If migrating from alpha/beta versions (not publicly released):**

1. **Complete reinstall** - Remove old packages and install fresh:
   ```bash
   dotnet remove package Carespace.SDK.Alpha
   dotnet add package Carespace.SDK
   ```

2. **Update namespaces** - Ensure you're using the correct namespace:
   ```csharp
   // Old (if applicable)
   using CarespaceSDK.Alpha;
   
   // New
   using CarespaceSDK;
   ```

3. **Update initialization** - Use the new initialization patterns:
   ```csharp
   // New approach
   using var client = CarespaceClient.CreateForDevelopment("api-key");
   ```

### Scenario 2: Framework Migration (.NET Framework to .NET 6+)

**If migrating from .NET Framework:**

1. **Update target framework** in `.csproj`:
   ```xml
   <TargetFramework>net6.0</TargetFramework>
   ```

2. **Enable nullable reference types**:
   ```xml
   <Nullable>enable</Nullable>
   ```

3. **Update async patterns** - Ensure proper async/await usage:
   ```csharp
   // Old synchronous pattern (not recommended)
   var users = client.Users.GetUsersAsync().Result;
   
   // New async pattern
   var users = await client.Users.GetUsersAsync();
   ```

### Scenario 3: Dependency Injection Migration

**Moving to DI container:**

1. **Update service registration**:
   ```csharp
   // Program.cs or Startup.cs
   builder.Services.AddCarespaceSDKForDevelopment("api-key");
   ```

2. **Update service constructors**:
   ```csharp
   public class UserService
   {
       private readonly CarespaceClient _carespace;
       
       public UserService(CarespaceClient carespace)
       {
           _carespace = carespace;
       }
   }
   ```

### Scenario 4: Configuration Migration

**Moving from hardcoded to configuration-based setup:**

1. **Add configuration section** to `appsettings.json`:
   ```json
   {
     "Carespace": {
       "ApiKey": "your-api-key",
       "BaseUrl": "https://api-dev.carespace.ai",
       "TimeoutSeconds": 30,
       "MaxRetryAttempts": 3,
       "EnableLogging": true
     }
   }
   ```

2. **Update initialization**:
   ```csharp
   // Old hardcoded approach
   var client = new CarespaceClient("hardcoded-key", "hardcoded-url");
   
   // New configuration-based approach
   var configuration = new CarespaceConfiguration
   {
       ApiKey = builder.Configuration["Carespace:ApiKey"],
       BaseUrl = builder.Configuration["Carespace:BaseUrl"],
       // ... other settings
   };
   var client = new CarespaceClient(configuration);
   ```

## Migration Checklist

### Pre-Migration Checklist

- [ ] **Backup project** - Create full backup of your codebase
- [ ] **Review changelog** - Check release notes for breaking changes
- [ ] **Test environment** - Ensure you have a safe testing environment
- [ ] **Dependencies** - Check compatibility of other packages
- [ ] **API keys** - Verify you have the correct API keys for testing

### During Migration Checklist

- [ ] **Update package** - Install new SDK version
- [ ] **Fix compilation errors** - Address any breaking changes
- [ ] **Update imports** - Verify all using statements are correct
- [ ] **Update initialization** - Modify client creation code
- [ ] **Update API calls** - Adapt to new method signatures
- [ ] **Update error handling** - Adjust exception handling if needed

### Post-Migration Checklist

- [ ] **Compile successfully** - Ensure project builds without errors
- [ ] **Run unit tests** - Verify all tests pass
- [ ] **Integration testing** - Test with actual API endpoints
- [ ] **Performance testing** - Verify no performance regressions
- [ ] **Documentation** - Update internal documentation
- [ ] **Team notification** - Inform team of changes

## Testing Your Migration

### Unit Test Migration

```csharp
[Fact]
public async Task Migration_ShouldMaintainFunctionality()
{
    // Arrange
    var client = CarespaceClient.CreateForDevelopment("test-api-key");
    
    // Act
    var response = await client.Users.GetUsersAsync();
    
    // Assert
    Assert.NotNull(response);
    // Add assertions based on your expected behavior
}
```

### Integration Test Migration

```csharp
[Fact]
public async Task Migration_IntegrationTest()
{
    // Test against development environment
    var client = CarespaceClient.CreateForDevelopment(
        Environment.GetEnvironmentVariable("CARESPACE_TEST_API_KEY"));
    
    var healthCheck = await client.HealthCheckAsync();
    Assert.True(healthCheck);
}
```

## Troubleshooting

### Common Issues and Solutions

#### Issue: "Package not found" errors

**Solution:**
```bash
# Clear NuGet cache
dotnet nuget locals all --clear

# Restore packages
dotnet restore
```

#### Issue: Compilation errors after upgrade

**Solution:**
1. Check for breaking changes in the changelog
2. Update method signatures according to new API
3. Ensure you're using the correct namespace

#### Issue: Runtime exceptions

**Solution:**
1. Check exception details - the SDK provides specific exception types
2. Verify API key is correct for the environment
3. Check network connectivity and API endpoint availability

#### Issue: Performance degradation

**Solution:**
1. Review new configuration options for performance tuning
2. Check if retry logic settings need adjustment
3. Verify you're using async/await properly

### Getting Help

If you encounter issues during migration:

1. **Check documentation** - Review API documentation and examples
2. **GitHub Issues** - Search existing issues or create a new one
3. **Stack Overflow** - Use tags: `carespace-sdk`, `.net`, `c#`
4. **Contact Support** - Email: support@carespace.ai

### Migration Support Script

```bash
#!/bin/bash
# migration-helper.sh
# Helper script for SDK migration

echo "🔄 Starting Carespace SDK Migration Helper"

echo "📦 Backing up project..."
timestamp=$(date +%Y%m%d_%H%M%S)
tar -czf "backup_${timestamp}.tar.gz" --exclude=node_modules --exclude=bin --exclude=obj .

echo "🧹 Cleaning build artifacts..."
dotnet clean

echo "📋 Checking current SDK version..."
dotnet list package | grep Carespace.SDK

echo "⬆️ Please update your package reference and run 'dotnet restore'"
echo "✅ Migration helper complete - proceed with manual updates"
```

## Best Practices for Future Migrations

1. **Stay Updated** - Regularly check for new releases and security updates
2. **Test Early** - Test new versions in development environments
3. **Incremental Updates** - Update frequently rather than skipping versions
4. **Documentation** - Keep internal documentation updated with each migration
5. **Automation** - Consider automating migration testing in CI/CD pipelines

## Version Support Policy

- **Current Version**: Full support with new features and bug fixes
- **Previous Major Version**: Security updates and critical bug fixes for 12 months
- **Older Versions**: Community support only

Plan your migrations accordingly to stay within supported versions.

---

**Need Help?** If you have questions about migrating to a new version of the Carespace .NET SDK, please:

- 📧 Contact: [support@carespace.ai](mailto:support@carespace.ai)
- 🐛 Report Issues: [GitHub Issues](https://github.com/carespace/sdk-monorepo/issues)
- 💬 Join Discussion: [GitHub Discussions](https://github.com/carespace/sdk-monorepo/discussions)