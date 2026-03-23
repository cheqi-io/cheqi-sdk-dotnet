using Cheqi.Sdk.Encryption;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Cheqi.Sdk.Config;

/// <summary>
/// Main configuration class for the Cheqi SDK.
///
/// Contains all configuration needed for SDK operation including:
/// - API endpoints and credentials
/// - Encryption configuration
/// - Timeout and retry settings
///
/// Thread-safe and immutable after construction.
/// </summary>
public sealed class CheqiSdkConfig
{
    public string ApiEndpoint { get; }
    public string? ApiKey { get; }
    public string? SupplierClientId { get; }
    public string? SupplierClientSecret { get; }
    public string? PrivateKeyBase64 { get; }
    public Encryption.EncryptionConfig? EncryptionConfig { get; }
    public int TimeoutSeconds { get; }
    public int MaxRetries { get; }
    
    // Convenience properties for API client
    public string? ClientId => SupplierClientId;
    public string? ClientSecret => SupplierClientSecret;
    public TimeSpan Timeout => TimeSpan.FromSeconds(TimeoutSeconds);

    private CheqiSdkConfig(Builder builder)
    {
        ApiEndpoint = builder._apiEndpoint!;
        ApiKey = builder._apiKey;
        SupplierClientId = builder._supplierClientId;
        SupplierClientSecret = builder._supplierClientSecret;
        PrivateKeyBase64 = builder._privateKeyBase64;
        EncryptionConfig = builder._encryptionConfig;
        TimeoutSeconds = builder._timeoutSeconds;
        MaxRetries = builder._maxRetries;
    }

    public static Builder BuilderInstance() => new Builder();

    public static CheqiSdkConfig FromOptions(CheqiSdkOptions options, ILogger? logger = null)
    {
        ArgumentNullException.ThrowIfNull(options);

        var builder = new Builder(logger);

        if (options.Environment.HasValue)
        {
            builder.CustomApiEndpoint(options.Environment.Value.GetBaseUrl());
        }

        if (!string.IsNullOrWhiteSpace(options.CustomApiEndpoint))
        {
            builder.CustomApiEndpoint(options.CustomApiEndpoint);
        }

        if (!string.IsNullOrWhiteSpace(options.ApiKey))
        {
            builder.ApiKey(options.ApiKey);
        }

        if (!string.IsNullOrWhiteSpace(options.SupplierClientId) ||
            !string.IsNullOrWhiteSpace(options.SupplierClientSecret))
        {
            builder.SupplierCredentials(options.SupplierClientId ?? string.Empty, options.SupplierClientSecret ?? string.Empty);
        }

        if (!string.IsNullOrWhiteSpace(options.PrivateKey))
        {
            builder.PrivateKey(options.PrivateKey);
        }

        if (options.EncryptionConfig != null)
        {
            builder.EncryptionConfig(options.EncryptionConfig);
        }

        builder.TimeoutSeconds(options.TimeoutSeconds);
        builder.MaxRetries(options.MaxRetries);

        return builder.Build();
    }

    public sealed class Builder
    {
        internal string? _apiEndpoint;
        internal string? _apiKey;
        internal string? _supplierClientId;
        internal string? _supplierClientSecret;
        internal string? _privateKeyBase64;
        internal Encryption.EncryptionConfig? _encryptionConfig;
        internal int _timeoutSeconds = 30;
        internal int _maxRetries = 3;
        private readonly ILogger? _logger;

        public Builder(ILogger? logger = null)
        {
            _logger = logger;
        }

        /// <summary>
        /// Sets the API endpoint using a predefined environment.
        /// </summary>
        public Builder ApiEndpoint(Environment apiEndpoint)
        {
            _apiEndpoint = apiEndpoint.GetBaseUrl();
            return this;
        }

        /// <summary>
        /// Sets a custom API endpoint URL.
        /// </summary>
        public Builder CustomApiEndpoint(string customUrl)
        {
            _apiEndpoint = customUrl;
            return this;
        }

        /// <summary>
        /// Sets the API key for direct company access.
        /// Use this for companies accessing their own data.
        /// </summary>
        public Builder ApiKey(string apiKey)
        {
            _apiKey = apiKey;
            return this;
        }

        /// <summary>
        /// Sets client credentials for OAuth token generation.
        /// Only needed for client applications using OAuth flow.
        /// </summary>
        public Builder SupplierCredentials(string clientId, string clientSecret)
        {
            _supplierClientId = clientId;
            _supplierClientSecret = clientSecret;
            return this;
        }

        public Builder EncryptionConfig(Encryption.EncryptionConfig encryptionConfig)
        {
            _encryptionConfig = encryptionConfig;
            return this;
        }

        public Builder TimeoutSeconds(int timeoutSeconds)
        {
            _timeoutSeconds = timeoutSeconds;
            return this;
        }

        public Builder MaxRetries(int maxRetries)
        {
            _maxRetries = maxRetries;
            return this;
        }

        /// <summary>
        /// Sets the issuer's private key for decrypting credit note requests.
        /// </summary>
        public Builder PrivateKey(string privateKeyBase64)
        {
            _privateKeyBase64 = privateKeyBase64;
            return this;
        }

        public CheqiSdkConfig Build()
        {
            var logger = _logger ?? NullLogger.Instance;

            // Validate API endpoint
            if (string.IsNullOrWhiteSpace(_apiEndpoint))
            {
                throw new InvalidOperationException("API endpoint must be set using either ApiEndpoint(Environment) or CustomApiEndpoint(String)");
            }

            // Optional authentication validation - log warning if no auth configured
            bool hasApiKey = !string.IsNullOrWhiteSpace(_apiKey);
            bool hasClientCredentials = !string.IsNullOrWhiteSpace(_supplierClientId) &&
                                       !string.IsNullOrWhiteSpace(_supplierClientSecret);

            if (!hasApiKey && !hasClientCredentials)
            {
                logger.LogWarning("No authentication configured. API calls will fail unless access tokens are provided explicitly.");
            }

            // Validate timeout
            if (_timeoutSeconds <= 0)
            {
                throw new ArgumentException($"Timeout must be positive, got: {_timeoutSeconds}");
            }
            if (_timeoutSeconds > 300)
            {
                logger.LogWarning("Timeout is very high ({TimeoutSeconds}s). Consider using a lower value for better UX", _timeoutSeconds);
            }

            // Validate retries
            if (_maxRetries < 0)
            {
                throw new ArgumentException($"Max retries cannot be negative, got: {_maxRetries}");
            }
            if (_maxRetries > 10)
            {
                logger.LogWarning("Max retries is very high ({MaxRetries}). Consider using a lower value", _maxRetries);
            }

            // Use default encryption config if not provided
            if (_encryptionConfig == null)
            {
                logger.LogDebug("No encryption config provided, using default");
                _encryptionConfig = Encryption.EncryptionConfig.BuilderInstance().Build();
            }

            var config = new CheqiSdkConfig(this);
            string authMode = hasApiKey ? "API Key" : (hasClientCredentials ? "OAuth Client Credentials" : "None (tokens required per-call)");
            logger.LogInformation("CheqiSDKConfig created: endpoint={Endpoint}, authMode={AuthMode}, timeout={Timeout}s, maxRetries={MaxRetries}",
                _apiEndpoint, authMode, _timeoutSeconds, _maxRetries);

            return config;
        }
    }
}
