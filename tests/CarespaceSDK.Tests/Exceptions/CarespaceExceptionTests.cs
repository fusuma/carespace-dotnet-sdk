using System.Net;
using Xunit;
using FluentAssertions;
using CarespaceSDK.Exceptions;

namespace CarespaceSDK.Tests.Exceptions;

public class CarespaceExceptionTests
{
    [Fact]
    public void CarespaceException_WithMessage_ShouldSetMessage()
    {
        var message = "Test exception message";
        var exception = new CarespaceException(message);

        exception.Message.Should().Be(message);
        exception.StatusCode.Should().BeNull();
        exception.ErrorCode.Should().BeNull();
        exception.ErrorDetails.Should().BeNull();
    }

    [Fact]
    public void CarespaceException_WithMessageAndStatusCode_ShouldSetProperties()
    {
        var message = "Test exception message";
        var statusCode = HttpStatusCode.BadRequest;
        var errorCode = "TEST_ERROR";
        var errorDetails = new { field = "value" };

        var exception = new CarespaceException(message, statusCode, errorCode, errorDetails);

        exception.Message.Should().Be(message);
        exception.StatusCode.Should().Be(statusCode);
        exception.ErrorCode.Should().Be(errorCode);
        exception.ErrorDetails.Should().Be(errorDetails);
    }

    [Fact]
    public void CarespaceAuthenticationException_ShouldHaveCorrectProperties()
    {
        var message = "Authentication failed";
        var exception = new CarespaceAuthenticationException(message);

        exception.Message.Should().Be(message);
        exception.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        exception.ErrorCode.Should().Be("AUTHENTICATION_FAILED");
    }

    [Fact]
    public void CarespaceAuthorizationException_ShouldHaveCorrectProperties()
    {
        var message = "Authorization failed";
        var exception = new CarespaceAuthorizationException(message);

        exception.Message.Should().Be(message);
        exception.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        exception.ErrorCode.Should().Be("AUTHORIZATION_FAILED");
    }

    [Fact]
    public void CarespaceNotFoundException_ShouldHaveCorrectProperties()
    {
        var message = "Resource not found";
        var exception = new CarespaceNotFoundException(message);

        exception.Message.Should().Be(message);
        exception.StatusCode.Should().Be(HttpStatusCode.NotFound);
        exception.ErrorCode.Should().Be("RESOURCE_NOT_FOUND");
    }

    [Fact]
    public void CarespaceValidationException_ShouldHaveCorrectProperties()
    {
        var message = "Validation failed";
        var validationErrors = new { field = "error" };
        var exception = new CarespaceValidationException(message, validationErrors);

        exception.Message.Should().Be(message);
        exception.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        exception.ErrorCode.Should().Be("VALIDATION_FAILED");
        exception.ErrorDetails.Should().Be(validationErrors);
    }

    [Fact]
    public void CarespaceRateLimitException_ShouldHaveCorrectProperties()
    {
        var message = "Rate limit exceeded";
        var retryAfter = TimeSpan.FromMinutes(5);
        var exception = new CarespaceRateLimitException(message, retryAfter);

        exception.Message.Should().Be(message);
        exception.StatusCode.Should().Be(HttpStatusCode.TooManyRequests);
        exception.ErrorCode.Should().Be("RATE_LIMIT_EXCEEDED");
        exception.RetryAfter.Should().Be(retryAfter);
    }

    [Fact]
    public void CarespaceServerException_ShouldHaveCorrectProperties()
    {
        var message = "Internal server error";
        var exception = new CarespaceServerException(message);

        exception.Message.Should().Be(message);
        exception.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        exception.ErrorCode.Should().Be("SERVER_ERROR");
    }
}