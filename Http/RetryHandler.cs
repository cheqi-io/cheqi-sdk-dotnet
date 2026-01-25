using System.Net;
using Cheqi.Sdk.Http.Exceptions;
using Microsoft.Extensions.Logging;

namespace Cheqi.Sdk.Http;

/// <summary>
/// Delegating handler that implements retry logic with exponential backoff.
/// </summary>
public sealed class RetryHandler : DelegatingHandler
{
    private readonly int _maxRetries;
    private readonly TimeSpan _baseDelay;
    private readonly ILogger? _logger;

    private static readonly HashSet<HttpStatusCode> RetryableStatusCodes = new()
    {
        HttpStatusCode.RequestTimeout,
        HttpStatusCode.TooManyRequests,
        HttpStatusCode.InternalServerError,
        HttpStatusCode.BadGateway,
        HttpStatusCode.ServiceUnavailable,
        HttpStatusCode.GatewayTimeout
    };

    public RetryHandler(int maxRetries = 3, TimeSpan? baseDelay = null, ILogger? logger = null)
        : base(new HttpClientHandler())
    {
        _maxRetries = maxRetries;
        _baseDelay = baseDelay ?? TimeSpan.FromMilliseconds(500);
        _logger = logger;
    }

    public RetryHandler(HttpMessageHandler innerHandler, int maxRetries = 3, TimeSpan? baseDelay = null, ILogger? logger = null)
        : base(innerHandler)
    {
        _maxRetries = maxRetries;
        _baseDelay = baseDelay ?? TimeSpan.FromMilliseconds(500);
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var attempt = 0;
        HttpResponseMessage? response = null;

        while (true)
        {
            attempt++;

            try
            {
                response = await base.SendAsync(request, cancellationToken);

                if (response.IsSuccessStatusCode || !ShouldRetry(response.StatusCode) || attempt > _maxRetries)
                {
                    return response;
                }

                _logger?.LogWarning(
                    "Request to {Url} failed with status {StatusCode}, attempt {Attempt} of {MaxRetries}",
                    request.RequestUri,
                    response.StatusCode,
                    attempt,
                    _maxRetries);
            }
            catch (HttpRequestException ex) when (attempt <= _maxRetries)
            {
                _logger?.LogWarning(
                    ex,
                    "Request to {Url} failed with exception, attempt {Attempt} of {MaxRetries}",
                    request.RequestUri,
                    attempt,
                    _maxRetries);
            }
            catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested && attempt <= _maxRetries)
            {
                _logger?.LogWarning(
                    ex,
                    "Request to {Url} timed out, attempt {Attempt} of {MaxRetries}",
                    request.RequestUri,
                    attempt,
                    _maxRetries);
            }

            if (attempt > _maxRetries)
            {
                throw new CheqiApiException(
                    $"Request failed after {_maxRetries} retries",
                    (int)(response?.StatusCode ?? HttpStatusCode.InternalServerError),
                    "MAX_RETRIES_EXCEEDED",
                    null);
            }

            var delay = CalculateDelay(attempt, response);
            _logger?.LogDebug("Waiting {Delay}ms before retry", delay.TotalMilliseconds);
            await Task.Delay(delay, cancellationToken);

            // Clone the request for retry (original request cannot be reused)
            request = await CloneRequestAsync(request);
        }
    }

    private bool ShouldRetry(HttpStatusCode statusCode)
    {
        return RetryableStatusCodes.Contains(statusCode);
    }

    private TimeSpan CalculateDelay(int attempt, HttpResponseMessage? response)
    {
        // Check for Retry-After header
        if (response?.Headers.RetryAfter?.Delta != null)
        {
            return response.Headers.RetryAfter.Delta.Value;
        }

        if (response?.Headers.RetryAfter?.Date != null)
        {
            var delay = response.Headers.RetryAfter.Date.Value - DateTimeOffset.UtcNow;
            if (delay > TimeSpan.Zero)
            {
                return delay;
            }
        }

        // Exponential backoff with jitter
        var exponentialDelay = _baseDelay * Math.Pow(2, attempt - 1);
        var jitter = Random.Shared.NextDouble() * 0.2; // Up to 20% jitter
        return exponentialDelay + TimeSpan.FromMilliseconds(exponentialDelay.TotalMilliseconds * jitter);
    }

    private static async Task<HttpRequestMessage> CloneRequestAsync(HttpRequestMessage request)
    {
        var clone = new HttpRequestMessage(request.Method, request.RequestUri);

        // Copy headers
        foreach (var header in request.Headers)
        {
            clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }

        // Copy content if present
        if (request.Content != null)
        {
            var content = await request.Content.ReadAsByteArrayAsync();
            clone.Content = new ByteArrayContent(content);

            foreach (var header in request.Content.Headers)
            {
                clone.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }
        }

        // Copy properties
        foreach (var prop in request.Options)
        {
            clone.Options.TryAdd(prop.Key, prop.Value);
        }

        return clone;
    }
}
