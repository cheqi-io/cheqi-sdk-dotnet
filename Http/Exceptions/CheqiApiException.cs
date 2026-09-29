namespace Cheqi.Sdk.Http.Exceptions;

/// <summary>
/// Exception thrown when Cheqi API communication fails.
///
/// This exception provides detailed information about API failures including:
/// - HTTP status codes
/// - Error codes for programmatic handling
/// - Correlation IDs for debugging
/// - Detailed error messages
/// - Retry information
/// </summary>
public class CheqiApiException : Exception
{
    public int HttpStatusCode { get; }
    public string ErrorCode { get; }
    public string? CorrelationId { get; }
    public virtual bool Retryable { get; }
    public long Timestamp { get; }

    /// <summary>
    /// Creates a CheqiApiException with full error details.
    /// </summary>
    /// <param name="message">Human-readable error message</param>
    /// <param name="httpStatusCode">HTTP status code from the API response</param>
    /// <param name="errorCode">Machine-readable error code for programmatic handling</param>
    /// <param name="correlationId">Correlation ID for request tracing</param>
    public CheqiApiException(string message, int httpStatusCode, string errorCode, string? correlationId) 
        : base(message)
    {
        HttpStatusCode = httpStatusCode;
        ErrorCode = errorCode;
        CorrelationId = correlationId;
        Retryable = IsRetryableStatus(httpStatusCode);
        Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }

    /// <summary>
    /// Creates a CheqiApiException with cause and full error details.
    /// </summary>
    /// <param name="message">Human-readable error message</param>
    /// <param name="cause">The underlying cause of the exception</param>
    /// <param name="httpStatusCode">HTTP status code from the API response</param>
    /// <param name="errorCode">Machine-readable error code for programmatic handling</param>
    /// <param name="correlationId">Correlation ID for request tracing</param>
    public CheqiApiException(string message, Exception cause, int httpStatusCode, string errorCode, string? correlationId) 
        : base(message, cause)
    {
        HttpStatusCode = httpStatusCode;
        ErrorCode = errorCode;
        CorrelationId = correlationId;
        Retryable = IsRetryableStatus(httpStatusCode);
        Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }

    /// <summary>
    /// Creates a CheqiApiException for general failures without HTTP details.
    /// </summary>
    /// <param name="message">Human-readable error message</param>
    public CheqiApiException(string message) : base(message)
    {
        HttpStatusCode = 0;
        ErrorCode = "UNKNOWN_ERROR";
        CorrelationId = null;
        Retryable = false;
        Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }

    /// <summary>
    /// Creates a CheqiApiException for general failures with cause.
    /// </summary>
    /// <param name="message">Human-readable error message</param>
    /// <param name="cause">The underlying cause of the exception</param>
    public CheqiApiException(string message, Exception cause) : base(message, cause)
    {
        HttpStatusCode = 0;
        ErrorCode = "UNKNOWN_ERROR";
        CorrelationId = null;
        Retryable = false;
        Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }

    /// <summary>
    /// Indicates whether this error is potentially retryable.
    /// </summary>
    /// <returns>true if the operation might succeed on retry, false otherwise</returns>
    public bool IsRetryable() => Retryable;

    /// <summary>
    /// Checks if this is an authentication/authorization error.
    /// </summary>
    /// <returns>true if this is a 401 or 403 error</returns>
    public bool IsAuthenticationError() => HttpStatusCode == 401 || HttpStatusCode == 403;

    /// <summary>
    /// Checks if this is a client error (4xx status codes).
    /// </summary>
    /// <returns>true if this is a client error</returns>
    public bool IsClientError() => HttpStatusCode >= 400 && HttpStatusCode < 500;

    /// <summary>
    /// Checks if this is a server error (5xx status codes).
    /// </summary>
    /// <returns>true if this is a server error</returns>
    public bool IsServerError() => HttpStatusCode >= 500 && HttpStatusCode < 600;

    /// <summary>
    /// Checks if this is a rate limiting error.
    /// </summary>
    /// <returns>true if this is a 429 (Too Many Requests) error</returns>
    public bool IsRateLimitError() => HttpStatusCode == 429;

    /// <summary>
    /// Creates a detailed string representation of the exception.
    /// </summary>
    /// <returns>Formatted error details</returns>
    public override string ToString()
    {
        var sb = new System.Text.StringBuilder();
        sb.Append("CheqiApiException{");
        sb.Append($"message='{Message}'");

        if (HttpStatusCode > 0)
        {
            sb.Append($", httpStatusCode={HttpStatusCode}");
        }

        if (ErrorCode != null)
        {
            sb.Append($", errorCode='{ErrorCode}'");
        }

        if (CorrelationId != null)
        {
            sb.Append($", correlationId='{CorrelationId}'");
        }

        sb.Append($", retryable={Retryable}");
        sb.Append($", timestamp={Timestamp}");
        sb.Append('}');

        return sb.ToString();
    }

    /// <summary>
    /// Determines if an HTTP status code indicates a retryable error.
    /// </summary>
    /// <param name="statusCode">HTTP status code</param>
    /// <returns>true if the error is potentially retryable</returns>
    private static bool IsRetryableStatus(int statusCode)
    {
        // Server errors (5xx) are generally retryable
        if (statusCode >= 500 && statusCode < 600)
        {
            return true;
        }

        // Rate limiting is retryable with backoff
        if (statusCode == 429)
        {
            return true;
        }

        // Request timeout is retryable
        if (statusCode == 408)
        {
            return true;
        }

        // Client errors (4xx) are generally not retryable
        // Authentication errors (401, 403) are not retryable
        // Bad request (400) is not retryable
        // Not found (404) is not retryable
        return false;
    }

    /// <summary>
    /// Common error codes used throughout the SDK.
    /// </summary>
    public static class ErrorCodes
    {
        public const string AUTHENTICATION_FAILED = "AUTHENTICATION_FAILED";
        public const string AUTHORIZATION_FAILED = "AUTHORIZATION_FAILED";
        public const string TEMPLATE_GENERATION_FAILED = "TEMPLATE_GENERATION_FAILED";
        public const string INVALID_REQUEST = "INVALID_REQUEST";
        public const string RATE_LIMIT_EXCEEDED = "RATE_LIMIT_EXCEEDED";
        public const string SERVER_ERROR = "SERVER_ERROR";
        public const string NETWORK_ERROR = "NETWORK_ERROR";
        public const string TIMEOUT_ERROR = "TIMEOUT_ERROR";
        public const string UNKNOWN_ERROR = "UNKNOWN_ERROR";
        public const string NULL_TEMPLATE = "NULL_TEMPLATE";
        public const string INVALID_RESPONSE = "INVALID_RESPONSE";
    }
}
