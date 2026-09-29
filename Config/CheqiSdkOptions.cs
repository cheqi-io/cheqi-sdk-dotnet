using System.Net.Http;
using Microsoft.Extensions.Logging;

namespace Cheqi.Sdk.Config;

/// <summary>
/// Idiomatic .NET options for constructing <see cref="CheqiSdk"/>.
/// </summary>
public sealed class CheqiSdkOptions
{
    public CheqiApiEnvironment? Environment { get; set; }

    public string? CustomApiEndpoint { get; set; }

    public string? ReceiptDownloadBaseUrl { get; set; }

    public string? ApiKey { get; set; }

    public string? SupplierClientId { get; set; }

    public string? SupplierClientSecret { get; set; }

    public string? PrivateKey { get; set; }

    public Encryption.EncryptionConfig? EncryptionConfig { get; set; }

    public int TimeoutSeconds { get; set; } = 30;

    public int MaxRetries { get; set; } = 3;

    public ILoggerFactory? LoggerFactory { get; set; }

    public HttpClient? HttpClient { get; set; }
}
