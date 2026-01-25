using Cheqi.Sdk.Decryption;
using Cheqi.Sdk.Exceptions;
using Cheqi.Sdk.Models;
using Cheqi.Sdk.Models.Ubl;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Text.Json;

namespace Cheqi.Sdk.Receipt;

/// <summary>
/// Processes encrypted receipts by decrypting and parsing them into PurchaseReceipt objects.
/// 
/// This processor handles:
/// - Decryption of encrypted receipt content
/// - Parsing JSON into UBL PurchaseReceipt objects
/// - Injecting customer details when available
/// </summary>
public class ReceiptProcessor
{
    private readonly ILogger<ReceiptProcessor> _logger;
    private readonly DecryptionService _decryptionService;
    private readonly JsonSerializerOptions _jsonOptions;

    /// <summary>
    /// Creates a new ReceiptProcessor.
    /// </summary>
    /// <param name="decryptionService">Service for decrypting receipts</param>
    /// <param name="logger">Optional logger</param>
    public ReceiptProcessor(DecryptionService decryptionService, ILogger<ReceiptProcessor>? logger = null)
    {
        _decryptionService = decryptionService ?? throw new ArgumentNullException(nameof(decryptionService));
        _logger = logger ?? NullLogger<ReceiptProcessor>.Instance;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        _logger.LogInformation("ReceiptProcessor initialized");
    }

    /// <summary>
    /// Processes an encrypted receipt by decrypting and parsing it.
    /// </summary>
    /// <param name="encryptedReceipt">The encrypted receipt DTO</param>
    /// <param name="privateKey">The private key for decryption (Base64 PKCS#8 format)</param>
    /// <returns>Fully parsed PurchaseReceipt with customer details if available</returns>
    /// <exception cref="ReceiptProcessingException">if decryption or parsing fails</exception>
    public PurchaseReceipt ProcessEncryptedReceipt(
        EncryptedReceipt encryptedReceipt,
        string privateKey)
    {
        _logger.LogDebug("Processing encrypted receipt for recipient: {RecipientId}", encryptedReceipt.recipientId);

        // Step 1: Decrypt receipt and customer details
        var decrypted = _decryptionService.DecryptReceipt(encryptedReceipt, privateKey);

        // Step 2: Parse receipt JSON into PurchaseReceipt object
        PurchaseReceipt receipt;
        try
        {
            receipt = ParseReceiptFromJson(decrypted.ReceiptContentJson);
            _logger.LogDebug("Successfully parsed receipt content");
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to parse receipt JSON: {Message}", e.Message);
            throw new ReceiptProcessingException("Failed to parse receipt JSON", e);
        }

        // Step 3: Inject customer party if available
        if (decrypted.HasCustomerDetails())
        {
            try
            {
                var customerParty = ParsePartyFromJson(decrypted.CustomerDetails!);
                receipt.AccountingCustomerParty = customerParty;
                _logger.LogDebug("Successfully injected customer details");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to parse customer details JSON: {Message}", e.Message);
                throw new ReceiptProcessingException("Failed to parse customer details JSON", e);
            }
        }

        _logger.LogInformation("Successfully processed encrypted receipt for recipient: {RecipientId}", encryptedReceipt.recipientId);
        return receipt;
    }

    /// <summary>
    /// Parses PurchaseReceipt from JSON string.
    /// </summary>
    private PurchaseReceipt ParseReceiptFromJson(string receiptJson)
    {
        return JsonSerializer.Deserialize<PurchaseReceipt>(receiptJson, _jsonOptions)
            ?? throw new ReceiptProcessingException("Failed to deserialize receipt JSON");
    }

    /// <summary>
    /// Parses Party from JSON string.
    /// </summary>
    private Party ParsePartyFromJson(string partyJson)
    {
        return JsonSerializer.Deserialize<Party>(partyJson, _jsonOptions)
            ?? throw new ReceiptProcessingException("Failed to deserialize party JSON");
    }
}
