using System.Net.Http;
using Microsoft.Extensions.Logging;

namespace Cheqi.Sdk.Config;

/// <summary>
/// Idiomatic .NET options for constructing <see cref="CheqiSdk"/>.
/// </summary>
public sealed class CheqiSdkOptions
{
    public CheqiApiEnvironment? Environment { get; init; }

    public string? CustomApiEndpoint { get; init; }

    public string? ApiKey { get; init; }

    public string? SupplierClientId { get; init; }

    public string? SupplierClientSecret { get; init; }

    public string? PrivateKey { get; init; }

    public Encryption.EncryptionConfig? EncryptionConfig { get; init; }

    public int TimeoutSeconds { get; init; } = 30;

    public int MaxRetries { get; init; } = 3;

    public ILoggerFactory? LoggerFactory { get; init; }

    public HttpClient? HttpClient { get; init; }
}
