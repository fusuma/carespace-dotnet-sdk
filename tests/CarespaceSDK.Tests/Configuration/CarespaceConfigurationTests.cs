using Xunit;
using FluentAssertions;
using CarespaceSDK.Configuration;

namespace CarespaceSDK.Tests.Configuration;

public class CarespaceConfigurationTests
{
    [Fact]
    public void DefaultConfiguration_ShouldHaveValidDefaults()
    {
        var config = new CarespaceConfiguration();

        config.BaseUrl.Should().Be("https://api-dev.carespace.ai");
        config.Timeout.Should().Be(TimeSpan.FromSeconds(30));
        config.MaxRetryAttempts.Should().Be(3);
        config.RetryDelay.Should().Be(TimeSpan.FromSeconds(1));
        config.UserAgent.Should().Be("CarespaceSDK/1.0.0 (.NET)");
        config.EnableRetry.Should().BeTrue();
        config.EnableLogging.Should().BeTrue();
    }

    [Fact]
    public void ForDevelopment_ShouldCreateDevelopmentConfiguration()
    {
        var config = CarespaceConfiguration.ForDevelopment("test-api-key");

        config.BaseUrl.Should().Be("https://api-dev.carespace.ai");
        config.ApiKey.Should().Be("test-api-key");
    }

    [Fact]
    public void ForProduction_ShouldCreateProductionConfiguration()
    {
        var config = CarespaceConfiguration.ForProduction("test-api-key");

        config.BaseUrl.Should().Be("https://api.carespace.ai");
        config.ApiKey.Should().Be("test-api-key");
    }

    [Fact]
    public void ForStaging_ShouldCreateStagingConfiguration()
    {
        var config = CarespaceConfiguration.ForStaging("test-api-key");

        config.BaseUrl.Should().Be("https://api-staging.carespace.ai");
        config.ApiKey.Should().Be("test-api-key");
    }

    [Fact]
    public void Validate_WithValidConfiguration_ShouldNotThrow()
    {
        var config = new CarespaceConfiguration
        {
            BaseUrl = "https://api.example.com",
            Timeout = TimeSpan.FromSeconds(30),
            MaxRetryAttempts = 3,
            RetryDelay = TimeSpan.FromSeconds(1)
        };

        var action = () => config.Validate();
        action.Should().NotThrow();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Validate_WithInvalidBaseUrl_ShouldThrow(string baseUrl)
    {
        var config = new CarespaceConfiguration { BaseUrl = baseUrl };

        var action = () => config.Validate();
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Validate_WithInvalidUri_ShouldThrow()
    {
        var config = new CarespaceConfiguration { BaseUrl = "not-a-valid-uri" };

        var action = () => config.Validate();
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Validate_WithZeroTimeout_ShouldThrow()
    {
        var config = new CarespaceConfiguration { Timeout = TimeSpan.Zero };

        var action = () => config.Validate();
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Validate_WithNegativeMaxRetryAttempts_ShouldThrow()
    {
        var config = new CarespaceConfiguration { MaxRetryAttempts = -1 };

        var action = () => config.Validate();
        action.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Validate_WithNegativeRetryDelay_ShouldThrow()
    {
        var config = new CarespaceConfiguration { RetryDelay = TimeSpan.FromSeconds(-1) };

        var action = () => config.Validate();
        action.Should().Throw<ArgumentException>();
    }
}