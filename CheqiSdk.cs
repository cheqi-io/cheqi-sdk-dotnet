using Cheqi.Sdk.Company;
using Cheqi.Sdk.Config;
using Cheqi.Sdk.CreditNote;
using Cheqi.Sdk.Decryption;
using Cheqi.Sdk.Download;
using Cheqi.Sdk.Encryption;
using Cheqi.Sdk.Http;
using Cheqi.Sdk.Matching;
using Cheqi.Sdk.Receipt;
using Cheqi.Sdk.Verification;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Cheqi.Sdk;

/// <summary>Entry point for encrypted receipt issuance and recovery. Reuse one instance per application.</summary>
public sealed class CheqiSdk : IDisposable
{
    private bool _disposed;
    public CheqiSdkConfig Config { get; }
    public ICheqiApiClient ApiClient { get; }
    public EncryptionService EncryptionService { get; }
    public DecryptionService DecryptionService { get; }
    public DownloadService DownloadService { get; }
    public MatchingService MatchingService { get; }
    public ReceiptService ReceiptService { get; }
    public CreditNoteService CreditNoteService { get; }
    public StoreService StoreService { get; }
    public CompanyService CompanyService { get; }
    public VerificationService VerificationService { get; }

    public CheqiSdk(CheqiSdkOptions options, ILoggerFactory? loggerFactory = null, HttpClient? httpClient = null)
        : this(CheqiSdkConfig.FromOptions(options), loggerFactory ?? options.LoggerFactory, httpClient ?? options.HttpClient) { }

    internal CheqiSdk(CheqiSdkConfig config, ILoggerFactory? loggerFactory = null, HttpClient? httpClient = null)
    {
        Config = config ?? throw new ArgumentNullException(nameof(config));
        var logging = loggerFactory ?? NullLoggerFactory.Instance;
        ApiClient = new CheqiApiClient(config, logging.CreateLogger<CheqiApiClient>(), httpClient);
        EncryptionService = new EncryptionService(logging.CreateLogger<EncryptionService>());
        DecryptionService = new DecryptionService(logging.CreateLogger<DecryptionService>());
        DownloadService = new DownloadService();
        MatchingService = new MatchingService(ApiClient, logging.CreateLogger<MatchingService>());
        ReceiptService = new ReceiptService(ApiClient, EncryptionService, MatchingService, DownloadService, config.ReceiptDownloadBaseUrl);
        CreditNoteService = new CreditNoteService(ApiClient, EncryptionService, MatchingService);
        StoreService = new StoreService(ApiClient, logging.CreateLogger<StoreService>());
        CompanyService = new CompanyService(ApiClient);
        VerificationService = new VerificationService();
    }

    public static CheqiSdk Create(CheqiSdkOptions options) => new(options);
    public static CheqiSdkBuilder Builder() => new();

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        ApiClient.Dispose();
        EncryptionService.Dispose();
    }
}
