using Cheqi.Sdk.Config;
using Cheqi.Sdk.Encryption;
using Cheqi.Sdk.Http;
using Cheqi.Sdk.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Cheqi.Sdk;

/// <summary>
/// Main entry point for the Cheqi SDK.
/// Provides access to all SDK services for processing encrypted receipts and credit notes.
/// </summary>
public sealed class CheqiSdk : IDisposable
{
    private readonly CheqiSdkConfig _config;
    private readonly ILoggerFactory _loggerFactory;
    private readonly ICheqiApiClient _apiClient;
    private bool _disposed;

    /// <summary>
    /// The encryption service for encrypting receipts and credit notes.
    /// </summary>
    public EncryptionService EncryptionService { get; }

    /// <summary>
    /// The decryption service for decrypting receipts and credit notes.
    /// </summary>
    public DecryptionService DecryptionService { get; }

    /// <summary>
    /// The matching service for customer identification.
    /// </summary>
    public MatchingService MatchingService { get; }

    /// <summary>
    /// The receipt service for processing receipts.
    /// </summary>
    public ReceiptService ReceiptService { get; }

    /// <summary>
    /// The credit note service for processing credit notes.
    /// </summary>
    public CreditNoteService CreditNoteService { get; }

    /// <summary>
    /// The company service for provisioning companies.
    /// </summary>
    public CompanyService CompanyService { get; }

    /// <summary>
    /// The store service for managing stores.
    /// </summary>
    public StoreService StoreService { get; }

    /// <summary>
    /// Creates a new SDK builder.
    /// </summary>
    /// <returns>A new CheqiSdkBuilder instance.</returns>
    public static CheqiSdkBuilder Builder() => new();

    /// <summary>
    /// Creates a new CheqiSdk instance with the specified configuration.
    /// </summary>
    /// <param name="config">The SDK configuration.</param>
    /// <param name="loggerFactory">Optional logger factory.</param>
    /// <param name="httpClient">Optional custom HTTP client.</param>
    internal CheqiSdk(CheqiSdkConfig config, ILoggerFactory? loggerFactory = null, HttpClient? httpClient = null)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _loggerFactory = loggerFactory ?? NullLoggerFactory.Instance;

        // Create API client
        var apiClientLogger = _loggerFactory.CreateLogger<CheqiApiClient>();
        _apiClient = new CheqiApiClient(_config, apiClientLogger, httpClient);

        // Create encryption services
        EncryptionService = new EncryptionService(_config.EncryptionConfig);
        DecryptionService = new DecryptionService(_config.PrivateKey, _config.EncryptionConfig);

        // Create business services
        MatchingService = new MatchingService(_apiClient, _loggerFactory.CreateLogger<MatchingService>());

        ReceiptService = new ReceiptService(
            _apiClient,
            EncryptionService,
            MatchingService,
            _loggerFactory.CreateLogger<ReceiptService>());

        CreditNoteService = new CreditNoteService(
            _apiClient,
            EncryptionService,
            MatchingService,
            _loggerFactory.CreateLogger<CreditNoteService>());

        CompanyService = new CompanyService(_apiClient, _loggerFactory.CreateLogger<CompanyService>());
        StoreService = new StoreService(_apiClient, _loggerFactory.CreateLogger<StoreService>());
    }

    /// <summary>
    /// Gets an OAuth access token using the configured client credentials.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The access token.</returns>
    public Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        return _apiClient.GetAccessTokenAsync(cancellationToken);
    }

    /// <summary>
    /// Disposes the SDK and releases all resources.
    /// </summary>
    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _apiClient.Dispose();
    }
}
