using Cheqi.Sdk.Http.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Net.Http;
using System.Text.Json;

namespace Cheqi.Sdk.Http;

/// <summary>
/// Handles HTTP response processing including error handling and deserialization.
/// </summary>
public class ResponseHandler
{
    private readonly ILogger _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public ResponseHandler(ILogger? logger = null, JsonSerializerOptions? jsonOptions = null)
    {
        _logger = logger ?? NullLogger<ResponseHandler>.Instance;
        _jsonOptions = jsonOptions ?? Serialization.CheqiJson.CreateOptions();
    }

    /// <summary>
    /// Processes a response and returns the body as a string.
    /// </summary>
    public async Task<string> HandleStringResponseAsync(HttpResponseMessage response, string operationName, CancellationToken cancellationToken = default)
    {
        string responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        string? correlationId = response.Headers.TryGetValues("X-Correlation-ID", out var values) 
            ? values.FirstOrDefault() 
            : null;

        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation("{OperationName} successful", operationName);

            if (string.IsNullOrWhiteSpace(responseContent))
            {
                throw new CheqiApiException(
                    "Backend returned empty response",
                    (int)response.StatusCode,
                    CheqiApiException.ErrorCodes.INVALID_RESPONSE,
                    correlationId
                );
            }
            return responseContent;
        }
        else
        {
            throw BuildApiException((int)response.StatusCode, responseContent, correlationId, operationName);
        }
    }

    /// <summary>
    /// Processes a response and deserializes to specified type.
    /// </summary>
    public async Task<T> HandleJsonResponseAsync<T>(HttpResponseMessage response, string operationName, CancellationToken cancellationToken = default)
    {
        string responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
        string? correlationId = response.Headers.TryGetValues("X-Correlation-ID", out var values) 
            ? values.FirstOrDefault() 
            : null;

        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation("{OperationName} successful", operationName);

            if (string.IsNullOrWhiteSpace(responseJson))
            {
                throw new CheqiApiException(
                    "Backend returned empty response",
                    (int)response.StatusCode,
                    CheqiApiException.ErrorCodes.INVALID_RESPONSE,
                    correlationId
                );
            }

            if (typeof(T) == typeof(Cheqi.Sdk.Models.Generated.ReceiptSubmissionResponse))
            {
                try
                {
                    using var document = JsonDocument.Parse(responseJson);
                    if (document.RootElement.TryGetProperty("state", out var state) && state.GetString() == "IN_PROGRESS")
                    {
                        int? retryAfter = response.Headers.RetryAfter?.Delta is TimeSpan delay
                            ? (int)Math.Ceiling(delay.TotalSeconds)
                            : response.Headers.RetryAfter?.Date is DateTimeOffset date
                                ? Math.Max(0, (int)Math.Ceiling((date - DateTimeOffset.UtcNow).TotalSeconds)) : null;
                        if (retryAfter == null && document.RootElement.TryGetProperty("retryAfterSeconds", out var seconds) && seconds.TryGetInt32(out var value)) retryAfter = value;
                        throw new SubmissionInProgressException("Submission in progress", correlationId, Math.Max(1, retryAfter ?? 2));
                    }
                }
                catch (Exception exception) when (exception is JsonException or InvalidOperationException or FormatException)
                { throw new CheqiApiException("Invalid JSON response", exception, (int)response.StatusCode, CheqiApiException.ErrorCodes.INVALID_RESPONSE, correlationId); }
            }
            T? result;
            try { result = JsonSerializer.Deserialize<T>(responseJson, _jsonOptions); }
            catch (Exception exception) when (exception is JsonException or ArgumentException or NotImplementedException or InvalidOperationException or FormatException)
            { throw new CheqiApiException("Invalid JSON response", exception, (int)response.StatusCode, CheqiApiException.ErrorCodes.INVALID_RESPONSE, correlationId); }
            if (result == null)
            {
                throw new CheqiApiException(
                    "Backend returned null response",
                    (int)response.StatusCode,
                    CheqiApiException.ErrorCodes.INVALID_RESPONSE,
                    correlationId
                );
            }
            if (result is Cheqi.Sdk.Models.Generated.ReceiptSubmissionResponse submission &&
                (string.IsNullOrWhiteSpace(submission.CheqiReceiptId) || string.IsNullOrWhiteSpace(submission.MatchId)))
                throw new CheqiApiException("Invalid receipt submission response", (int)response.StatusCode, CheqiApiException.ErrorCodes.INVALID_RESPONSE, correlationId);
            return result;
        }
        else
        {
            throw BuildApiException((int)response.StatusCode, responseJson, correlationId, operationName);
        }
    }

    /// <summary>
    /// Handles void responses (201 Created, 204 No Content).
    /// </summary>
    public async Task HandleVoidResponseAsync(HttpResponseMessage response, string operationName, CancellationToken cancellationToken = default)
    {
        string responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        string? correlationId = response.Headers.TryGetValues("X-Correlation-ID", out var values) 
            ? values.FirstOrDefault() 
            : null;

        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation("{OperationName} successful", operationName);
            return;
        }
        else
        {
            throw BuildApiException((int)response.StatusCode, responseContent, correlationId, operationName);
        }
    }

    /// <summary>
    /// Processes a response and deserializes to a list of specified type.
    /// </summary>
    public async Task<List<T>> HandleJsonListResponseAsync<T>(HttpResponseMessage response, string operationName, CancellationToken cancellationToken = default)
    {
        string responseJson = await response.Content.ReadAsStringAsync(cancellationToken);
        string? correlationId = response.Headers.TryGetValues("X-Correlation-ID", out var values) 
            ? values.FirstOrDefault() 
            : null;

        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation("{OperationName} successful", operationName);

            if (string.IsNullOrWhiteSpace(responseJson))
            {
                return new List<T>();
            }

            var result = JsonSerializer.Deserialize<List<T>>(responseJson, _jsonOptions);
            return result ?? new List<T>();
        }
        else
        {
            throw BuildApiException((int)response.StatusCode, responseJson, correlationId, operationName);
        }
    }

    private CheqiApiException BuildApiException(int statusCode, string responseContent, string? correlationId, string operationName)
    {
        _logger.LogError("{OperationName} failed. Status: {StatusCode}",
            operationName, statusCode);

        if (statusCode == 401)
        {
            return new CheqiApiException(
                "Invalid or expired access token",
                statusCode,
                CheqiApiException.ErrorCodes.AUTHENTICATION_FAILED,
                correlationId
            );
        }

        if (statusCode == 403)
        {
            return new CheqiApiException(
                "Access token does not have required permissions",
                statusCode,
                CheqiApiException.ErrorCodes.AUTHORIZATION_FAILED,
                correlationId
            );
        }

        string errorCode = DetermineErrorCode(statusCode);
        string errorMessage = ParseErrorMessage(responseContent, $"{operationName} failed");

        return new CheqiApiException(errorMessage, statusCode, errorCode, correlationId);
    }

    private string DetermineErrorCode(int statusCode)
    {
        return statusCode switch
        {
            400 => CheqiApiException.ErrorCodes.INVALID_REQUEST,
            401 => CheqiApiException.ErrorCodes.AUTHENTICATION_FAILED,
            403 => CheqiApiException.ErrorCodes.AUTHORIZATION_FAILED,
            408 => CheqiApiException.ErrorCodes.TIMEOUT_ERROR,
            429 => CheqiApiException.ErrorCodes.RATE_LIMIT_EXCEEDED,
            500 or 502 or 503 or 504 => CheqiApiException.ErrorCodes.SERVER_ERROR,
            _ => CheqiApiException.ErrorCodes.UNKNOWN_ERROR
        };
    }

    private string ParseErrorMessage(string responseJson, string defaultMessage)
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(responseJson))
            {
                using var doc = JsonDocument.Parse(responseJson);
                var root = doc.RootElement;
                
                if (root.TryGetProperty("message", out var message))
                    return message.GetString() ?? defaultMessage;
                if (root.TryGetProperty("error", out var error))
                    return error.GetString() ?? defaultMessage;
                if (root.TryGetProperty("error_description", out var errorDesc))
                    return errorDesc.GetString() ?? defaultMessage;
            }
        }
        catch (Exception e)
        {
            _logger.LogDebug("Failed to parse error message from response: {Message}", e.Message);
        }
        return defaultMessage;
    }
}
