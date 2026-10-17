using Cheqi.Sdk.Config;
using Microsoft.Extensions.Logging;

namespace Cheqi.Sdk;

/// <summary>
/// Builder class for CheqiSDK configuration.
/// </summary>
public sealed class CheqiSdkBuilder
{
    private readonly CheqiSdkConfig.Builder _configBuilder = CheqiSdkConfig.BuilderInstance();
    private ILoggerFactory? _loggerFactory;

    /// <summary>
    /// Sets the API endpoint using a predefined environment.
    /// </summary>
    /// <param name="apiEndpoint">the predefined environment (SANDBOX, TEST, PRODUCTION)</param>
    /// <returns>this builder instance</returns>
    public CheqiSdkBuilder ApiEndpoint(Config.Environment apiEndpoint)
    {
        _configBuilder.ApiEndpoint(apiEndpoint);
        return this;
    }

    /// <summary>
    /// Sets a custom API endpoint URL.
    /// Use this method when you need to connect to a custom or self-hosted Cheqi API instance.
    /// </summary>
    /// <param name="customUrl">the custom API base URL (e.g., "https://custom.api.example.com")</param>
    /// <returns>this builder instance</returns>
    public CheqiSdkBuilder CustomApiEndpoint(string customUrl)
    {
        _configBuilder.CustomApiEndpoint(customUrl);
        return this;
    }

    public CheqiSdkBuilder ReceiptDownloadBaseUrl(string baseUrl)
    {
        _configBuilder.ReceiptDownloadBaseUrl(baseUrl);
        return this;
    }

    public CheqiSdkBuilder ApiKey(string apiKey)
    {
        _configBuilder.ApiKey(apiKey);
        return this;
    }

    public CheqiSdkBuilder PrivateKey(string privateKey)
    {
        _configBuilder.PrivateKey(privateKey);
        return this;
    }

    public CheqiSdkBuilder TimeoutSeconds(int timeoutSeconds)
    {
        _configBuilder.TimeoutSeconds(timeoutSeconds);
        return this;
    }

    public CheqiSdkBuilder MaxRetries(int maxRetries)
    {
        _configBuilder.MaxRetries(maxRetries);
        return this;
    }

    /// <summary>
    /// Sets the logger factory for SDK logging.
    /// </summary>
    /// <param name="loggerFactory">The logger factory instance</param>
    /// <returns>this builder instance</returns>
    public CheqiSdkBuilder LoggerFactory(ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory;
        return this;
    }

    public CheqiSdk Build()
    {
        return new CheqiSdk(_configBuilder.Build(), _loggerFactory);
    }
}
