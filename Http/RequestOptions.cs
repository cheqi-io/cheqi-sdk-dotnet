namespace Cheqi.Sdk.Http;

/// <summary>
/// Per-call settings that override the SDK-level configuration for a single API call.
/// </summary>
/// <remarks>
/// Pass an OAuth access token here when acting on behalf of a connected company. The SDK never
/// stores or refreshes tokens: obtain them with <see cref="OAuth.OAuthService.RequestAccessTokenAsync"/>,
/// cache them per Grant in your own store, and pass the current one on each call.
/// Without an access token the call authenticates with the configured API key.
/// <code>
/// var options = RequestOptions.WithAccessToken(await tokenStore.GetTokenAsync(grantId));
/// await sdk.ReceiptService.IssueReceiptAsync(identificationDetails, receiptPayload, options: options);
/// </code>
/// </remarks>
public sealed class RequestOptions : IEquatable<RequestOptions>
{
    private readonly string? _accessToken;
    private readonly TimeSpan? _timeout;
    private readonly int? _maxRetries;

    /// <summary>Options that use the SDK-level configuration for everything.</summary>
    public static RequestOptions Default { get; } = new();

    /// <summary>Options that authenticate this call with the given OAuth access token.</summary>
    public static RequestOptions WithAccessToken(string accessToken) => new() { AccessToken = accessToken };

    /// <summary>OAuth access token for this call, or <c>null</c> to use the configured API key.</summary>
    public string? AccessToken
    {
        get => _accessToken;
        init
        {
            if (value != null && string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("AccessToken cannot be empty", nameof(AccessToken));
            _accessToken = value;
        }
    }

    /// <summary>Timeout for this call, or <c>null</c> to use the configured timeout.</summary>
    public TimeSpan? Timeout
    {
        get => _timeout;
        init
        {
            if (value is { } timeout && timeout <= TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(Timeout), "Timeout must be positive");
            _timeout = value;
        }
    }

    /// <summary>
    /// Maximum retries for this call, or <c>null</c> to use the configured value. Only applies to
    /// operations the SDK retries; encrypted submissions are never retried automatically.
    /// </summary>
    public int? MaxRetries
    {
        get => _maxRetries;
        init
        {
            if (value < 0) throw new ArgumentOutOfRangeException(nameof(MaxRetries), "MaxRetries cannot be negative");
            _maxRetries = value;
        }
    }

    public bool Equals(RequestOptions? other) => other is not null
        && _accessToken == other._accessToken && _timeout == other._timeout && _maxRetries == other._maxRetries;

    public override bool Equals(object? obj) => Equals(obj as RequestOptions);

    public override int GetHashCode() => HashCode.Combine(_accessToken, _timeout, _maxRetries);

    // Never print the token itself.
    public override string ToString() =>
        $"RequestOptions {{ AccessToken = {(_accessToken == null ? "null" : "***")}, Timeout = {_timeout}, MaxRetries = {_maxRetries} }}";
}
