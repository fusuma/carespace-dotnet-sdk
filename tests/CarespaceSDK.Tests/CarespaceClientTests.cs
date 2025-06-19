using Xunit;
using FluentAssertions;
using CarespaceSDK.Configuration;
using CarespaceSDK.Exceptions;

namespace CarespaceSDK.Tests;

public class CarespaceClientTests : IDisposable
{
    private readonly CarespaceClient _client;

    public CarespaceClientTests()
    {
        var configuration = CarespaceConfiguration.ForDevelopment();
        _client = new CarespaceClient(configuration);
    }

    [Fact]
    public void Constructor_WithValidConfiguration_ShouldCreateClient()
    {
        var configuration = CarespaceConfiguration.ForDevelopment();
        using var client = new CarespaceClient(configuration);

        client.Should().NotBeNull();
        client.Auth.Should().NotBeNull();
        client.Users.Should().NotBeNull();
        client.Clients.Should().NotBeNull();
        client.Programs.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_WithApiKeyAndBaseUrl_ShouldCreateClient()
    {
        using var client = new CarespaceClient("test-api-key", "https://api.example.com");

        client.Should().NotBeNull();
    }

    [Fact]
    public void CreateForDevelopment_ShouldCreateClientWithDevelopmentConfig()
    {
        using var client = CarespaceClient.CreateForDevelopment("test-api-key");

        client.Should().NotBeNull();
    }

    [Fact]
    public void CreateForProduction_ShouldCreateClientWithProductionConfig()
    {
        using var client = CarespaceClient.CreateForProduction("test-api-key");

        client.Should().NotBeNull();
    }

    [Fact]
    public void CreateForStaging_ShouldCreateClientWithStagingConfig()
    {
        using var client = CarespaceClient.CreateForStaging("test-api-key");

        client.Should().NotBeNull();
    }

    [Fact]
    public void SetApiKey_ShouldNotThrow()
    {
        var action = () => _client.SetApiKey("new-api-key");
        action.Should().NotThrow();
    }

    [Fact]
    public void SetDefaultHeader_ShouldNotThrow()
    {
        var action = () => _client.SetDefaultHeader("X-Custom-Header", "test-value");
        action.Should().NotThrow();
    }

    [Fact]
    public void RemoveDefaultHeader_ShouldNotThrow()
    {
        var action = () => _client.RemoveDefaultHeader("X-Custom-Header");
        action.Should().NotThrow();
    }

    public void Dispose()
    {
        _client?.Dispose();
    }
}