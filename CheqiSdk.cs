using Cheqi.Sdk.Company;
using Cheqi.Sdk.Config;
using Cheqi.Sdk.CreditNote;
using Cheqi.Sdk.Decryption;
using Cheqi.Sdk.Encryption;
using Cheqi.Sdk.Http;
using Cheqi.Sdk.Matching;
using Cheqi.Sdk.Models;
using Cheqi.Sdk.Models.Ubl;
using Cheqi.Sdk.Receipt;
using Cheqi.Sdk.Utils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Cheqi.Sdk;

/// <summary>
/// Main entry point for the Cheqi SDK providing end-to-end encrypted receipt processing.
///
/// This SDK enables suppliers to:
/// 1. Match customers using payment identifiers
/// 2. Generate receipt templates from transaction data
/// 3. Create encrypted receipts for matched customers
/// 4. Communicate with Cheqi backend through encrypted channels
/// 
/// Example usage with predefined environment:
/// <code>
/// CheqiSdk sdk = CheqiSdk.Builder()
///     .ApiEndpoint(Environment.PRODUCTION)
///     .SupplierCredentials("client_id", "client_secret")
///     .Build();
/// </code>
///
/// Example usage with custom URL:
/// <code>
/// CheqiSdk sdk = CheqiSdk.Builder()
///     .CustomApiEndpoint("https://custom.api.example.com")
///     .SupplierCredentials("client_id", "client_secret")
///     .Build();
/// </code>
/// </summary>
public sealed class CheqiSdk
{
    private readonly CheqiSdkConfig _config;
    private readonly ILoggerFactory _loggerFactory;
    private readonly EncryptionService _encryptionService;
    private readonly DecryptionService _decryptionService;
    private readonly ICheqiApiClient _apiClient;
    private readonly MatchingService _matchingService;
    private readonly ReceiptService _receiptService;
    private readonly ReceiptProcessor _receiptProcessor;
    private readonly CompanyService _companyService;
    private readonly StoreService _storeService;
    private readonly RFC8785Canonicalizer _canonicalizer;
    private readonly CreditNoteService _creditNoteService;

    internal CheqiSdk(CheqiSdkConfig config, ILoggerFactory? loggerFactory = null)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _loggerFactory = loggerFactory ?? NullLoggerFactory.Instance;

        // Create logger for RFC8785Canonicalizer
        var canonicalizerLogger = _loggerFactory.CreateLogger<RFC8785Canonicalizer>();
        _canonicalizer = new RFC8785Canonicalizer(canonicalizerLogger);

        // Create decryption service with logger
        var decryptionLogger = _loggerFactory.CreateLogger<DecryptionService>();
        _decryptionService = new DecryptionService(decryptionLogger);

        // Create receipt processor with logger
        var receiptProcessorLogger = _loggerFactory.CreateLogger<ReceiptProcessor>();
        _receiptProcessor = new ReceiptProcessor(_decryptionService, receiptProcessorLogger);

        // Create encryption service with logger
        var encryptionLogger = _loggerFactory.CreateLogger<EncryptionService>();
        _encryptionService = new EncryptionService(encryptionLogger);

        // Create API client with logger
        var apiClientLogger = _loggerFactory.CreateLogger<CheqiApiClient>();
        _apiClient = new CheqiApiClient(config, apiClientLogger);

        // Create matching service with logger
        var matchingLogger = _loggerFactory.CreateLogger<MatchingService>();
        _matchingService = new MatchingService(_apiClient, matchingLogger);

        // Create receipt service with logger
        var receiptLogger = _loggerFactory.CreateLogger<ReceiptService>();
        _receiptService = new ReceiptService(_apiClient, _encryptionService, _matchingService, receiptLogger);

        // Create company service with logger
        var companyLogger = _loggerFactory.CreateLogger<CompanyService>();
        _companyService = new CompanyService(_apiClient, companyLogger);

        // Create store service with logger
        var storeLogger = _loggerFactory.CreateLogger<StoreService>();
        _storeService = new StoreService(_apiClient, storeLogger);

        // Create credit note service with logger
        var creditNoteLogger = _loggerFactory.CreateLogger<CreditNoteService>();
        _creditNoteService = new CreditNoteService(_apiClient, _encryptionService, _matchingService, creditNoteLogger);
    }

    /// <summary>
    /// Creates a new SDK builder for configuration.
    /// </summary>
    public static CheqiSdkBuilder Builder()
    {
        return new CheqiSdkBuilder();
    }

    // ============================================
    // C# Properties (idiomatic C# access)
    // ============================================

    /// <summary>
    /// Gets the encryption service for advanced operations.
    /// </summary>
    public EncryptionService EncryptionService => _encryptionService;

    /// <summary>
    /// Gets the decryption service for advanced operations.
    /// </summary>
    public DecryptionService DecryptionService => _decryptionService;

    /// <summary>
    /// Gets the matching service for customer identification.
    /// </summary>
    public MatchingService MatchingService => _matchingService;

    /// <summary>
    /// Gets the receipt service for receipt template generation and encryption.
    /// </summary>
    public ReceiptService ReceiptService => _receiptService;

    /// <summary>
    /// Gets the API client for direct backend communication.
    /// </summary>
    public ICheqiApiClient ApiClient => _apiClient;

    /// <summary>
    /// Gets the RFC 8785 canonicalizer for JSON canonicalization.
    /// </summary>
    public RFC8785Canonicalizer Canonicalizer => _canonicalizer;

    /// <summary>
    /// Gets the current SDK configuration.
    /// </summary>
    public CheqiSdkConfig Config => _config;

    /// <summary>
    /// Gets the company service for company provisioning.
    /// </summary>
    public CompanyService CompanyService => _companyService;

    /// <summary>
    /// Gets the store service for store management.
    /// </summary>
    public StoreService StoreService => _storeService;

    /// <summary>
    /// Gets the credit note service for credit note processing.
    /// </summary>
    public CreditNoteService? CreditNoteService => _creditNoteService;

    // ============================================
    // Java-style Getter Methods (cross-platform consistency)
    // ============================================

    /// <summary>
    /// Gets the encryption service for advanced operations.
    /// </summary>
    /// <returns>EncryptionService instance</returns>
    public EncryptionService GetEncryptionService()
    {
        return _encryptionService;
    }

    /// <summary>
    /// Gets the decryption service for advanced operations.
    /// </summary>
    /// <returns>DecryptionService instance</returns>
    public DecryptionService GetDecryptionService()
    {
        return _decryptionService;
    }

    /// <summary>
    /// Gets the matching service for customer identification.
    /// </summary>
    /// <returns>MatchingService instance</returns>
    public MatchingService GetMatchingService()
    {
        return _matchingService;
    }

    /// <summary>
    /// Processes an encrypted receipt (synchronous - Java compatibility).
    /// </summary>
    public PurchaseReceipt ProcessEncryptedReceipt(EncryptedReceipt encryptedReceipt, string privateKey)
    {
        return _receiptProcessor.ProcessEncryptedReceipt(encryptedReceipt, privateKey);
    }

    /// <summary>
    /// Processes an encrypted receipt asynchronously (C# idiomatic).
    /// </summary>
    public Task<PurchaseReceipt> ProcessEncryptedReceiptAsync(EncryptedReceipt encryptedReceipt, string privateKey, CancellationToken cancellationToken = default)
    {
        return Task.Run(() => _receiptProcessor.ProcessEncryptedReceipt(encryptedReceipt, privateKey), cancellationToken);
    }

    /// <summary>
    /// Gets the receipt service for receipt template generation and encryption.
    /// </summary>
    /// <returns>ReceiptService instance</returns>
    public ReceiptService GetReceiptService()
    {
        return _receiptService;
    }

    /// <summary>
    /// Gets the API client for direct backend communication.
    /// </summary>
    /// <returns>CheqiApiClient instance</returns>
    public ICheqiApiClient GetApiClient()
    {
        return _apiClient;
    }

    /// <summary>
    /// Gets the RFC 8785 canonicalizer for JSON canonicalization.
    /// </summary>
    /// <returns>RFC8785Canonicalizer instance</returns>
    public RFC8785Canonicalizer GetCanonicalizer()
    {
        return _canonicalizer;
    }

    /// <summary>
    /// Gets the current SDK configuration.
    /// </summary>
    /// <returns>CheqiSdkConfig instance</returns>
    public CheqiSdkConfig GetConfig()
    {
        return _config;
    }

    /// <summary>
    /// Gets the company service for company provisioning.
    /// </summary>
    /// <returns>CompanyService instance</returns>
    public CompanyService GetCompanyService()
    {
        return _companyService;
    }

    /// <summary>
    /// Gets the store service for store management.
    /// </summary>
    /// <returns>StoreService instance</returns>
    public StoreService GetStoreService()
    {
        return _storeService;
    }

    /// <summary>
    /// Gets the credit note service for credit note processing.
    /// </summary>
    /// <returns>CreditNoteService instance</returns>
    /// <exception cref="InvalidOperationException">if no private key was configured during SDK initialization</exception>
    public CreditNoteService GetCreditNoteService()
    {
        if (_creditNoteService == null)
        {
            throw new InvalidOperationException(
                "CreditNoteService requires a private key for decryption. " +
                "Please configure the SDK with .PrivateKey(String) during initialization. " +
                "Example: CheqiSdk.Builder().PrivateKey(\"your-base64-key\").Build()");
        }
        return _creditNoteService;
    }
}
