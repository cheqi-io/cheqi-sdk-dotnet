namespace Cheqi.Sdk.Exceptions;

/// <summary>
/// Main exception class for Cheqi SDK operations.
///
/// This exception is thrown when SDK operations fail due to:
/// - Invalid input parameters or configuration
/// - Network connectivity issues
/// - Backend API errors
/// - Encryption/decryption failures
/// - Authentication or authorization problems
///
/// The exception provides detailed error information including:
/// - Human-readable error messages
/// - Error codes for programmatic handling
/// - HTTP status codes when applicable
/// - Correlation IDs for debugging
/// - Underlying cause exceptions
/// </summary>
public class CheqiSdkException : Exception
{
    public string ErrorCode { get; }
    public int HttpStatusCode { get; }
    public string? CorrelationId { get; }

    /// <summary>
    /// Creates a new CheqiSdkException with a message.
    /// </summary>
    /// <param name="message">Human-readable error message</param>
    public CheqiSdkException(string message) : base(message)
    {
        ErrorCode = ErrorCodes.UNKNOWN_ERROR;
        HttpStatusCode = 0;
        CorrelationId = null;
    }

    /// <summary>
    /// Creates a new CheqiSdkException with a message and cause.
    /// </summary>
    /// <param name="message">Human-readable error message</param>
    /// <param name="cause">Underlying exception that caused this error</param>
    public CheqiSdkException(string message, Exception cause) : base(message, cause)
    {
        ErrorCode = ErrorCodes.UNKNOWN_ERROR;
        HttpStatusCode = 0;
        CorrelationId = null;
    }

    /// <summary>
    /// Creates a new CheqiSdkException with detailed error information.
    /// </summary>
    /// <param name="message">Human-readable error message</param>
    /// <param name="errorCode">SDK-specific error code for programmatic handling</param>
    /// <param name="httpStatusCode">HTTP status code if this error originated from an API call</param>
    /// <param name="correlationId">Correlation ID for debugging and tracing</param>
    public CheqiSdkException(string message, string errorCode, int httpStatusCode, string? correlationId) : base(message)
    {
        ErrorCode = errorCode;
        HttpStatusCode = httpStatusCode;
        CorrelationId = correlationId;
    }

    /// <summary>
    /// Creates a new CheqiSdkException with detailed error information and cause.
    /// </summary>
    /// <param name="message">Human-readable error message</param>
    /// <param name="cause">Underlying exception that caused this error</param>
    /// <param name="errorCode">SDK-specific error code for programmatic handling</param>
    /// <param name="httpStatusCode">HTTP status code if this error originated from an API call</param>
    /// <param name="correlationId">Correlation ID for debugging and tracing</param>
    public CheqiSdkException(string message, Exception cause, string errorCode, int httpStatusCode, string? correlationId)
        : base(message, cause)
    {
        ErrorCode = errorCode;
        HttpStatusCode = httpStatusCode;
        CorrelationId = correlationId;
    }

    /// <summary>
    /// Checks if this exception has an HTTP status code.
    /// </summary>
    /// <returns>true if httpStatusCode > 0</returns>
    public bool HasHttpStatusCode() => HttpStatusCode > 0;

    /// <summary>
    /// Checks if this exception has a correlation ID.
    /// </summary>
    /// <returns>true if correlationId is not null and not empty</returns>
    public bool HasCorrelationId() => !string.IsNullOrWhiteSpace(CorrelationId);

    /// <summary>
    /// Checks if this is a client error (4xx HTTP status code).
    /// </summary>
    /// <returns>true if HTTP status code is between 400-499</returns>
    public bool IsClientError() => HttpStatusCode >= 400 && HttpStatusCode < 500;

    /// <summary>
    /// Checks if this is a server error (5xx HTTP status code).
    /// </summary>
    /// <returns>true if HTTP status code is between 500-599</returns>
    public bool IsServerError() => HttpStatusCode >= 500 && HttpStatusCode < 600;

    /// <summary>
    /// Checks if this is an authentication error.
    /// </summary>
    /// <returns>true if error code indicates authentication failure</returns>
    public bool IsAuthenticationError() =>
        ErrorCode == ErrorCodes.AUTHENTICATION_FAILED ||
        ErrorCode == ErrorCodes.INVALID_ACCESS_TOKEN;

    /// <summary>
    /// Checks if this is an authorization error.
    /// </summary>
    /// <returns>true if error code indicates authorization failure</returns>
    public bool IsAuthorizationError() =>
        ErrorCode == ErrorCodes.AUTHORIZATION_FAILED ||
        ErrorCode == ErrorCodes.INSUFFICIENT_PERMISSIONS;

    /// <summary>
    /// Checks if this is a validation error.
    /// </summary>
    /// <returns>true if error code indicates validation failure</returns>
    public bool IsValidationError() =>
        ErrorCode == ErrorCodes.VALIDATION_ERROR ||
        ErrorCode == ErrorCodes.INVALID_REQUEST;

    /// <summary>
    /// Checks if this is a network-related error.
    /// </summary>
    /// <returns>true if error code indicates network issues</returns>
    public bool IsNetworkError() =>
        ErrorCode == ErrorCodes.NETWORK_ERROR ||
        ErrorCode == ErrorCodes.TIMEOUT_ERROR;

    public override string ToString()
    {
        var sb = new System.Text.StringBuilder();
        sb.Append("CheqiSdkException{");
        sb.Append($"message='{Message}'");
        sb.Append($", errorCode='{ErrorCode}'");
        if (HttpStatusCode > 0)
        {
            sb.Append($", httpStatusCode={HttpStatusCode}");
        }
        if (HasCorrelationId())
        {
            sb.Append($", correlationId='{CorrelationId}'");
        }
        if (InnerException != null)
        {
            sb.Append($", cause={InnerException.GetType().Name}");
        }
        sb.Append('}');
        return sb.ToString();
    }

    /// <summary>
    /// Standard error codes used throughout the Cheqi SDK.
    /// </summary>
    public static class ErrorCodes
    {
        // General errors
        public const string UNKNOWN_ERROR = "UNKNOWN_ERROR";
        public const string INVALID_REQUEST = "INVALID_REQUEST";
        public const string VALIDATION_ERROR = "VALIDATION_ERROR";
        public const string CONFIGURATION_ERROR = "CONFIGURATION_ERROR";

        // Authentication and authorization
        public const string AUTHENTICATION_FAILED = "AUTHENTICATION_FAILED";
        public const string AUTHORIZATION_FAILED = "AUTHORIZATION_FAILED";
        public const string INVALID_ACCESS_TOKEN = "INVALID_ACCESS_TOKEN";
        public const string INSUFFICIENT_PERMISSIONS = "INSUFFICIENT_PERMISSIONS";

        // Network and connectivity
        public const string NETWORK_ERROR = "NETWORK_ERROR";
        public const string TIMEOUT_ERROR = "TIMEOUT_ERROR";
        public const string RATE_LIMIT_EXCEEDED = "RATE_LIMIT_EXCEEDED";

        // API and server errors
        public const string SERVER_ERROR = "SERVER_ERROR";
        public const string INVALID_RESPONSE = "INVALID_RESPONSE";
        public const string SERVICE_UNAVAILABLE = "SERVICE_UNAVAILABLE";

        // SDK-specific errors
        public const string ENCRYPTION_ERROR = "ENCRYPTION_ERROR";
        public const string DECRYPTION_ERROR = "DECRYPTION_ERROR";
        public const string KEY_GENERATION_ERROR = "KEY_GENERATION_ERROR";
        public const string CUSTOMER_NOT_FOUND = "CUSTOMER_NOT_FOUND";
        public const string RECEIPT_GENERATION_ERROR = "RECEIPT_GENERATION_ERROR";
        public const string TEMPLATE_PROCESSING_ERROR = "TEMPLATE_PROCESSING_ERROR";

        // Data validation errors
        public const string MISSING_REQUIRED_FIELD = "MISSING_REQUIRED_FIELD";
        public const string INVALID_FIELD_VALUE = "INVALID_FIELD_VALUE";
        public const string INVALID_CURRENCY_CODE = "INVALID_CURRENCY_CODE";
        public const string INVALID_AMOUNT = "INVALID_AMOUNT";
        public const string INVALID_DATE_FORMAT = "INVALID_DATE_FORMAT";
    }
}
