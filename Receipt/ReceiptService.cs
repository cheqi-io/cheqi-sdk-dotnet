using Cheqi.Sdk.Encryption;
using Cheqi.Sdk.Exceptions;
using Cheqi.Sdk.Http;
using Cheqi.Sdk.Http.Exceptions;
using Cheqi.Sdk.Matching;
using Cheqi.Sdk.Models;
using Cheqi.Sdk.Models.Ubl;
using Cheqi.Sdk.Utils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Cheqi.Sdk.Receipt;

/// <summary>
/// Service for handling receipt operations in the Cheqi SDK.
/// 
/// This service provides comprehensive receipt management functionality including:
/// - Receipt template generation from transaction data
/// - Encrypted receipt creation for matched customers
/// - Integration with backend receipt processing APIs
/// 
/// The service coordinates between the encryption service and API client to provide
/// end-to-end encrypted receipt processing while maintaining data privacy.
/// </summary>
public class ReceiptService
{
    private readonly ILogger<ReceiptService> _logger;
    private readonly ICheqiApiClient _apiClient;
    private readonly EncryptionService _encryptionService;
    private readonly MatchingService _matchingService;
    private readonly RFC8785Canonicalizer _canonicalizer;
    private readonly JsonSerializerOptions _jsonOptions;

    /// <summary>
    /// Creates a new ReceiptService instance.
    /// </summary>
    /// <param name="apiClient">API client for backend communication</param>
    /// <param name="encryptionService">Service for handling encryption operations</param>
    /// <param name="matchingService">Service for customer matching</param>
    /// <param name="logger">Optional logger</param>
    public ReceiptService(
        ICheqiApiClient apiClient,
        EncryptionService encryptionService,
        MatchingService matchingService,
        ILogger<ReceiptService>? logger = null)
    {
        _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
        _encryptionService = encryptionService ?? throw new ArgumentNullException(nameof(encryptionService));
        _matchingService = matchingService ?? throw new ArgumentNullException(nameof(matchingService));
        _logger = logger ?? NullLogger<ReceiptService>.Instance;
        _canonicalizer = new RFC8785Canonicalizer();
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
            WriteIndented = false, // Compact JSON for RFC8785 canonicalization
            Converters = { new DecimalJsonConverter() }
        };
        _logger.LogInformation("ReceiptService initialized successfully");
    }

    /// <summary>
    /// Complete receipt processing workflow:
    /// 1. Matches customer using payment details
    /// 2. If customer not found AND no email provided, returns early (saves template generation)
    /// 3. Generates receipt template (only if deliverable via app or email)
    /// 4. If customer found: creates encrypted receipts and sends to backend
    /// 5. If customer not found but email provided: sends receipt via email
    /// </summary>
    /// <param name="identificationDetails">Payment information for customer matching</param>
    /// <param name="receiptRequest">Receipt template generation request</param>
    /// <param name="accessToken">Valid OAuth access token</param>
    /// <returns>ReceiptResult with success status and archival data</returns>
    public async Task<ReceiptResult> ProcessCompleteReceiptAsync(
        IdentificationDetails identificationDetails,
        ReceiptTemplateRequest receiptRequest,
        string accessToken,
        CancellationToken cancellationToken = default)
    {
        if (identificationDetails == null)
            throw new CheqiSDKException("PaymentDetails cannot be null",
                CheqiSDKException.ErrorCodes.VALIDATION_ERROR, 400, null);

        if (receiptRequest == null)
            throw new CheqiSDKException("ReceiptTemplateRequest cannot be null",
                CheqiSDKException.ErrorCodes.VALIDATION_ERROR, 400, null);

        if (string.IsNullOrWhiteSpace(accessToken))
            throw new CheqiSDKException("Access token cannot be null or empty",
                CheqiSDKException.ErrorCodes.AUTHENTICATION_FAILED, 401, null);

        try
        {
            _logger.LogDebug("Matching customer using payment details");
            var matchResponse = await _matchingService.MatchCustomerAsync(identificationDetails, accessToken, cancellationToken);

            if (!matchResponse.customerFound && string.IsNullOrEmpty(identificationDetails.recipientEmail))
            {
                _logger.LogInformation("Customer not found and no email provided - skipping template generation");
                return ReceiptResult.CustomerNotFound();
            }

            _logger.LogDebug("Generating receipt template");
            string receiptTemplate = await GenerateReceiptTemplateAsync(receiptRequest, accessToken, cancellationToken);

            // Use the original JSON from backend and inject paymentMeans to avoid serialization differences
            string receiptTemplateJson = receiptTemplate;
            if (matchResponse.paymentMeans != null && matchResponse.paymentMeans.Count > 0)
            {
                // Parse the original JSON, inject paymentMeans, and serialize back
                using var doc = JsonDocument.Parse(receiptTemplate);
                using var stream = new MemoryStream();
                using (var writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = false }))
                {
                    writer.WriteStartObject();
                    
                    // Copy all properties from original JSON
                    foreach (var property in doc.RootElement.EnumerateObject())
                    {
                        if (property.Name != "paymentMeans")
                        {
                            property.WriteTo(writer);
                        }
                    }
                    
                    // Add paymentMeans
                    writer.WritePropertyName("paymentMeans");
                    JsonSerializer.Serialize(writer, matchResponse.paymentMeans, _jsonOptions);
                    
                    writer.WriteEndObject();
                }
                receiptTemplateJson = Encoding.UTF8.GetString(stream.ToArray());
            }

            if (matchResponse.customerFound)
            {
                string canonicalReceiptJson = _canonicalizer.Canonicalize(receiptTemplateJson);
                string templateHash = HashUtils.Sha256Hex(canonicalReceiptJson);

                _logger.LogDebug("Creating encrypted receipts for {Count} recipients", matchResponse.recipients?.Count ?? 0);
                var encryptedReceipts = CreateEncryptedReceipts(
                    canonicalReceiptJson,
                    matchResponse.recipients ?? new List<Recipient>()
                );

                _logger.LogDebug("Sending {Count} encrypted receipts to backend", encryptedReceipts.Count);
                var receiptCreatedResponse = await SendEncryptedReceiptsAsync(encryptedReceipts, templateHash, matchResponse, accessToken, cancellationToken);

                _logger.LogInformation("Receipt processing completed successfully");
                return ReceiptResult.DeliveredToApp(receiptCreatedResponse, canonicalReceiptJson);
            }

            _logger.LogInformation("Customer not found, sending receipt via email");
            var receipt = JsonSerializer.Deserialize<PurchaseReceipt>(receiptTemplateJson, _jsonOptions);
            return await SendReceiptViaEmailAsync(identificationDetails.recipientEmail!, receipt!, accessToken, cancellationToken);
        }
        catch (CheqiSDKException)
        {
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unexpected error during receipt processing: {Message}", e.Message);
            throw new CheqiSDKException("Receipt processing failed: " + e.Message, e);
        }
    }

    /// <summary>
    /// Complete receipt processing workflow using API key from SDK config.
    /// Uses Bearer token authentication with the API key configured during SDK initialization.
    /// For companies accessing their own data directly.
    /// </summary>
    public async Task<ReceiptResult> ProcessCompleteReceiptAsync(
        IdentificationDetails identificationDetails,
        ReceiptTemplateRequest receiptRequest,
        CancellationToken cancellationToken = default)
    {
        if (identificationDetails == null)
            throw new CheqiSDKException("PaymentDetails cannot be null",
                CheqiSDKException.ErrorCodes.VALIDATION_ERROR, 400, null);

        if (receiptRequest == null)
            throw new CheqiSDKException("ReceiptTemplateRequest cannot be null",
                CheqiSDKException.ErrorCodes.VALIDATION_ERROR, 400, null);

        try
        {
            _logger.LogDebug("Matching customer using payment details with API key");
            var matchResponse = await _matchingService.MatchCustomerAsync(identificationDetails, cancellationToken);

            if (!matchResponse.customerFound && string.IsNullOrEmpty(identificationDetails.recipientEmail))
            {
                _logger.LogInformation("Customer not found and no email provided - skipping template generation");
                return ReceiptResult.CustomerNotFound();
            }

            _logger.LogDebug("Generating receipt template with API key");
            string receiptTemplate = await GenerateReceiptTemplateAsync(receiptRequest, cancellationToken);

            // Use the original JSON from backend and inject paymentMeans to avoid serialization differences
            string receiptTemplateJson = receiptTemplate;
            if (matchResponse.paymentMeans != null && matchResponse.paymentMeans.Count > 0)
            {
                // Parse the original JSON, inject paymentMeans, and serialize back
                using var doc = JsonDocument.Parse(receiptTemplate);
                using var stream = new MemoryStream();
                using (var writer = new Utf8JsonWriter(stream, new JsonWriterOptions { Indented = false }))
                {
                    writer.WriteStartObject();
                    
                    // Copy all properties from original JSON
                    foreach (var property in doc.RootElement.EnumerateObject())
                    {
                        if (property.Name != "paymentMeans")
                        {
                            property.WriteTo(writer);
                        }
                    }
                    
                    // Add paymentMeans
                    writer.WritePropertyName("paymentMeans");
                    JsonSerializer.Serialize(writer, matchResponse.paymentMeans, _jsonOptions);
                    
                    writer.WriteEndObject();
                }
                receiptTemplateJson = Encoding.UTF8.GetString(stream.ToArray());
            }

            if (matchResponse.customerFound)
            {
                string canonicalReceiptJson = _canonicalizer.Canonicalize(receiptTemplateJson);
                string templateHash = HashUtils.Sha256Hex(canonicalReceiptJson);

                _logger.LogDebug("Creating encrypted receipts for {Count} recipients", matchResponse.recipients?.Count ?? 0);
                var encryptedReceipts = CreateEncryptedReceipts(
                    canonicalReceiptJson,
                    matchResponse.recipients ?? new List<Recipient>());

                _logger.LogDebug("Sending {Count} encrypted receipts to backend with API key", encryptedReceipts.Count);
                var receiptCreatedResponse = await SendEncryptedReceiptsAsync(encryptedReceipts, templateHash, matchResponse, cancellationToken);

                _logger.LogInformation("Receipt processing completed successfully");
                return ReceiptResult.DeliveredToApp(receiptCreatedResponse, canonicalReceiptJson);
            }

            _logger.LogInformation("Customer not found, sending receipt via email");
            var receipt = JsonSerializer.Deserialize<PurchaseReceipt>(receiptTemplateJson, _jsonOptions);

            return await SendReceiptViaEmailAsync(identificationDetails.recipientEmail!, receipt!, cancellationToken);
        }
        catch (CheqiSDKException)
        {
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unexpected error during receipt processing: {Message}", e.Message);
            throw new CheqiSDKException("Receipt processing failed: " + e.Message, e);
        }
    }

    /// <summary>
    /// Generates a receipt template from transaction data.
    /// </summary>
    public async Task<string> GenerateReceiptTemplateAsync(
        ReceiptTemplateRequest request,
        string accessToken,
        CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new CheqiSDKException("Receipt template request cannot be null",
                CheqiSDKException.ErrorCodes.VALIDATION_ERROR, 400, null);

        if (string.IsNullOrWhiteSpace(accessToken))
            throw new CheqiSDKException("Access token is required for receipt template generation",
                CheqiSDKException.ErrorCodes.AUTHENTICATION_FAILED, 401, null);

        _logger.LogDebug("Generating receipt template for receiptId: {DocumentNumber}", request.documentNumber);

        try
        {
            string template = await _apiClient.GenerateReceiptTemplateAsync(request, accessToken, cancellationToken);
            _logger.LogDebug("Receipt template generated successfully");
            return template;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unexpected error generating receipt template: {Message}", e.Message);
            throw new CheqiSDKException("Failed to generate receipt template: " + e.Message, e);
        }
    }

    /// <summary>
    /// Generates a receipt template using API key from SDK config.
    /// </summary>
    public async Task<string> GenerateReceiptTemplateAsync(
        ReceiptTemplateRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new CheqiSDKException("Receipt template request cannot be null",
                CheqiSDKException.ErrorCodes.VALIDATION_ERROR, 400, null);

        _logger.LogDebug("Generating receipt template with API key for receiptId: {DocumentNumber}", request.documentNumber);

        try
        {
            string template = await _apiClient.GenerateReceiptTemplateAsync(request, null, cancellationToken);
            _logger.LogDebug("Receipt template generated successfully");
            return template;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unexpected error generating receipt template: {Message}", e.Message);
            throw new CheqiSDKException("Failed to generate receipt template: " + e.Message, e);
        }
    }

    /// <summary>
    /// Creates encrypted receipts for multiple customer devices.
    /// </summary>
    public HashSet<EncryptedReceiptRequestDto> CreateEncryptedReceipts(
        string purchaseReceipt,
        List<Recipient> recipients)
    {
        _logger.LogInformation("[RECEIPT] CreateEncryptedReceipts called with {Count} recipients", recipients?.Count ?? 0);
        _logger.LogDebug("[RECEIPT] Purchase receipt length: {Length} characters", purchaseReceipt?.Length ?? 0);
        
        if (string.IsNullOrWhiteSpace(purchaseReceipt))
        {
            _logger.LogError("[RECEIPT] Purchase receipt is null or empty");
            throw new CheqiSDKException("Purchase receipt cannot be null or empty",
                CheqiSDKException.ErrorCodes.VALIDATION_ERROR, 400, null);
        }

        if (recipients == null || recipients.Count == 0)
        {
            _logger.LogError("[RECEIPT] Recipients list is null or empty");
            throw new CheqiSDKException("Recipients cannot be null or empty",
                CheqiSDKException.ErrorCodes.VALIDATION_ERROR, 400, null);
        }

        // Log recipient details
        for (int i = 0; i < recipients.Count; i++)
        {
            var recipient = recipients[i];
            _logger.LogDebug("[RECEIPT] Recipient {Index}: ID={Id}, Type={Type}, HasPublicKey={HasKey}, PublicKeyLength={KeyLength}",
                i + 1, 
                recipient?.id, 
                recipient?.receiverType,
                !string.IsNullOrEmpty(recipient?.publicKey),
                recipient?.publicKey?.Length ?? 0);
        }

        try
        {
            _logger.LogInformation("[RECEIPT] Calling EncryptionService.EncryptReceiptForRecipients");
            var encryptedReceipts = _encryptionService.EncryptReceiptForRecipients(
                purchaseReceipt,
                recipients
            );
            _logger.LogInformation("[RECEIPT] Successfully created {Count} encrypted receipts", encryptedReceipts.Count);
            return encryptedReceipts;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "[RECEIPT] Failed to create encrypted receipts: {Message}. Type: {ExceptionType}, StackTrace: {StackTrace}", 
                e.Message, e.GetType().Name, e.StackTrace);
            throw new CheqiSDKException("Failed to create encrypted receipts: " + e.Message, e);
        }
    }

    /// <summary>
    /// Sends encrypted receipts using API key from SDK config.
    /// </summary>
    public async Task<ReceiptCreatedResponse> SendEncryptedReceiptsAsync(
        HashSet<EncryptedReceiptRequestDto> encryptedReceipts,
        string templateHash,
        RecipientResolutionResponse recipientResolutionResponse,
        CancellationToken cancellationToken = default)
    {
        if (encryptedReceipts == null || encryptedReceipts.Count == 0)
            throw new CheqiSDKException("Encrypted receipts cannot be null or empty",
                CheqiSDKException.ErrorCodes.VALIDATION_ERROR, 400, null);

        if (string.IsNullOrWhiteSpace(templateHash))
            throw new CheqiSDKException("Template hash cannot be null or empty",
                CheqiSDKException.ErrorCodes.VALIDATION_ERROR, 400, null);

        if (recipientResolutionResponse == null)
            throw new CheqiSDKException("Customer match response cannot be null",
                CheqiSDKException.ErrorCodes.VALIDATION_ERROR, 400, null);

        if (!recipientResolutionResponse.customerFound)
            throw new CheqiSDKException("Cannot send receipts: customer was not found during matching",
                CheqiSDKException.ErrorCodes.CUSTOMER_NOT_FOUND, 400, null);

        _logger.LogDebug("Sending {Count} encrypted receipts to backend with API key", encryptedReceipts.Count);

        try
        {
            var request = EncryptedReceiptsRequest.Builder()
                .EncryptedReceipts(encryptedReceipts)
                .MatchId(recipientResolutionResponse.matchId ?? throw new CheqiSDKException("MatchId is required", CheqiSDKException.ErrorCodes.VALIDATION_ERROR, 400, null))
                .TemplateHash(templateHash)
                .Build();

            var response = await _apiClient.SubmitEncryptedReceiptsAsync(request, null, cancellationToken);
            _logger.LogInformation("Successfully sent {Count} encrypted receipts", encryptedReceipts.Count);
            return response;
        }
        catch (CheqiApiException e)
        {
            _logger.LogError(e, "Failed to send encrypted receipts: {Message}", e.Message);
            throw new CheqiSDKException("Failed to send encrypted receipts: " + e.Message, e);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unexpected error sending encrypted receipts: {Message}", e.Message);
            throw new CheqiSDKException("Failed to send encrypted receipts: " + e.Message, e);
        }
    }

    /// <summary>
    /// Sends encrypted receipts to the backend for delivery to customer devices.
    /// </summary>
    public async Task<ReceiptCreatedResponse> SendEncryptedReceiptsAsync(
        HashSet<EncryptedReceiptRequestDto> encryptedReceipts,
        string templateHash,
        RecipientResolutionResponse recipientResolutionResponse,
        string accessToken,
        CancellationToken cancellationToken = default)
    {
        if (encryptedReceipts == null || encryptedReceipts.Count == 0)
            throw new CheqiSDKException("Encrypted receipts cannot be null or empty",
                CheqiSDKException.ErrorCodes.VALIDATION_ERROR, 400, null);

        if (string.IsNullOrWhiteSpace(templateHash))
            throw new CheqiSDKException("Template hash cannot be null or empty",
                CheqiSDKException.ErrorCodes.VALIDATION_ERROR, 400, null);

        if (recipientResolutionResponse == null)
            throw new CheqiSDKException("Customer match response cannot be null",
                CheqiSDKException.ErrorCodes.VALIDATION_ERROR, 400, null);

        if (!recipientResolutionResponse.customerFound)
            throw new CheqiSDKException("Cannot send receipts: customer was not found during matching",
                CheqiSDKException.ErrorCodes.CUSTOMER_NOT_FOUND, 400, null);

        if (string.IsNullOrWhiteSpace(accessToken))
            throw new CheqiSDKException("Access token cannot be null or empty",
                CheqiSDKException.ErrorCodes.AUTHENTICATION_FAILED, 401, null);

        _logger.LogDebug("Sending {Count} encrypted receipts to backend", encryptedReceipts.Count);

        try
        {
            var request = EncryptedReceiptsRequest.Builder()
                .EncryptedReceipts(encryptedReceipts)
                .MatchId(recipientResolutionResponse.matchId ?? throw new CheqiSDKException("MatchId is required", CheqiSDKException.ErrorCodes.VALIDATION_ERROR, 400, null))
                .TemplateHash(templateHash)
                .Build();

            var response = await _apiClient.SubmitEncryptedReceiptsAsync(request, accessToken, cancellationToken);
            _logger.LogInformation("Successfully sent {Count} encrypted receipts", encryptedReceipts.Count);
            return response;
        }
        catch (CheqiApiException e)
        {
            _logger.LogError(e, "Failed to send encrypted receipts: {Message}", e.Message);
            throw new CheqiSDKException("Failed to send encrypted receipts: " + e.Message, e);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unexpected error sending encrypted receipts: {Message}", e.Message);
            throw new CheqiSDKException("Failed to send encrypted receipts: " + e.Message, e);
        }
    }

    /// <summary>
    /// Sends receipt via email using API key from SDK config.
    /// </summary>
    public async Task<ReceiptResult> SendReceiptViaEmailAsync(
        string customerEmail,
        PurchaseReceipt purchaseReceipt,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(customerEmail) || !customerEmail.Contains("@"))
            throw new CheqiSDKException("Valid customer email is required",
                CheqiSDKException.ErrorCodes.VALIDATION_ERROR, 400, null);

        if (purchaseReceipt == null)
            throw new CheqiSDKException("Purchase receipt cannot be null",
                CheqiSDKException.ErrorCodes.VALIDATION_ERROR, 400, null);

        _logger.LogDebug("Sending receipt via email to: {Email}", MaskEmail(customerEmail));

        try
        {
            await _apiClient.SendReceiptViaEmailAsync(customerEmail, purchaseReceipt, null, cancellationToken);
            _logger.LogInformation("Successfully sent receipt via email to: {Email}", MaskEmail(customerEmail));
            return ReceiptResult.DeliveredViaEmail(customerEmail);
        }
        catch (CheqiApiException e)
        {
            _logger.LogError(e, "Failed to send receipt via email: {Message}", e.Message);
            throw new CheqiSDKException("Failed to send receipt via email: " + e.Message, e);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unexpected error sending receipt via email: {Message}", e.Message);
            throw new CheqiSDKException("Failed to send receipt via email: " + e.Message, e);
        }
    }

    /// <summary>
    /// Sends a receipt via email when customer doesn't have Cheqi registration.
    /// </summary>
    public async Task<ReceiptResult> SendReceiptViaEmailAsync(
        string customerEmail,
        PurchaseReceipt purchaseReceipt,
        string accessToken,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(customerEmail) || !customerEmail.Contains("@"))
            throw new CheqiSDKException("Valid customer email is required",
                CheqiSDKException.ErrorCodes.VALIDATION_ERROR, 400, null);

        if (purchaseReceipt == null)
            throw new CheqiSDKException("Purchase receipt cannot be null",
                CheqiSDKException.ErrorCodes.VALIDATION_ERROR, 400, null);

        if (string.IsNullOrWhiteSpace(accessToken))
            throw new CheqiSDKException("Access token cannot be null or empty",
                CheqiSDKException.ErrorCodes.AUTHENTICATION_FAILED, 401, null);

        _logger.LogDebug("Sending receipt via email to: {Email}", MaskEmail(customerEmail));

        try
        {
            await _apiClient.SendReceiptViaEmailAsync(customerEmail, purchaseReceipt, accessToken, cancellationToken);
            _logger.LogInformation("Successfully sent receipt via email to: {Email}", MaskEmail(customerEmail));
            return ReceiptResult.DeliveredViaEmail(customerEmail);
        }
        catch (CheqiApiException e)
        {
            _logger.LogError(e, "Failed to send receipt via email: {Message}", e.Message);
            throw new CheqiSDKException("Failed to send receipt via email: " + e.Message, e);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unexpected error sending receipt via email: {Message}", e.Message);
            throw new CheqiSDKException("Failed to send receipt via email: " + e.Message, e);
        }
    }

    /// <summary>
    /// Masks an email address for logging purposes to protect privacy.
    /// Example: john.doe@example.com becomes j***e@example.com
    /// </summary>
    private string MaskEmail(string email)
    {
        if (string.IsNullOrEmpty(email) || !email.Contains("@"))
        {
            return "***";
        }

        var parts = email.Split('@');
        var localPart = parts[0];

        if (localPart.Length <= 2)
        {
            return "***@" + parts[1];
        }

        return localPart[0] + "***" + localPart[^1] + "@" + parts[1];
    }
}
