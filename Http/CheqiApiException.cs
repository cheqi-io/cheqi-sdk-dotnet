using System.Net;
using Cheqi.Sdk.Exceptions;

namespace Cheqi.Sdk.Http;

/// <summary>
/// Exception thrown when an API request fails.
/// </summary>
public class CheqiApiException : CheqiSdkException
{
    /// <summary>
    /// HTTP status code of the failed request.
    /// </summary>
    public HttpStatusCode StatusCode { get; }

    /// <summary>
    /// Response body from the failed request, if available.
    /// </summary>
    public string? ResponseBody { get; }

    /// <summary>
    /// Request URL that failed.
    /// </summary>
    public string? RequestUrl { get; }

    public CheqiApiException(string message, HttpStatusCode statusCode)
        : base(message)
    {
        StatusCode = statusCode;
    }

    public CheqiApiException(string message, HttpStatusCode statusCode, string? responseBody)
        : base(message)
    {
        StatusCode = statusCode;
        ResponseBody = responseBody;
    }

    public CheqiApiException(string message, HttpStatusCode statusCode, string? responseBody, string? requestUrl)
        : base(message)
    {
        StatusCode = statusCode;
        ResponseBody = responseBody;
        RequestUrl = requestUrl;
    }

    public CheqiApiException(string message, HttpStatusCode statusCode, Exception innerException)
        : base(message, innerException)
    {
        StatusCode = statusCode;
    }

    /// <summary>
    /// Returns true if the error is retryable (5xx errors, rate limiting).
    /// </summary>
    public bool IsRetryable => (int)StatusCode >= 500 || StatusCode == HttpStatusCode.TooManyRequests;
}
