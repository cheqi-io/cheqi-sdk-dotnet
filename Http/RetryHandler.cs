using System.Net;
using Microsoft.Extensions.Logging;

namespace Cheqi.Sdk.Http;

/// <summary>Retries transient failures; encrypted submissions use explicit match recovery.</summary>
public sealed class RetryHandler : DelegatingHandler
{
    public static readonly HttpRequestOptionsKey<bool> DisableRetries = new("Cheqi.DisableRetries");
    public static readonly HttpRequestOptionsKey<int> MaxRetriesOverride = new("Cheqi.MaxRetries");
    private readonly int _maxRetries;
    private readonly TimeSpan _baseDelay;
    private readonly ILogger? _logger;
    public RetryHandler(int maxRetries = 3, TimeSpan? baseDelay = null, ILogger? logger = null)
        : this(new HttpClientHandler(), maxRetries, baseDelay, logger) { }
    public RetryHandler(HttpMessageHandler innerHandler, int maxRetries = 3, TimeSpan? baseDelay = null, ILogger? logger = null)
        : base(innerHandler)
    {
        if (maxRetries < 0) throw new ArgumentOutOfRangeException(nameof(maxRetries));
        _maxRetries = maxRetries;
        _baseDelay = baseDelay ?? TimeSpan.FromSeconds(1);
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (request.Options.TryGetValue(DisableRetries, out var disabled) && disabled)
            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        // Buffer before the initial send, so every retry has identical content and headers.
        var content = request.Content == null ? null : await request.Content.ReadAsByteArrayAsync(cancellationToken).ConfigureAwait(false);
        var maxRetries = request.Options.TryGetValue(MaxRetriesOverride, out var overridden) ? overridden : _maxRetries;
        for (var attempt = 0; ; attempt++)
        {
            using var retry = attempt == 0 ? null : CloneRequest(request, content);
            HttpResponseMessage? response = null;
            try
            {
                response = await base.SendAsync(retry ?? request, cancellationToken).ConfigureAwait(false);
                if (attempt >= maxRetries || !IsRetryable(response.StatusCode)) return response;
            }
            catch (HttpRequestException) when (attempt < maxRetries) { }
            catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested && attempt < maxRetries) { }
            var delay = Delay(attempt, response);
            response?.Dispose();
            _logger?.LogDebug("Retry attempt {Attempt} in {Delay}ms", attempt + 1, delay.TotalMilliseconds);
            await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
        }
    }
    private static bool IsRetryable(HttpStatusCode code) => code is HttpStatusCode.RequestTimeout or HttpStatusCode.TooManyRequests || (int)code is >= 500 and < 600;
    private TimeSpan Delay(int attempt, HttpResponseMessage? response)
    {
        if (response?.Headers.RetryAfter?.Delta is TimeSpan delta) return delta > TimeSpan.Zero ? delta : TimeSpan.Zero;
        if (response?.Headers.RetryAfter?.Date is DateTimeOffset date) return date > DateTimeOffset.UtcNow ? date - DateTimeOffset.UtcNow : TimeSpan.Zero;
        var baseMs = response?.StatusCode == HttpStatusCode.TooManyRequests ? _baseDelay.TotalMilliseconds * 5 : _baseDelay.TotalMilliseconds;
        return TimeSpan.FromMilliseconds(Math.Min(30000, baseMs * Math.Pow(2, attempt) * (0.75 + Random.Shared.NextDouble() * 0.5)));
    }
    private static HttpRequestMessage CloneRequest(HttpRequestMessage source, byte[]? content)
    {
        var clone = new HttpRequestMessage(source.Method, source.RequestUri) { Version = source.Version, VersionPolicy = source.VersionPolicy };
        foreach (var header in source.Headers) clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
        foreach (var option in source.Options) clone.Options.Set(new HttpRequestOptionsKey<object?>(option.Key), option.Value);
        if (content != null)
        {
            clone.Content = new ByteArrayContent(content);
            foreach (var header in source.Content!.Headers) clone.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }
        return clone;
    }
}
