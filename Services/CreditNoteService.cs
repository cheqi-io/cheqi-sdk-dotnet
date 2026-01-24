using Cheqi.Sdk.CreditNote;
using Cheqi.Sdk.Encryption;
using Cheqi.Sdk.Exceptions;
using Cheqi.Sdk.Http;
using Cheqi.Sdk.Models;
using Microsoft.Extensions.Logging;

namespace Cheqi.Sdk.Services;

/// <summary>
/// Service for processing and delivering encrypted credit notes.
/// </summary>
public sealed class CreditNoteService
{
    private readonly ICheqiApiClient _apiClient;
    private readonly EncryptionService _encryptionService;
    private readonly MatchingService _matchingService;
    private readonly ILogger? _logger;

    public CreditNoteService(
        ICheqiApiClient apiClient,
        EncryptionService encryptionService,
        MatchingService matchingService,
        ILogger? logger = null)
    {
        _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
        _encryptionService = encryptionService ?? throw new ArgumentNullException(nameof(encryptionService));
        _matchingService = matchingService ?? throw new ArgumentNullException(nameof(matchingService));
        _logger = logger;
    }

    /// <summary>
    /// Generates a credit note template from the request.
    /// </summary>
    /// <param name="request">The credit note template request.</param>
    /// <param name="accessToken">Optional access token.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The generated credit note as JSON.</returns>
    public Task<string> GenerateCreditNoteTemplateAsync(
        CreditNoteTemplateRequest request,
        string? accessToken = null,
        CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        return _apiClient.GenerateCreditNoteTemplateAsync(request, accessToken, cancellationToken);
    }

    /// <summary>
    /// Creates encrypted credit notes for the specified recipients.
    /// </summary>
    /// <param name="creditNoteJson">The credit note as JSON.</param>
    /// <param name="recipients">The list of recipients.</param>
    /// <returns>The encrypted credit notes request DTO.</returns>
    public EncryptedCreditNoteRequestDto CreateEncryptedCreditNotes(string creditNoteJson, IEnumerable<Recipient> recipients)
    {
        if (string.IsNullOrEmpty(creditNoteJson))
            throw new ArgumentException("Credit note JSON cannot be null or empty", nameof(creditNoteJson));
        if (recipients == null)
            throw new ArgumentNullException(nameof(recipients));

        var encryptedData = _encryptionService.EncryptCreditNoteForRecipients(creditNoteJson, recipients);

        return new EncryptedCreditNoteRequestDto
        {
            EncryptedCreditNotes = encryptedData
        };
    }

    /// <summary>
    /// Submits encrypted credit notes to the API.
    /// </summary>
    /// <param name="request">The encrypted credit notes request.</param>
    /// <param name="accessToken">Optional access token.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The credit note creation response.</returns>
    public Task<CreditNoteCreatedResponse> SubmitEncryptedCreditNotesAsync(
        EncryptedCreditNoteRequestDto request,
        string? accessToken = null,
        CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        return _apiClient.SubmitEncryptedCreditNotesAsync(request, accessToken, cancellationToken);
    }

    /// <summary>
    /// Processes a complete credit note: generates template, matches customer, encrypts, and delivers.
    /// </summary>
    /// <param name="identificationDetails">Customer identification details.</param>
    /// <param name="creditNoteRequest">The credit note template request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The result of the credit note processing.</returns>
    public Task<CreditNoteResult> ProcessCompleteCreditNoteAsync(
        IdentificationDetails identificationDetails,
        CreditNoteTemplateRequest creditNoteRequest,
        CancellationToken cancellationToken = default)
    {
        return ProcessCompleteCreditNoteAsync(identificationDetails, creditNoteRequest, null, cancellationToken);
    }

    /// <summary>
    /// Processes a complete credit note with an explicit access token.
    /// </summary>
    /// <param name="identificationDetails">Customer identification details.</param>
    /// <param name="creditNoteRequest">The credit note template request.</param>
    /// <param name="accessToken">OAuth access token.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The result of the credit note processing.</returns>
    public async Task<CreditNoteResult> ProcessCompleteCreditNoteAsync(
        IdentificationDetails identificationDetails,
        CreditNoteTemplateRequest creditNoteRequest,
        string? accessToken,
        CancellationToken cancellationToken = default)
    {
        if (identificationDetails == null)
            throw new ArgumentNullException(nameof(identificationDetails));
        if (creditNoteRequest == null)
            throw new ArgumentNullException(nameof(creditNoteRequest));

        try
        {
            _logger?.LogDebug("Starting complete credit note processing for {CreditNoteId}", creditNoteRequest.CreditNoteId);

            // Step 1: Match customer
            var matchResult = await _matchingService.MatchCustomerAsync(identificationDetails, accessToken, cancellationToken);

            if (!matchResult.HasRecipients)
            {
                _logger?.LogInformation("No recipients found for credit note {CreditNoteId}", creditNoteRequest.CreditNoteId);
                return CreditNoteResult.NoRecipients();
            }

            _logger?.LogDebug("Found {Count} recipients for credit note {CreditNoteId}", matchResult.Recipients.Count, creditNoteRequest.CreditNoteId);

            // Step 2: Generate credit note template
            var creditNoteJson = await GenerateCreditNoteTemplateAsync(creditNoteRequest, accessToken, cancellationToken);

            // Step 3: Encrypt for all recipients
            var encryptedRequest = CreateEncryptedCreditNotes(creditNoteJson, matchResult.Recipients);

            // Step 4: Submit encrypted credit notes
            var response = await SubmitEncryptedCreditNotesAsync(encryptedRequest, accessToken, cancellationToken);

            if (!response.Success)
            {
                _logger?.LogWarning("Credit note submission failed for {CreditNoteId}: {Message}", creditNoteRequest.CreditNoteId, response.Message);
                return CreditNoteResult.Failure(response.Message ?? "Credit note submission failed", response.ErrorCode);
            }

            _logger?.LogInformation(
                "Credit note {CreditNoteId} processed successfully, delivered to {Count} recipients",
                creditNoteRequest.CreditNoteId,
                response.RecipientCount);

            return CreditNoteResult.Success(response.CreditNoteId ?? creditNoteRequest.CreditNoteId, response.RecipientCount);
        }
        catch (MatchingException ex)
        {
            _logger?.LogError(ex, "Customer matching failed for credit note {CreditNoteId}", creditNoteRequest.CreditNoteId);
            return CreditNoteResult.Failure($"Customer matching failed: {ex.Message}");
        }
        catch (EncryptionException ex)
        {
            _logger?.LogError(ex, "Encryption failed for credit note {CreditNoteId}", creditNoteRequest.CreditNoteId);
            return CreditNoteResult.Failure($"Encryption failed: {ex.Message}");
        }
        catch (CheqiApiException ex)
        {
            _logger?.LogError(ex, "API error processing credit note {CreditNoteId}", creditNoteRequest.CreditNoteId);
            return CreditNoteResult.Failure($"API error: {ex.Message}", ex.ErrorCode);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger?.LogError(ex, "Unexpected error processing credit note {CreditNoteId}", creditNoteRequest.CreditNoteId);
            throw new CreditNoteProcessingException($"Failed to process credit note: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Processes a credit note using pre-generated credit note JSON.
    /// </summary>
    /// <param name="identificationDetails">Customer identification details.</param>
    /// <param name="creditNoteJson">The pre-generated credit note JSON.</param>
    /// <param name="accessToken">Optional access token.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The result of the credit note processing.</returns>
    public async Task<CreditNoteResult> ProcessCreditNoteWithJsonAsync(
        IdentificationDetails identificationDetails,
        string creditNoteJson,
        string? accessToken = null,
        CancellationToken cancellationToken = default)
    {
        if (identificationDetails == null)
            throw new ArgumentNullException(nameof(identificationDetails));
        if (string.IsNullOrEmpty(creditNoteJson))
            throw new ArgumentException("Credit note JSON cannot be null or empty", nameof(creditNoteJson));

        try
        {
            // Step 1: Match customer
            var matchResult = await _matchingService.MatchCustomerAsync(identificationDetails, accessToken, cancellationToken);

            if (!matchResult.HasRecipients)
            {
                return CreditNoteResult.NoRecipients();
            }

            // Step 2: Encrypt for all recipients
            var encryptedRequest = CreateEncryptedCreditNotes(creditNoteJson, matchResult.Recipients);

            // Step 3: Submit encrypted credit notes
            var response = await SubmitEncryptedCreditNotesAsync(encryptedRequest, accessToken, cancellationToken);

            if (!response.Success)
            {
                return CreditNoteResult.Failure(response.Message ?? "Credit note submission failed", response.ErrorCode);
            }

            return CreditNoteResult.Success(response.CreditNoteId ?? "", response.RecipientCount);
        }
        catch (MatchingException ex)
        {
            return CreditNoteResult.Failure($"Customer matching failed: {ex.Message}");
        }
        catch (EncryptionException ex)
        {
            return CreditNoteResult.Failure($"Encryption failed: {ex.Message}");
        }
        catch (CheqiApiException ex)
        {
            return CreditNoteResult.Failure($"API error: {ex.Message}", ex.ErrorCode);
        }
    }
}
