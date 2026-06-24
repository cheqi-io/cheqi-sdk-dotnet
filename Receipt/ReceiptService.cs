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
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using EncryptedReceiptRequestDto = Cheqi.Sdk.Models.Generated.EncryptedReceiptRequest;
using Recipient = Cheqi.Sdk.Models.Generated.MatchedRecipient;
using EncryptedReceiptsRequest = Cheqi.Sdk.Models.Generated.EncryptedReceiptsRequest;
using RecipientResolutionResponse = Cheqi.Sdk.Models.Generated.RecipientResolutionResponse;
using IdentificationDetails = Cheqi.Sdk.Models.Generated.IdentificationDetails;
using ReceiptCreatedResponse = Cheqi.Sdk.Models.Generated.ReceiptCreatedResponse;
using ReceiptFormat = Cheqi.Sdk.Models.Generated.ReceiptFormat;
using ReceiptTemplateResponse = Cheqi.Sdk.Models.Generated.ReceiptTemplateResponse;
using ReceiptTemplateGenerationRequest = Cheqi.Sdk.Models.Generated.ReceiptTemplateGenerationRequest;
using ReceiptEnvelope = Cheqi.Sdk.Models.Generated.ReceiptEnvelope;
using CheqiReceipt = Cheqi.Sdk.Models.Generated.CheqiReceipt;
using NotificationDisplayCode = Cheqi.Sdk.Models.Generated.NotificationDisplayCode;

namespace Cheqi.Sdk.Receipt;

/// <summary>
/// Service for handling receipt operations in the Cheqi SDK.
///
/// This service mirrors the Java SDK receipt flow:
/// - Customer matching / recipient resolution
/// - VAT context enrichment from the match response
/// - Receipt format determination from the matched recipients
/// - Typed receipt template generation (CHEQI / UBL formats + VAT metadata)
/// - Per-recipient encrypted receipt envelopes (only the formats each recipient accepts)
/// - Email fallback when no Cheqi route is found
/// </summary>
public class ReceiptService
{
    private readonly ILogger<ReceiptService> _logger;
    private readonly ICheqiApiClient _apiClient;
    private readonly EncryptionService _encryptionService;
    private readonly MatchingService _matchingService;
    private readonly JsonSerializerOptions _jsonOptions;

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
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = false,
            Converters = { new DecimalJsonConverter() }
        };
        _logger.LogInformation("ReceiptService initialized successfully");
    }

    /// <summary>
    /// Complete receipt processing workflow (mirrors Java <c>processCompleteReceipt</c>):
    /// 1. Matches the customer using payment details.
    /// 2. If no route is found and no email is provided, returns early.
    /// 3. Determines accepted formats and enriches VAT context from the match.
    /// 4. Generates the receipt template.
    /// 5. If a route is found: builds a per-recipient envelope, encrypts and submits.
    /// 6. Otherwise: sends the receipt via email.
    /// </summary>
    /// <param name="identificationDetails">Payment information for customer matching.</param>
    /// <param name="receiptRequest">Receipt template generation request.</param>
    /// <param name="accessToken">OAuth access token, or null to use the API key from SDK config.</param>
    /// <param name="notificationDisplayCode">Optional barcode/QR code for the push notification.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<ReceiptResult> ProcessCompleteReceiptAsync(
        IdentificationDetails identificationDetails,
        ReceiptTemplateRequest receiptRequest,
        string? accessToken = null,
        NotificationDisplayCode? notificationDisplayCode = null,
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
            _logger.LogDebug("Matching customer using payment details with {Auth}", accessToken != null ? "access token" : "API key");
            var matchResponse = accessToken != null
                ? await _matchingService.MatchCustomerAsync(identificationDetails, accessToken, cancellationToken)
                : await _matchingService.MatchCustomerAsync(identificationDetails, cancellationToken);

            if (matchResponse.RouteFound != true && string.IsNullOrEmpty(identificationDetails.RecipientEmail))
            {
                _logger.LogInformation("Customer not found and no email provided - skipping template generation");
                return ReceiptResult.CustomerNotFound();
            }

            var formats = DetermineFormats(matchResponse);
            var enrichedRequest = EnrichWithVatContext(receiptRequest, matchResponse);

            _logger.LogDebug("Generating receipt template");
            var templateResponse = await GenerateReceiptTemplateAsync(enrichedRequest, formats, accessToken, cancellationToken);

            if (matchResponse.RouteFound == true)
            {
                return await DeliverEncryptedReceiptsAsync(matchResponse, templateResponse, notificationDisplayCode, accessToken, cancellationToken);
            }

            _logger.LogInformation("Customer not found, sending receipt via email");
            return await SendReceiptViaEmailAsync(identificationDetails.RecipientEmail!, templateResponse.Cheqi!, accessToken, cancellationToken);
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
    /// Generates a receipt template from transaction data (mirrors Java <c>generateReceiptTemplate</c>).
    /// </summary>
    public async Task<ReceiptTemplateResponse> GenerateReceiptTemplateAsync(
        ReceiptTemplateRequest request,
        List<ReceiptFormat> receiptFormats,
        string? accessToken = null,
        CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new CheqiSDKException("Receipt template request cannot be null",
                CheqiSDKException.ErrorCodes.VALIDATION_ERROR, 400, null);

        _logger.LogDebug("Generating receipt template for receiptId: {DocumentNumber}", request.DocumentNumber);

        var generationRequest = BuildGenerationRequest(request, receiptFormats);

        try
        {
            var template = await _apiClient.GenerateReceiptTemplateAsync(generationRequest, accessToken, cancellationToken);
            _logger.LogDebug("Receipt template generated successfully");
            return template;
        }
        catch (CheqiApiException)
        {
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to generate receipt template: {Message}", e.Message);
            throw new CheqiSDKException("Failed to generate receipt template: " + e.Message, e);
        }
    }

    /// <summary>
    /// Creates an encrypted receipt for a single recipient.
    /// </summary>
    public EncryptedReceiptRequestDto CreateEncryptedReceipts(string purchaseReceipt, Recipient matchedRecipient)
    {
        if (string.IsNullOrWhiteSpace(purchaseReceipt))
            throw new CheqiSDKException("Purchase receipt cannot be null or empty",
                CheqiSDKException.ErrorCodes.VALIDATION_ERROR, 400, null);

        if (matchedRecipient == null)
            throw new CheqiSDKException("matchedRecipient cannot be null",
                CheqiSDKException.ErrorCodes.VALIDATION_ERROR, 400, null);

        try
        {
            return _encryptionService.EncryptReceiptForRecipient(purchaseReceipt, matchedRecipient);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to create encrypted receipts: {Message}", e.Message);
            throw new CheqiSDKException("Failed to create encrypted receipts: " + e.Message, e);
        }
    }

    /// <summary>
    /// Sends encrypted receipts to the backend for delivery to customer devices.
    /// </summary>
    public async Task<ReceiptCreatedResponse> SendEncryptedReceiptsAsync(
        HashSet<EncryptedReceiptRequestDto> encryptedReceipts,
        string templateHash,
        RecipientResolutionResponse recipientResolutionResponse,
        string? accessToken = null,
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

        if (recipientResolutionResponse.RouteFound != true)
            throw new CheqiSDKException("Cannot send receipts: customer was not found during matching",
                CheqiSDKException.ErrorCodes.CUSTOMER_NOT_FOUND, 400, null);

        _logger.LogDebug("Sending {Count} encrypted receipts to backend", encryptedReceipts.Count);

        try
        {
            var request = new EncryptedReceiptsRequest(
                encryptedReceipts.ToList(),
                recipientResolutionResponse.MatchId ?? throw new CheqiSDKException("MatchId is required", CheqiSDKException.ErrorCodes.VALIDATION_ERROR, 400, null),
                templateHash);

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
    /// Sends a receipt via email (fallback when the customer is not found in Cheqi).
    /// </summary>
    public async Task<ReceiptResult> SendReceiptViaEmailAsync(
        string customerEmail,
        CheqiReceipt cheqiReceipt,
        string? accessToken = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(customerEmail) || !customerEmail.Contains("@"))
            throw new CheqiSDKException("Valid customer email is required",
                CheqiSDKException.ErrorCodes.VALIDATION_ERROR, 400, null);

        if (cheqiReceipt == null)
            throw new CheqiSDKException("Cheqi receipt cannot be null",
                CheqiSDKException.ErrorCodes.VALIDATION_ERROR, 400, null);

        _logger.LogDebug("Sending receipt via email to: {Email}", MaskEmail(customerEmail));

        try
        {
            await _apiClient.SendReceiptViaEmailAsync(customerEmail, cheqiReceipt, accessToken, cancellationToken);
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

    // ===== PRIVATE HELPERS =====

    private async Task<ReceiptResult> DeliverEncryptedReceiptsAsync(
        RecipientResolutionResponse matchResponse,
        ReceiptTemplateResponse templateResponse,
        NotificationDisplayCode? notificationDisplayCode,
        string? accessToken,
        CancellationToken cancellationToken)
    {
        var recipients = matchResponse.Recipients ?? new List<Recipient>();
        _logger.LogDebug("Creating encrypted receipts for {Count} recipients", recipients.Count);

        var encryptedReceipts = new HashSet<EncryptedReceiptRequestDto>();
        foreach (var recipient in recipients)
        {
            var envelope = BuildEnvelopeForRecipient(recipient, templateResponse);
            var envelopeJson = JsonSerializer.Serialize(envelope, _jsonOptions);
            var encryptedReceipt = _encryptionService.EncryptReceiptForRecipient(envelopeJson, recipient);
            if (notificationDisplayCode != null)
            {
                encryptedReceipt.NotificationDisplayCode = notificationDisplayCode;
            }
            encryptedReceipts.Add(encryptedReceipt);
        }

        string templateContent = templateResponse.Cheqi != null
            ? JsonSerializer.Serialize(templateResponse.Cheqi, _jsonOptions)
            : (templateResponse.UblPurchaseReceipt ?? string.Empty);
        string templateHash = HashUtils.Sha256Hex(templateContent);

        _logger.LogDebug("Sending {Count} encrypted receipts to backend", encryptedReceipts.Count);
        var response = await SendEncryptedReceiptsAsync(encryptedReceipts, templateHash, matchResponse, accessToken, cancellationToken);

        _logger.LogInformation("Receipt processing completed successfully");
        return ReceiptResult.DeliveredToApp(response, templateContent);
    }

    private ReceiptEnvelope BuildEnvelopeForRecipient(Recipient recipient, ReceiptTemplateResponse templateResponse)
    {
        var envelope = new ReceiptEnvelope();
        var acceptedFormats = recipient.AcceptedFormats;

        if (acceptedFormats != null && acceptedFormats.Contains(ReceiptFormat.CHEQI) && templateResponse.Cheqi != null)
        {
            envelope.Cheqi = templateResponse.Cheqi;
        }
        if (acceptedFormats != null && acceptedFormats.Contains(ReceiptFormat.UBLPURCHASERECEIPT) && templateResponse.UblPurchaseReceipt != null)
        {
            envelope.UblPurchaseReceipt = templateResponse.UblPurchaseReceipt;
        }
        if (acceptedFormats != null && acceptedFormats.Contains(ReceiptFormat.UBLINVOICE) && templateResponse.UblInvoice != null)
        {
            envelope.UblInvoice = templateResponse.UblInvoice;
        }
        if (templateResponse.VatMetadata != null)
        {
            envelope.VatMetaData = templateResponse.VatMetadata;
        }
        return envelope;
    }

    private List<ReceiptFormat> DetermineFormats(RecipientResolutionResponse matchResponse)
    {
        return (matchResponse.Recipients ?? new List<Recipient>())
            .SelectMany(r => r.AcceptedFormats ?? new List<ReceiptFormat>())
            .Distinct()
            .ToList();
    }

    private ReceiptTemplateRequest EnrichWithVatContext(
        ReceiptTemplateRequest request, RecipientResolutionResponse matchResponse)
    {
        return ReceiptTemplateRequest.Builder()
            .From(request)
            .BuyerCountryCode(request.BuyerCountryCode ?? matchResponse.BuyerCountryCode)
            .BuyerType(request.BuyerType ?? matchResponse.BuyerType)
            .TaxesApplied(ResolveTaxesApplied(request))
            .Build();
    }

    private bool ResolveTaxesApplied(ReceiptTemplateRequest request)
    {
        if (request.TaxesApplied != null)
        {
            return request.TaxesApplied.Value;
        }
        if (request.TotalTaxAmount > 0)
        {
            return true;
        }
        return request.Taxes != null && request.Taxes.Count > 0;
    }

    private ReceiptTemplateGenerationRequest BuildGenerationRequest(
        ReceiptTemplateRequest request, List<ReceiptFormat> formats)
    {
        return new ReceiptTemplateGenerationRequest(
            request,
            formats,
            request.BuyerCountryCode ?? string.Empty,
            request.BuyerType ?? default,
            request.TaxesApplied ?? false);
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
