using System.Security.Cryptography;

namespace Cheqi.Sdk.Config;

/// <summary>
/// Immutable configuration for the Cheqi SDK.
/// </summary>
public sealed class CheqiSdkConfig
{
    /// <summary>
    /// The base URL for the Cheqi API.
    /// </summary>
    public string ApiEndpoint { get; }

    /// <summary>
    /// API key for direct authentication (optional if using OAuth).
    /// </summary>
    public string? ApiKey { get; }

    /// <summary>
    /// OAuth client ID for client credentials flow (optional if using API key).
    /// </summary>
    public string? ClientId { get; }

    /// <summary>
    /// OAuth client secret for client credentials flow (optional if using API key).
    /// </summary>
    public string? ClientSecret { get; }

    /// <summary>
    /// Private key for decryption operations (PEM format, optional).
    /// </summary>
    public RSA? PrivateKey { get; }

    /// <summary>
    /// HTTP request timeout.
    /// </summary>
    public TimeSpan Timeout { get; }

    /// <summary>
    /// Maximum number of retry attempts for failed requests.
    /// </summary>
    public int MaxRetries { get; }

    /// <summary>
    /// Encryption configuration.
    /// </summary>
    public EncryptionConfig EncryptionConfig { get; }

    private CheqiSdkConfig(
        string apiEndpoint,
        string? apiKey,
        string? clientId,
        string? clientSecret,
        RSA? privateKey,
        TimeSpan timeout,
        int maxRetries,
        EncryptionConfig encryptionConfig)
    {
        ApiEndpoint = apiEndpoint;
        ApiKey = apiKey;
        ClientId = clientId;
        ClientSecret = clientSecret;
        PrivateKey = privateKey;
        Timeout = timeout;
        MaxRetries = maxRetries;
        EncryptionConfig = encryptionConfig;
    }

    /// <summary>
    /// Creates a new configuration builder.
    /// </summary>
    public static Builder CreateBuilder() => new();

    /// <summary>
    /// Validates that the configuration has valid authentication credentials.
    /// </summary>
    public bool HasValidAuthentication =>
        !string.IsNullOrEmpty(ApiKey) ||
        (!string.IsNullOrEmpty(ClientId) && !string.IsNullOrEmpty(ClientSecret));

    /// <summary>
    /// Builder for creating CheqiSdkConfig instances.
    /// </summary>
    public sealed class Builder
    {
        private string _apiEndpoint = Environment.Production.GetBaseUrl();
        private string? _apiKey;
        private string? _clientId;
        private string? _clientSecret;
        private RSA? _privateKey;
        private TimeSpan _timeout = TimeSpan.FromSeconds(30);
        private int _maxRetries = 3;
        private EncryptionConfig _encryptionConfig = EncryptionConfig.Default;

        /// <summary>
        /// Sets the API endpoint using a predefined environment.
        /// </summary>
        public Builder WithEnvironment(Environment environment)
        {
            _apiEndpoint = environment.GetBaseUrl();
            return this;
        }

        /// <summary>
        /// Sets a custom API endpoint URL.
        /// </summary>
        public Builder WithApiEndpoint(string apiEndpoint)
        {
            _apiEndpoint = apiEndpoint ?? throw new ArgumentNullException(nameof(apiEndpoint));
            return this;
        }

        /// <summary>
        /// Sets the API key for authentication.
        /// </summary>
        public Builder WithApiKey(string apiKey)
        {
            _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
            return this;
        }

        /// <summary>
        /// Sets the OAuth client credentials for authentication.
        /// </summary>
        public Builder WithClientCredentials(string clientId, string clientSecret)
        {
            _clientId = clientId ?? throw new ArgumentNullException(nameof(clientId));
            _clientSecret = clientSecret ?? throw new ArgumentNullException(nameof(clientSecret));
            return this;
        }

        /// <summary>
        /// Sets the private key for decryption from PEM format.
        /// </summary>
        public Builder WithPrivateKey(string privateKeyPem)
        {
            if (string.IsNullOrEmpty(privateKeyPem))
                throw new ArgumentNullException(nameof(privateKeyPem));

            _privateKey = RSA.Create();
            _privateKey.ImportFromPem(privateKeyPem);
            return this;
        }

        /// <summary>
        /// Sets the private key for decryption.
        /// </summary>
        public Builder WithPrivateKey(RSA privateKey)
        {
            _privateKey = privateKey ?? throw new ArgumentNullException(nameof(privateKey));
            return this;
        }

        /// <summary>
        /// Sets the HTTP request timeout.
        /// </summary>
        public Builder WithTimeout(TimeSpan timeout)
        {
            if (timeout <= TimeSpan.Zero)
                throw new ArgumentException("Timeout must be positive", nameof(timeout));
            _timeout = timeout;
            return this;
        }

        /// <summary>
        /// Sets the maximum number of retry attempts.
        /// </summary>
        public Builder WithMaxRetries(int maxRetries)
        {
            if (maxRetries < 0)
                throw new ArgumentException("Max retries cannot be negative", nameof(maxRetries));
            _maxRetries = maxRetries;
            return this;
        }

        /// <summary>
        /// Sets the encryption configuration.
        /// </summary>
        public Builder WithEncryptionConfig(EncryptionConfig config)
        {
            _encryptionConfig = config ?? throw new ArgumentNullException(nameof(config));
            return this;
        }

        /// <summary>
        /// Builds the configuration instance.
        /// </summary>
        public CheqiSdkConfig Build()
        {
            if (string.IsNullOrEmpty(_apiEndpoint))
                throw new InvalidOperationException("API endpoint must be configured");

            return new CheqiSdkConfig(
                _apiEndpoint,
                _apiKey,
                _clientId,
                _clientSecret,
                _privateKey,
                _timeout,
                _maxRetries,
                _encryptionConfig);
        }
    }
}
