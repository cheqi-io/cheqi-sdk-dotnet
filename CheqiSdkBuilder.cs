using System.Security.Cryptography;
using Cheqi.Sdk.Config;
using Microsoft.Extensions.Logging;

namespace Cheqi.Sdk;

/// <summary>
/// Builder for creating CheqiSdk instances with fluent configuration.
/// </summary>
public sealed class CheqiSdkBuilder
{
    private readonly CheqiSdkConfig.Builder _configBuilder = CheqiSdkConfig.CreateBuilder();
    private ILoggerFactory? _loggerFactory;
    private HttpClient? _httpClient;

    /// <summary>
    /// Sets the API endpoint using a predefined environment.
    /// </summary>
    /// <param name="environment">The target environment.</param>
    /// <returns>This builder for chaining.</returns>
    public CheqiSdkBuilder ApiEndpoint(Config.Environment environment)
    {
        _configBuilder.WithEnvironment(environment);
        return this;
    }

    /// <summary>
    /// Sets a custom API endpoint URL.
    /// </summary>
    /// <param name="apiEndpoint">The API endpoint URL.</param>
    /// <returns>This builder for chaining.</returns>
    public CheqiSdkBuilder ApiEndpoint(string apiEndpoint)
    {
        _configBuilder.WithApiEndpoint(apiEndpoint);
        return this;
    }

    /// <summary>
    /// Sets the API key for authentication.
    /// </summary>
    /// <param name="apiKey">The API key.</param>
    /// <returns>This builder for chaining.</returns>
    public CheqiSdkBuilder ApiKey(string apiKey)
    {
        _configBuilder.WithApiKey(apiKey);
        return this;
    }

    /// <summary>
    /// Sets the OAuth client credentials for authentication.
    /// </summary>
    /// <param name="clientId">The client ID.</param>
    /// <param name="clientSecret">The client secret.</param>
    /// <returns>This builder for chaining.</returns>
    public CheqiSdkBuilder SupplierCredentials(string clientId, string clientSecret)
    {
        _configBuilder.WithClientCredentials(clientId, clientSecret);
        return this;
    }

    /// <summary>
    /// Sets the private key for decryption from PEM format.
    /// </summary>
    /// <param name="privateKeyPem">The private key in PEM format.</param>
    /// <returns>This builder for chaining.</returns>
    public CheqiSdkBuilder PrivateKey(string privateKeyPem)
    {
        _configBuilder.WithPrivateKey(privateKeyPem);
        return this;
    }

    /// <summary>
    /// Sets the private key for decryption.
    /// </summary>
    /// <param name="privateKey">The RSA private key.</param>
    /// <returns>This builder for chaining.</returns>
    public CheqiSdkBuilder PrivateKey(RSA privateKey)
    {
        _configBuilder.WithPrivateKey(privateKey);
        return this;
    }

    /// <summary>
    /// Sets the HTTP request timeout.
    /// </summary>
    /// <param name="timeout">The timeout duration.</param>
    /// <returns>This builder for chaining.</returns>
    public CheqiSdkBuilder Timeout(TimeSpan timeout)
    {
        _configBuilder.WithTimeout(timeout);
        return this;
    }

    /// <summary>
    /// Sets the maximum number of retry attempts.
    /// </summary>
    /// <param name="maxRetries">The maximum number of retries.</param>
    /// <returns>This builder for chaining.</returns>
    public CheqiSdkBuilder MaxRetries(int maxRetries)
    {
        _configBuilder.WithMaxRetries(maxRetries);
        return this;
    }

    /// <summary>
    /// Sets the encryption configuration.
    /// </summary>
    /// <param name="config">The encryption configuration.</param>
    /// <returns>This builder for chaining.</returns>
    public CheqiSdkBuilder EncryptionConfig(EncryptionConfig config)
    {
        _configBuilder.WithEncryptionConfig(config);
        return this;
    }

    /// <summary>
    /// Sets the logger factory for SDK logging.
    /// </summary>
    /// <param name="loggerFactory">The logger factory.</param>
    /// <returns>This builder for chaining.</returns>
    public CheqiSdkBuilder LoggerFactory(ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory;
        return this;
    }

    /// <summary>
    /// Sets a custom HTTP client to use for API requests.
    /// </summary>
    /// <param name="httpClient">The HTTP client.</param>
    /// <returns>This builder for chaining.</returns>
    public CheqiSdkBuilder HttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
        return this;
    }

    /// <summary>
    /// Builds the CheqiSdk instance.
    /// </summary>
    /// <returns>The configured CheqiSdk instance.</returns>
    public CheqiSdk Build()
    {
        var config = _configBuilder.Build();

        if (!config.HasValidAuthentication)
        {
            throw new InvalidOperationException(
                "Authentication must be configured. Use ApiKey() or SupplierCredentials().");
        }

        return new CheqiSdk(config, _loggerFactory, _httpClient);
    }
}
