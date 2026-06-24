using Cheqi.Sdk.Decryption;
using Cheqi.Sdk.Encryption;
using Cheqi.Sdk.Exceptions;
using Cheqi.Sdk.Http;
using Cheqi.Sdk.Http.Exceptions;
using Cheqi.Sdk.Matching;
using Cheqi.Sdk.Models;
using Cheqi.Sdk.Utils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Linq;
using System.Text.Json;
using DecryptionException = Cheqi.Sdk.Decryption.DecryptionException;
using Recipient = Cheqi.Sdk.Models.Generated.MatchedRecipient;
using RecipientResolutionResponse = Cheqi.Sdk.Models.Generated.RecipientResolutionResponse;
using IdentificationDetails = Cheqi.Sdk.Models.Generated.IdentificationDetails;
using ReceiptFormat = Cheqi.Sdk.Models.Generated.ReceiptFormat;
using CreditNoteTemplateResponse = Cheqi.Sdk.Models.Generated.CreditNoteTemplateResponse;
using CreditNoteTemplateGenerationRequest = Cheqi.Sdk.Models.Generated.CreditNoteTemplateGenerationRequest;

namespace Cheqi.Sdk.CreditNote;

/// <summary>
/// Service for processing credit note requests with end-to-end encryption.
///
/// Mirrors the Java SDK credit-note flow:
/// - Customer matching via cheqiReceiptId (links to the original receipt)
/// - Receipt format determination from the matched recipients
/// - Typed credit-note template generation (CHEQI / UBL credit-note formats)
/// - Per-recipient encrypted credit-note envelopes (only the formats each recipient accepts)
/// - Decryption of delivered credit notes and incoming credit-note requests
/// </summary>
public class CreditNoteService
{
    private readonly ILogger<CreditNoteService> _logger;
    private readonly DecryptionService _decryptionService;
    private readonly MatchingService _matchingService;
    private readonly ICheqiApiClient _apiClient;
    private readonly EncryptionService _encryptionService;
    private readonly CreditNoteMergeService _creditNoteMergeService;
    private readonly JsonSerializerOptions _jsonOptions;

    public CreditNoteService(
        ICheqiApiClient apiClient,
        EncryptionService encryptionService,
        MatchingService matchingService,
        ILogger<CreditNoteService>? logger = null)
    {
        _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
        _encryptionService = encryptionService ?? throw new ArgumentNullException(nameof(encryptionService));
        _matchingService = matchingService ?? throw new ArgumentNullException(nameof(matchingService));
        _logger = logger ?? NullLogger<CreditNoteService>.Instance;

        _decryptionService = new DecryptionService();
        _creditNoteMergeService = new CreditNoteMergeService();

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            WriteIndented = false
        };

        _logger.LogInformation("CreditNoteService initialized successfully");
    }

    #region Process Complete Credit Note

    /// <summary>
    /// Complete credit note processing workflow (mirrors Java <c>processCompleteCreditNote</c>):
    /// 1. Matches the customer using the original receipt id (cheqiReceiptId).
    /// 2. If no route is found and no email is provided, returns early.
    /// 3. Determines accepted formats and generates the credit-note template.
    /// 4. If a route is found: builds a per-recipient envelope, encrypts and submits.
    /// </summary>
    /// <param name="identificationDetails">Customer identification details (must include cheqiReceiptId).</param>
    /// <param name="creditNoteTemplateRequest">Credit note template request.</param>
    /// <param name="accessToken">OAuth access token, or null to use the API key from SDK config.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<CreditNoteResult> ProcessCompleteCreditNoteAsync(
        IdentificationDetails identificationDetails,
        CreditNoteTemplateRequest creditNoteTemplateRequest,
        string? accessToken = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(identificationDetails);
        ArgumentNullException.ThrowIfNull(creditNoteTemplateRequest);

        if (string.IsNullOrWhiteSpace(identificationDetails.CheqiReceiptId))
        {
            throw new CheqiSDKException(
                "Original receipt ID (cheqiReceiptId) is required for credit note processing",
                CheqiSDKException.ErrorCodes.VALIDATION_ERROR, 400, null);
        }

        var originalReceiptId = identificationDetails.CheqiReceiptId!;

        try
        {
            _logger.LogDebug("Matching customer with {Auth}", accessToken != null ? "access token" : "API key");
            var matchResponse = accessToken != null
                ? await _matchingService.MatchCustomerAsync(identificationDetails, accessToken, cancellationToken)
                : await _matchingService.MatchCustomerAsync(identificationDetails, cancellationToken);

            if (matchResponse.RouteFound != true && string.IsNullOrWhiteSpace(identificationDetails.RecipientEmail))
            {
                _logger.LogInformation("Customer not found and no email provided - skipping template generation");
                return CreditNoteResult.NoRecipients();
            }

            var formats = DetermineFormats(matchResponse);

            _logger.LogDebug("Generating credit note template");
            var templateResponse = await GenerateCreditNoteTemplateAsync(creditNoteTemplateRequest, formats, matchResponse, accessToken, cancellationToken);

            if (matchResponse.RouteFound != true)
            {
                _logger.LogInformation("Customer not found but email provided - credit note template generated but not sent");
                return CreditNoteResult.NoRecipients();
            }

            return await DeliverEncryptedCreditNotesAsync(originalReceiptId, matchResponse, templateResponse, accessToken, cancellationToken);
        }
        catch (CheqiSDKException)
        {
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unexpected error during credit note processing: {Message}", e.Message);
            throw new CheqiSDKException("Credit note processing failed: " + e.Message, e);
        }
    }

    #endregion

    #region Decrypt Credit Note Request

    /// <summary>
    /// Decrypts and parses an encrypted credit note request.
    /// </summary>
    public CreditNoteInitiationRequest DecryptCreditNoteRequest(
        EncryptedCreditNoteInitiationRequest encryptedCreditNote,
        string privateKeyBase64)
    {
        ArgumentNullException.ThrowIfNull(encryptedCreditNote);
        ArgumentNullException.ThrowIfNull(privateKeyBase64);

        _logger.LogDebug("Processing credit note request for receipt: {ReceiptId}", encryptedCreditNote.CheqiReceiptId);

        try
        {
            var decryptedCreditNote = _decryptionService.DecryptCreditNote(encryptedCreditNote, privateKeyBase64);

            var creditNoteRequest = JsonSerializer.Deserialize<CreditNoteInitiationRequest>(
                decryptedCreditNote.CreditNoteContentJson, _jsonOptions)
                ?? throw new CheqiSDKException("Failed to deserialize credit note request", CheqiSDKException.ErrorCodes.INVALID_RESPONSE, 500, null);

            _logger.LogInformation("Successfully processed credit note request for receipt: {ReceiptId}", creditNoteRequest.CheqiReceiptId);
            return creditNoteRequest;
        }
        catch (DecryptionException)
        {
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to parse credit note JSON: {Message}", e.Message);
            throw new CheqiSDKException("Failed to parse credit note request: " + e.Message, e);
        }
    }

    /// <summary>
    /// Decrypts and parses an encrypted credit note request asynchronously.
    /// </summary>
    public Task<CreditNoteInitiationRequest> DecryptCreditNoteRequestAsync(
        EncryptedCreditNoteInitiationRequest encryptedCreditNote,
        string privateKeyBase64,
        CancellationToken cancellationToken = default)
    {
        return Task.Run(() => DecryptCreditNoteRequest(encryptedCreditNote, privateKeyBase64), cancellationToken);
    }

    public DecryptedDeliveredCreditNote DecryptDeliveredCreditNote(
        Models.Generated.EncryptedCreditNoteDto encryptedCreditNote,
        string privateKeyBase64)
    {
        ArgumentNullException.ThrowIfNull(encryptedCreditNote);
        ArgumentNullException.ThrowIfNull(privateKeyBase64);

        var decryptedCreditNote = _decryptionService.DecryptDeliveredCreditNote(encryptedCreditNote, privateKeyBase64);
        return _creditNoteMergeService.Merge(decryptedCreditNote);
    }

    public Task<DecryptedDeliveredCreditNote> DecryptDeliveredCreditNoteAsync(
        Models.Generated.EncryptedCreditNoteDto encryptedCreditNote,
        string privateKeyBase64,
        CancellationToken cancellationToken = default)
    {
        return Task.Run(() => DecryptDeliveredCreditNote(encryptedCreditNote, privateKeyBase64), cancellationToken);
    }

    #endregion

    #region Generate Credit Note Template

    /// <summary>
    /// Generates a credit note template (mirrors Java <c>generateCreditNoteTemplate</c>).
    /// </summary>
    public async Task<CreditNoteTemplateResponse> GenerateCreditNoteTemplateAsync(
        CreditNoteTemplateRequest request,
        List<ReceiptFormat> formats,
        RecipientResolutionResponse matchResponse,
        string? accessToken = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        _logger.LogDebug("Generating credit note template for: {DocumentNumber}", request.DocumentNumber);

        var generationRequest = BuildGenerationRequest(request, formats, matchResponse);

        try
        {
            var template = await _apiClient.GenerateCreditNoteTemplateAsync(generationRequest, accessToken, cancellationToken);
            _logger.LogDebug("Credit note template generated successfully");
            return template;
        }
        catch (CheqiApiException)
        {
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to generate credit note template: {Message}", e.Message);
            throw new CheqiSDKException("Failed to generate credit note template: " + e.Message, e);
        }
    }

    #endregion

    #region Send Encrypted Credit Notes

    /// <summary>
    /// Sends encrypted credit notes to the backend for delivery.
    /// </summary>
    public async Task<CreditNoteCreatedResponse> SendEncryptedCreditNotesAsync(
        string parentCheqiReceiptId,
        HashSet<EncryptedCreditNote> encryptedCreditNotes,
        string templateHash,
        RecipientResolutionResponse recipientResolutionResponse,
        string? accessToken = null,
        CancellationToken cancellationToken = default)
    {
        if (encryptedCreditNotes == null || encryptedCreditNotes.Count == 0)
        {
            throw new CheqiSDKException("Encrypted credit notes cannot be null or empty",
                CheqiSDKException.ErrorCodes.VALIDATION_ERROR, 400, null);
        }

        if (string.IsNullOrWhiteSpace(templateHash))
        {
            throw new CheqiSDKException("Template hash cannot be null or empty",
                CheqiSDKException.ErrorCodes.VALIDATION_ERROR, 400, null);
        }

        ArgumentNullException.ThrowIfNull(recipientResolutionResponse);

        if (recipientResolutionResponse.RouteFound != true)
        {
            throw new CheqiSDKException("Cannot send credit notes: customer was not found during matching",
                CheqiSDKException.ErrorCodes.CUSTOMER_NOT_FOUND, 400, null);
        }

        _logger.LogDebug("Sending {Count} encrypted credit notes to backend", encryptedCreditNotes.Count);

        try
        {
            var request = new EncryptedCreditNotesRequest
            {
                MatchId = recipientResolutionResponse.MatchId,
                ParentCheqiReceiptId = parentCheqiReceiptId,
                EncryptedCreditNotes = encryptedCreditNotes,
                TemplateHash = templateHash
            };

            var response = await _apiClient.SubmitEncryptedCreditNotesAsync(request, accessToken, cancellationToken);
            _logger.LogInformation("Successfully sent {Count} encrypted credit notes", encryptedCreditNotes.Count);
            return response;
        }
        catch (CheqiApiException e)
        {
            _logger.LogError(e, "Failed to send encrypted credit notes: {Message}", e.Message);
            throw new CheqiSDKException("Failed to send encrypted credit notes: " + e.Message, e);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unexpected error sending encrypted credit notes: {Message}", e.Message);
            throw new CheqiSDKException("Failed to send encrypted credit notes: " + e.Message, e);
        }
    }

    #endregion

    #region Create Encrypted Credit Notes

    /// <summary>
    /// Creates an encrypted credit note for a single recipient.
    /// </summary>
    public EncryptedCreditNote CreateEncryptedCreditNote(string creditNoteEnvelope, Recipient recipient)
    {
        if (string.IsNullOrWhiteSpace(creditNoteEnvelope))
        {
            throw new CheqiSDKException("Credit note envelope cannot be null or empty",
                CheqiSDKException.ErrorCodes.VALIDATION_ERROR, 400, null);
        }

        ArgumentNullException.ThrowIfNull(recipient);

        try
        {
            return _encryptionService.EncryptCreditNoteForRecipient(creditNoteEnvelope, recipient);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to create encrypted credit note: {Message}", e.Message);
            throw new CheqiSDKException("Failed to create encrypted credit note: " + e.Message, e);
        }
    }

    #endregion

    #region Private Helpers

    private async Task<CreditNoteResult> DeliverEncryptedCreditNotesAsync(
        string originalReceiptId,
        RecipientResolutionResponse matchResponse,
        CreditNoteTemplateResponse templateResponse,
        string? accessToken,
        CancellationToken cancellationToken)
    {
        var recipients = matchResponse.Recipients ?? new List<Recipient>();
        _logger.LogDebug("Creating encrypted credit notes for {Count} recipients", recipients.Count);

        var encryptedCreditNotes = new HashSet<EncryptedCreditNote>();
        foreach (var recipient in recipients)
        {
            var envelope = BuildEnvelopeForRecipient(recipient, templateResponse);
            var envelopeJson = JsonSerializer.Serialize(envelope, _jsonOptions);
            encryptedCreditNotes.Add(_encryptionService.EncryptCreditNoteForRecipient(envelopeJson, recipient));
        }

        string templateContent = templateResponse.Cheqi != null
            ? JsonSerializer.Serialize(templateResponse.Cheqi, _jsonOptions)
            : (templateResponse.UblCreditNote ?? string.Empty);
        string templateHash = HashUtils.Sha256Hex(templateContent);

        _logger.LogDebug("Sending {Count} encrypted credit notes to backend", encryptedCreditNotes.Count);
        var response = await SendEncryptedCreditNotesAsync(originalReceiptId, encryptedCreditNotes, templateHash, matchResponse, accessToken, cancellationToken);

        _logger.LogInformation("Credit note processing completed successfully: {Count} credit notes created", encryptedCreditNotes.Count);
        return CreditNoteResult.Success(response.CreditNoteId ?? string.Empty, response.RecipientCount);
    }

    private CreditNoteEnvelope BuildEnvelopeForRecipient(Recipient recipient, CreditNoteTemplateResponse templateResponse)
    {
        var envelope = new CreditNoteEnvelope();
        var acceptedFormats = recipient.AcceptedFormats;

        if (acceptedFormats != null && acceptedFormats.Contains(ReceiptFormat.CHEQI) && templateResponse.Cheqi != null)
        {
            envelope.CheqiCreditNote = templateResponse.Cheqi;
        }
        if (acceptedFormats != null && acceptedFormats.Contains(ReceiptFormat.UBLCREDITNOTE) && templateResponse.UblCreditNote != null)
        {
            envelope.UblCreditNote = templateResponse.UblCreditNote;
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

    private CreditNoteTemplateGenerationRequest BuildGenerationRequest(
        CreditNoteTemplateRequest request, List<ReceiptFormat> formats, RecipientResolutionResponse matchResponse)
    {
        // Java hardcodes taxesApplied = true for credit notes.
        return new CreditNoteTemplateGenerationRequest(
            request.ToGenerated(),
            formats,
            matchResponse.BuyerCountryCode ?? string.Empty,
            matchResponse.BuyerType ?? default,
            true);
    }

    #endregion
}
