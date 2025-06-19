using System.Net;

namespace CarespaceSDK.Exceptions;

public class CarespaceException : Exception
{
    public HttpStatusCode? StatusCode { get; }
    public string? ErrorCode { get; }
    public object? ErrorDetails { get; }

    public CarespaceException(string message) : base(message)
    {
    }

    public CarespaceException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }

    public CarespaceException(
        string message, 
        HttpStatusCode statusCode, 
        string? errorCode = null, 
        object? errorDetails = null) 
        : base(message)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
        ErrorDetails = errorDetails;
    }

    public CarespaceException(
        string message, 
        HttpStatusCode statusCode, 
        Exception innerException, 
        string? errorCode = null, 
        object? errorDetails = null) 
        : base(message, innerException)
    {
        StatusCode = statusCode;
        ErrorCode = errorCode;
        ErrorDetails = errorDetails;
    }
}

public class CarespaceAuthenticationException : CarespaceException
{
    public CarespaceAuthenticationException(string message) 
        : base(message, HttpStatusCode.Unauthorized, "AUTHENTICATION_FAILED")
    {
    }

    public CarespaceAuthenticationException(string message, Exception innerException) 
        : base(message, HttpStatusCode.Unauthorized, innerException, "AUTHENTICATION_FAILED")
    {
    }
}

public class CarespaceAuthorizationException : CarespaceException
{
    public CarespaceAuthorizationException(string message) 
        : base(message, HttpStatusCode.Forbidden, "AUTHORIZATION_FAILED")
    {
    }

    public CarespaceAuthorizationException(string message, Exception innerException) 
        : base(message, HttpStatusCode.Forbidden, innerException, "AUTHORIZATION_FAILED")
    {
    }
}

public class CarespaceNotFoundException : CarespaceException
{
    public CarespaceNotFoundException(string message) 
        : base(message, HttpStatusCode.NotFound, "RESOURCE_NOT_FOUND")
    {
    }

    public CarespaceNotFoundException(string message, Exception innerException) 
        : base(message, HttpStatusCode.NotFound, innerException, "RESOURCE_NOT_FOUND")
    {
    }
}

public class CarespaceValidationException : CarespaceException
{
    public CarespaceValidationException(string message, object? validationErrors = null) 
        : base(message, HttpStatusCode.BadRequest, "VALIDATION_FAILED", validationErrors)
    {
    }

    public CarespaceValidationException(string message, Exception innerException, object? validationErrors = null) 
        : base(message, HttpStatusCode.BadRequest, innerException, "VALIDATION_FAILED", validationErrors)
    {
    }
}

public class CarespaceRateLimitException : CarespaceException
{
    public TimeSpan? RetryAfter { get; }

    public CarespaceRateLimitException(string message, TimeSpan? retryAfter = null) 
        : base(message, HttpStatusCode.TooManyRequests, "RATE_LIMIT_EXCEEDED")
    {
        RetryAfter = retryAfter;
    }

    public CarespaceRateLimitException(string message, Exception innerException, TimeSpan? retryAfter = null) 
        : base(message, HttpStatusCode.TooManyRequests, innerException, "RATE_LIMIT_EXCEEDED")
    {
        RetryAfter = retryAfter;
    }
}

public class CarespaceServerException : CarespaceException
{
    public CarespaceServerException(string message) 
        : base(message, HttpStatusCode.InternalServerError, "SERVER_ERROR")
    {
    }

    public CarespaceServerException(string message, Exception innerException) 
        : base(message, HttpStatusCode.InternalServerError, innerException, "SERVER_ERROR")
    {
    }
}