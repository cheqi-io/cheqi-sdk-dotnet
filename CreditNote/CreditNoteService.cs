using Cheqi.Sdk.Decryption;
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
using System.Text.Json;
using DecryptionException = Cheqi.Sdk.Decryption.DecryptionException;

namespace Cheqi.Sdk.CreditNote;

/// <summary>
/// Service for processing credit note requests with end-to-end encryption.
/// 
/// This service handles:
/// - Credit note generation and encryption for returns/refunds
/// - Decryption of encrypted credit note requests from customers
/// - Customer identification via cheqiReceiptId (links to original receipt)
/// - Bidirectional encrypted communication (merchant ↔ customer)
/// 
/// Note: Credit notes identify customers using the cheqiReceiptId from the original receipt,
/// not payment details like card PAR or IBAN. The cheqiReceiptId automatically resolves
/// to the customer who received the original receipt.
/// </summary>
public class CreditNoteService
{
    private readonly ILogger<CreditNoteService> _logger;
    private readonly DecryptionService _decryptionService;
    private readonly MatchingService _matchingService;
    private readonly ICheqiApiClient _apiClient;
    private readonly RFC8785Canonicalizer _canonicalizer;
    private readonly EncryptionService _encryptionService;
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
        _canonicalizer = new RFC8785Canonicalizer();
        
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
        
        _logger.LogInformation("CreditNoteService initialized successfully");
    }

    #region Process Complete Credit Note (API Key)

    /// <summary>
    /// Process complete credit note using API key from SDK config.
    /// Uses Bearer token authentication with the API key configured during SDK initialization.
    /// For companies accessing their own data directly.
    /// </summary>
    /// <param name="identificationDetails">Customer identification details</param>
    /// <param name="creditNoteTemplateRequest">Credit note template request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>CreditNoteResult indicating success or failure</returns>
    /// <exception cref="CheqiSDKException">if processing fails</exception>
    public async Task<CreditNoteResult> ProcessCompleteCreditNoteAsync(
        IdentificationDetails identificationDetails,
        CreditNoteTemplateRequest creditNoteTemplateRequest,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(identificationDetails);
        ArgumentNullException.ThrowIfNull(creditNoteTemplateRequest);

        // Validate that cheqiReceiptId is present for credit notes
        if (string.IsNullOrWhiteSpace(identificationDetails.cheqiReceiptId))
        {
            throw new CheqiSDKException(
                "Original receipt ID (cheqiReceiptId) is required for credit note processing",
                CheqiSDKException.ErrorCodes.VALIDATION_ERROR,
                400,
                null);
        }

        try
        {
            _logger.LogDebug("Matching customer with API key");
            var matchResponse = await _matchingService.MatchCustomerAsync(identificationDetails, cancellationToken);

            if (!matchResponse.customerFound && string.IsNullOrWhiteSpace(identificationDetails.recipientEmail))
            {
                _logger.LogInformation("Customer not found and no email provided - skipping template generation as no recipient is known");
                return CreditNoteResult.NoRecipients();
            }

            // Generate credit note template
            _logger.LogDebug("Generating credit note template with API key");
            string creditNoteTemplate = await GenerateCreditNoteTemplateAsync(creditNoteTemplateRequest, cancellationToken);

            var creditNote = JsonSerializer.Deserialize<Models.Ubl.CreditNote>(creditNoteTemplate, _jsonOptions)
                ?? throw new CheqiSDKException("Failed to deserialize credit note template", CheqiSDKException.ErrorCodes.INVALID_RESPONSE, 500, null);

            if (matchResponse.customerFound)
            {
                string creditNoteTemplateJson = JsonSerializer.Serialize(creditNote, _jsonOptions);
                string canonicalCreditNoteJson = _canonicalizer.Canonicalize(creditNoteTemplateJson);
                string templateHash = HashUtils.Sha256Hex(canonicalCreditNoteJson);

                _logger.LogDebug("Creating encrypted credit notes for {Count} recipients", matchResponse.recipients?.Count ?? 0);
                var encryptedCreditNotes = CreateEncryptedCreditNotes(
                    canonicalCreditNoteJson,
                    matchResponse.recipients ?? new List<Recipient>()
                );

                _logger.LogDebug("Sending {Count} encrypted credit notes to backend with API key", encryptedCreditNotes.Count);
                var response = await SendEncryptedCreditNotesAsync(
                    identificationDetails.cheqiReceiptId!,
                    encryptedCreditNotes,
                    templateHash,
                    matchResponse,
                    cancellationToken);

                _logger.LogInformation("Credit note processing completed successfully: {Count} credit notes created for {RecipientCount} recipients",
                    encryptedCreditNotes.Count, matchResponse.recipients?.Count ?? 0);
                return CreditNoteResult.Success(response.CreditNoteId ?? string.Empty, response.RecipientCount);
            }
            else
            {
                _logger.LogInformation("Customer not found but email provided - credit note template generated but not sent");
                return CreditNoteResult.NoRecipients();
            }
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

    #region Process Complete Credit Note (OAuth)

    /// <summary>
    /// Process complete credit note using OAuth2 access token.
    /// For third-party integrators accessing merchant data via OAuth2 authorization.
    /// </summary>
    /// <param name="identificationDetails">Customer identification details</param>
    /// <param name="creditNoteTemplateRequest">Credit note template request</param>
    /// <param name="accessToken">OAuth2 access token</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>CreditNoteResult indicating success or failure</returns>
    /// <exception cref="CheqiSDKException">if processing fails</exception>
    public async Task<CreditNoteResult> ProcessCompleteCreditNoteAsync(
        IdentificationDetails identificationDetails,
        CreditNoteTemplateRequest creditNoteTemplateRequest,
        string accessToken,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(identificationDetails);
        ArgumentNullException.ThrowIfNull(creditNoteTemplateRequest);

        if (string.IsNullOrWhiteSpace(accessToken))
        {
            throw new CheqiSDKException(
                "Access token cannot be null or empty",
                CheqiSDKException.ErrorCodes.AUTHENTICATION_FAILED,
                401,
                null);
        }

        // Validate that cheqiReceiptId is present for credit notes
        if (string.IsNullOrWhiteSpace(identificationDetails.cheqiReceiptId))
        {
            throw new CheqiSDKException(
                "Original receipt ID (cheqiReceiptId) is required for credit note processing",
                CheqiSDKException.ErrorCodes.VALIDATION_ERROR,
                400,
                null);
        }

        try
        {
            _logger.LogDebug("Matching customer with access token");
            var matchResponse = await _matchingService.MatchCustomerAsync(identificationDetails, accessToken, cancellationToken);

            if (!matchResponse.customerFound && string.IsNullOrWhiteSpace(identificationDetails.recipientEmail))
            {
                _logger.LogInformation("Customer not found and no email provided - skipping template generation as no recipient is known");
                return CreditNoteResult.NoRecipients();
            }

            _logger.LogDebug("Generating credit note template with access token");
            string creditNoteTemplate = await GenerateCreditNoteTemplateAsync(creditNoteTemplateRequest, accessToken, cancellationToken);

            var creditNote = JsonSerializer.Deserialize<Models.Ubl.CreditNote>(creditNoteTemplate, _jsonOptions)
                ?? throw new CheqiSDKException("Failed to deserialize credit note template", CheqiSDKException.ErrorCodes.INVALID_RESPONSE, 500, null);

            if (matchResponse.customerFound)
            {
                string creditNoteTemplateJson = JsonSerializer.Serialize(creditNote, _jsonOptions);
                string canonicalCreditNoteJson = _canonicalizer.Canonicalize(creditNoteTemplateJson);
                string templateHash = HashUtils.Sha256Hex(canonicalCreditNoteJson);

                _logger.LogDebug("Creating encrypted credit notes for {Count} recipients", matchResponse.recipients?.Count ?? 0);
                var encryptedCreditNotes = CreateEncryptedCreditNotes(
                    canonicalCreditNoteJson,
                    matchResponse.recipients ?? new List<Recipient>()
                );

                _logger.LogDebug("Sending {Count} encrypted credit notes to backend with access token", encryptedCreditNotes.Count);
                var response = await SendEncryptedCreditNotesAsync(
                    identificationDetails.cheqiReceiptId!,
                    encryptedCreditNotes,
                    templateHash,
                    matchResponse,
                    accessToken,
                    cancellationToken);

                _logger.LogInformation("Credit note processing completed successfully: {Count} credit notes created for {RecipientCount} recipients",
                    encryptedCreditNotes.Count, matchResponse.recipients?.Count ?? 0);
                return CreditNoteResult.Success(response.CreditNoteId ?? string.Empty, response.RecipientCount);
            }
            else
            {
                _logger.LogInformation("Customer not found but email provided - credit note template generated but not sent");
                return CreditNoteResult.NoRecipients();
            }
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
    /// <param name="encryptedCreditNote">The encrypted credit note from webhook or API</param>
    /// <param name="privateKeyBase64">The merchant's private key in Base64 PKCS#8 format</param>
    /// <returns>Parsed CreditNoteInitiationRequest object ready for processing</returns>
    /// <exception cref="DecryptionException">if decryption fails</exception>
    /// <exception cref="CheqiSDKException">if JSON parsing fails</exception>
    public CreditNoteInitiationRequest DecryptCreditNoteRequest(
        EncryptedCreditNoteInitiationRequest encryptedCreditNote,
        string privateKeyBase64)
    {
        ArgumentNullException.ThrowIfNull(encryptedCreditNote);
        ArgumentNullException.ThrowIfNull(privateKeyBase64);

        _logger.LogDebug("Processing credit note request for receipt: {ReceiptId}", encryptedCreditNote.CheqiReceiptId);

        try
        {
            // Step 1: Decrypt the credit note
            var decryptedCreditNote = _decryptionService.DecryptCreditNote(
                encryptedCreditNote,
                privateKeyBase64
            );

            // Step 2: Parse JSON to CreditNoteInitiationRequest
            var creditNoteRequest = JsonSerializer.Deserialize<CreditNoteInitiationRequest>(
                decryptedCreditNote.CreditNoteContentJson,
                _jsonOptions
            ) ?? throw new CheqiSDKException("Failed to deserialize credit note request", CheqiSDKException.ErrorCodes.INVALID_RESPONSE, 500, null);

            _logger.LogInformation("Successfully processed credit note request for receipt: {ReceiptId}",
                creditNoteRequest.CheqiReceiptId);

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
    /// <param name="encryptedCreditNote">The encrypted credit note from webhook or API</param>
    /// <param name="privateKeyBase64">The merchant's private key in Base64 PKCS#8 format</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Parsed CreditNoteInitiationRequest object ready for processing</returns>
    public Task<CreditNoteInitiationRequest> DecryptCreditNoteRequestAsync(
        EncryptedCreditNoteInitiationRequest encryptedCreditNote,
        string privateKeyBase64,
        CancellationToken cancellationToken = default)
    {
        return Task.Run(() => DecryptCreditNoteRequest(encryptedCreditNote, privateKeyBase64), cancellationToken);
    }

    #endregion

    #region Generate Credit Note Template

    /// <summary>
    /// Generates a credit note template using API key from SDK config.
    /// Uses Bearer token authentication with the API key configured during SDK initialization.
    /// </summary>
    /// <param name="request">Credit note template request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Generated credit note as JSON string</returns>
    /// <exception cref="CheqiSDKException">if generation fails</exception>
    public async Task<string> GenerateCreditNoteTemplateAsync(
        CreditNoteTemplateRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.CreditNoteId))
        {
            throw new CheqiSDKException(
                "Invalid credit note template request: CreditNoteId is required",
                CheqiSDKException.ErrorCodes.VALIDATION_ERROR,
                400,
                null);
        }

        _logger.LogDebug("Generating credit note template with API key for credit note: {CreditNoteId}", request.CreditNoteId);

        try
        {
            string template = await _apiClient.GenerateCreditNoteTemplateAsync(request, null, cancellationToken);
            _logger.LogDebug("Credit note template generated successfully");
            return template;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unexpected error generating credit note template: {Message}", e.Message);
            throw new CheqiSDKException("Failed to generate credit note template: " + e.Message, e);
        }
    }

    /// <summary>
    /// Generates a credit note template using OAuth2 access token.
    /// For third-party integrators accessing merchant data via OAuth2 authorization.
    /// </summary>
    /// <param name="request">Credit note template request</param>
    /// <param name="accessToken">OAuth2 access token</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Generated credit note as JSON string</returns>
    /// <exception cref="CheqiSDKException">if generation fails</exception>
    public async Task<string> GenerateCreditNoteTemplateAsync(
        CreditNoteTemplateRequest request,
        string accessToken,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.CreditNoteId))
        {
            throw new CheqiSDKException(
                "Invalid credit note template request: CreditNoteId is required",
                CheqiSDKException.ErrorCodes.VALIDATION_ERROR,
                400,
                null);
        }

        if (string.IsNullOrWhiteSpace(accessToken))
        {
            throw new CheqiSDKException(
                "Access token is required for credit note template generation",
                CheqiSDKException.ErrorCodes.AUTHENTICATION_FAILED,
                401,
                null);
        }

        _logger.LogDebug("Generating credit note template for credit note: {CreditNoteId}", request.CreditNoteId);

        try
        {
            string template = await _apiClient.GenerateCreditNoteTemplateAsync(request, accessToken, cancellationToken);
            _logger.LogDebug("Credit note template generated successfully");
            return template;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unexpected error generating credit note template: {Message}", e.Message);
            throw new CheqiSDKException("Failed to generate credit note template: " + e.Message, e);
        }
    }

    #endregion

    #region Send Encrypted Credit Notes

    /// <summary>
    /// Sends encrypted credit notes using API key from SDK config.
    /// Uses Bearer token authentication with the API key configured during SDK initialization.
    /// </summary>
    /// <param name="parentCheqiReceiptId">Original receipt ID</param>
    /// <param name="encryptedCreditNotes">Set of encrypted credit notes</param>
    /// <param name="templateHash">SHA-256 hash of canonical credit note JSON</param>
    /// <param name="recipientResolutionResponse">Customer match response</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Credit note creation response</returns>
    /// <exception cref="CheqiSDKException">if sending fails</exception>
    public async Task<CreditNoteCreatedResponse> SendEncryptedCreditNotesAsync(
        string parentCheqiReceiptId,
        HashSet<EncryptedCreditNote> encryptedCreditNotes,
        string templateHash,
        RecipientResolutionResponse recipientResolutionResponse,
        CancellationToken cancellationToken = default)
    {
        if (encryptedCreditNotes == null || encryptedCreditNotes.Count == 0)
        {
            throw new CheqiSDKException(
                "Encrypted credit notes cannot be null or empty",
                CheqiSDKException.ErrorCodes.VALIDATION_ERROR,
                400,
                null);
        }

        if (string.IsNullOrWhiteSpace(templateHash))
        {
            throw new CheqiSDKException(
                "Template hash cannot be null or empty",
                CheqiSDKException.ErrorCodes.VALIDATION_ERROR,
                400,
                null);
        }

        ArgumentNullException.ThrowIfNull(recipientResolutionResponse);

        if (!recipientResolutionResponse.customerFound)
        {
            throw new CheqiSDKException(
                "Cannot send credit notes: customer was not found during matching",
                CheqiSDKException.ErrorCodes.CUSTOMER_NOT_FOUND,
                400,
                null);
        }

        _logger.LogDebug("Sending {Count} encrypted credit notes to backend with API key", encryptedCreditNotes.Count);

        try
        {
            var request = new EncryptedCreditNotesRequest
            {
                MatchId = recipientResolutionResponse.matchId,
                ParentCheqiReceiptId = parentCheqiReceiptId,
                EncryptedCreditNotes = encryptedCreditNotes,
                TemplateHash = templateHash
            };

            var response = await _apiClient.SubmitEncryptedCreditNotesAsync(request, null, cancellationToken);
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

    /// <summary>
    /// Sends encrypted credit notes using OAuth2 access token.
    /// For third-party integrators accessing merchant data via OAuth2 authorization.
    /// </summary>
    /// <param name="parentCheqiReceiptId">Original receipt ID</param>
    /// <param name="encryptedCreditNotes">Set of encrypted credit notes</param>
    /// <param name="templateHash">SHA-256 hash of canonical credit note JSON</param>
    /// <param name="recipientResolutionResponse">Customer match response</param>
    /// <param name="accessToken">OAuth2 access token</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Credit note creation response</returns>
    /// <exception cref="CheqiSDKException">if sending fails</exception>
    public async Task<CreditNoteCreatedResponse> SendEncryptedCreditNotesAsync(
        string parentCheqiReceiptId,
        HashSet<EncryptedCreditNote> encryptedCreditNotes,
        string templateHash,
        RecipientResolutionResponse recipientResolutionResponse,
        string accessToken,
        CancellationToken cancellationToken = default)
    {
        if (encryptedCreditNotes == null || encryptedCreditNotes.Count == 0)
        {
            throw new CheqiSDKException(
                "Encrypted credit notes cannot be null or empty",
                CheqiSDKException.ErrorCodes.VALIDATION_ERROR,
                400,
                null);
        }

        if (string.IsNullOrWhiteSpace(templateHash))
        {
            throw new CheqiSDKException(
                "Template hash cannot be null or empty",
                CheqiSDKException.ErrorCodes.VALIDATION_ERROR,
                400,
                null);
        }

        ArgumentNullException.ThrowIfNull(recipientResolutionResponse);

        if (!recipientResolutionResponse.customerFound)
        {
            throw new CheqiSDKException(
                "Cannot send credit notes: customer was not found during matching",
                CheqiSDKException.ErrorCodes.CUSTOMER_NOT_FOUND,
                400,
                null);
        }

        if (string.IsNullOrWhiteSpace(accessToken))
        {
            throw new CheqiSDKException(
                "Access token cannot be null or empty",
                CheqiSDKException.ErrorCodes.AUTHENTICATION_FAILED,
                401,
                null);
        }

        _logger.LogDebug("Sending {Count} encrypted credit notes to backend with access token", encryptedCreditNotes.Count);

        try
        {
            var request = new EncryptedCreditNotesRequest
            {
                MatchId = recipientResolutionResponse.matchId,
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
    /// Creates encrypted credit notes for multiple customer devices.
    /// 
    /// This method takes a credit note template and encrypts it for each of the customer's
    /// registered devices using their respective public keys. Each device will receive
    /// its own encrypted version that can only be decrypted with its private key.
    /// </summary>
    /// <param name="creditNote">The credit note in canonical JSON format</param>
    /// <param name="recipients">List of recipients with their public key information</param>
    /// <returns>Set of encrypted credit notes, one per device</returns>
    /// <exception cref="CheqiSDKException">if encryption fails or parameters are invalid</exception>
    public HashSet<EncryptedCreditNote> CreateEncryptedCreditNotes(
        string creditNote,
        List<Recipient> recipients)
    {
        if (string.IsNullOrWhiteSpace(creditNote))
        {
            throw new CheqiSDKException(
                "Credit note cannot be null or empty",
                CheqiSDKException.ErrorCodes.VALIDATION_ERROR,
                400,
                null);
        }

        if (recipients == null || recipients.Count == 0)
        {
            throw new CheqiSDKException(
                "Recipients cannot be null or empty",
                CheqiSDKException.ErrorCodes.VALIDATION_ERROR,
                400,
                null);
        }

        _logger.LogDebug("Creating encrypted credit notes for {Count} recipients", recipients.Count);

        try
        {
            var encryptedCreditNotes = _encryptionService.EncryptCreditNoteForRecipients(creditNote, recipients);
            _logger.LogDebug("Successfully created {Count} encrypted credit notes", encryptedCreditNotes.Count);
            return encryptedCreditNotes;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unexpected error creating encrypted credit notes: {Message}", e.Message);
            throw new CheqiSDKException("Failed to create encrypted credit notes: " + e.Message, e);
        }
    }

    #endregion
}
