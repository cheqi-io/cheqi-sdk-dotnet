using Cheqi.Sdk.Encryption;
using Cheqi.Sdk.Exceptions;
using Cheqi.Sdk.Http;
using Cheqi.Sdk.Models;
using Microsoft.Extensions.Logging;

namespace Cheqi.Sdk.Services;

/// <summary>
/// Service for processing and delivering encrypted receipts.
/// </summary>
public sealed class ReceiptService
{
    private readonly ICheqiApiClient _apiClient;
    private readonly EncryptionService _encryptionService;
    private readonly MatchingService _matchingService;
    private readonly ILogger? _logger;

    public ReceiptService(
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
    /// Generates a receipt template from the request.
    /// </summary>
    /// <param name="request">The receipt template request.</param>
    /// <param name="accessToken">Optional access token.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The generated receipt as JSON.</returns>
    public Task<string> GenerateReceiptTemplateAsync(
        ReceiptTemplateRequest request,
        string? accessToken = null,
        CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        return _apiClient.GenerateReceiptTemplateAsync(request, accessToken, cancellationToken);
    }

    /// <summary>
    /// Creates encrypted receipts for the specified recipients.
    /// </summary>
    /// <param name="receiptJson">The receipt as JSON.</param>
    /// <param name="recipients">The list of recipients.</param>
    /// <returns>The encrypted receipts request DTO.</returns>
    public EncryptedReceiptRequestDto CreateEncryptedReceipts(string receiptJson, IEnumerable<Recipient> recipients)
    {
        if (string.IsNullOrEmpty(receiptJson))
            throw new ArgumentException("Receipt JSON cannot be null or empty", nameof(receiptJson));
        if (recipients == null)
            throw new ArgumentNullException(nameof(recipients));

        var encryptedData = _encryptionService.EncryptReceiptForRecipients(receiptJson, recipients);

        return new EncryptedReceiptRequestDto
        {
            EncryptedReceipts = encryptedData
        };
    }

    /// <summary>
    /// Submits encrypted receipts to the API.
    /// </summary>
    /// <param name="request">The encrypted receipts request.</param>
    /// <param name="accessToken">Optional access token.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The receipt creation response.</returns>
    public Task<ReceiptCreatedResponse> SubmitEncryptedReceiptsAsync(
        EncryptedReceiptRequestDto request,
        string? accessToken = null,
        CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        return _apiClient.SubmitEncryptedReceiptsAsync(request, accessToken, cancellationToken);
    }

    /// <summary>
    /// Processes a complete receipt: generates template, matches customer, encrypts, and delivers.
    /// </summary>
    /// <param name="identificationDetails">Customer identification details.</param>
    /// <param name="receiptRequest">The receipt template request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The result of the receipt processing.</returns>
    public Task<ReceiptResult> ProcessCompleteReceiptAsync(
        IdentificationDetails identificationDetails,
        ReceiptTemplateRequest receiptRequest,
        CancellationToken cancellationToken = default)
    {
        return ProcessCompleteReceiptAsync(identificationDetails, receiptRequest, null, cancellationToken);
    }

    /// <summary>
    /// Processes a complete receipt with an explicit access token.
    /// </summary>
    /// <param name="identificationDetails">Customer identification details.</param>
    /// <param name="receiptRequest">The receipt template request.</param>
    /// <param name="accessToken">OAuth access token.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The result of the receipt processing.</returns>
    public async Task<ReceiptResult> ProcessCompleteReceiptAsync(
        IdentificationDetails identificationDetails,
        ReceiptTemplateRequest receiptRequest,
        string? accessToken,
        CancellationToken cancellationToken = default)
    {
        if (identificationDetails == null)
            throw new ArgumentNullException(nameof(identificationDetails));
        if (receiptRequest == null)
            throw new ArgumentNullException(nameof(receiptRequest));

        try
        {
            _logger?.LogDebug("Starting complete receipt processing for receipt {ReceiptId}", receiptRequest.ReceiptId);

            // Step 1: Match customer
            var matchResult = await _matchingService.MatchCustomerAsync(identificationDetails, accessToken, cancellationToken);

            if (!matchResult.HasRecipients)
            {
                _logger?.LogInformation("No recipients found for receipt {ReceiptId}", receiptRequest.ReceiptId);
                return ReceiptResult.NoRecipients();
            }

            _logger?.LogDebug("Found {Count} recipients for receipt {ReceiptId}", matchResult.Recipients.Count, receiptRequest.ReceiptId);

            // Step 2: Generate receipt template
            var receiptJson = await GenerateReceiptTemplateAsync(receiptRequest, accessToken, cancellationToken);

            // Step 3: Encrypt for all recipients
            var encryptedRequest = CreateEncryptedReceipts(receiptJson, matchResult.Recipients);

            // Step 4: Submit encrypted receipts
            var response = await SubmitEncryptedReceiptsAsync(encryptedRequest, accessToken, cancellationToken);

            if (!response.Success)
            {
                _logger?.LogWarning("Receipt submission failed for {ReceiptId}: {Message}", receiptRequest.ReceiptId, response.Message);
                return ReceiptResult.Failure(response.Message ?? "Receipt submission failed", response.ErrorCode);
            }

            _logger?.LogInformation(
                "Receipt {ReceiptId} processed successfully, delivered to {Count} recipients",
                receiptRequest.ReceiptId,
                response.RecipientCount);

            return ReceiptResult.Success(response.ReceiptId ?? receiptRequest.ReceiptId, response.RecipientCount);
        }
        catch (MatchingException ex)
        {
            _logger?.LogError(ex, "Customer matching failed for receipt {ReceiptId}", receiptRequest.ReceiptId);
            return ReceiptResult.Failure($"Customer matching failed: {ex.Message}");
        }
        catch (EncryptionException ex)
        {
            _logger?.LogError(ex, "Encryption failed for receipt {ReceiptId}", receiptRequest.ReceiptId);
            return ReceiptResult.Failure($"Encryption failed: {ex.Message}");
        }
        catch (CheqiApiException ex)
        {
            _logger?.LogError(ex, "API error processing receipt {ReceiptId}", receiptRequest.ReceiptId);
            return ReceiptResult.Failure($"API error: {ex.Message}", ex.ErrorCode);
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger?.LogError(ex, "Unexpected error processing receipt {ReceiptId}", receiptRequest.ReceiptId);
            throw new ReceiptProcessingException($"Failed to process receipt: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Processes a receipt using pre-generated receipt JSON.
    /// </summary>
    /// <param name="identificationDetails">Customer identification details.</param>
    /// <param name="receiptJson">The pre-generated receipt JSON.</param>
    /// <param name="accessToken">Optional access token.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The result of the receipt processing.</returns>
    public async Task<ReceiptResult> ProcessReceiptWithJsonAsync(
        IdentificationDetails identificationDetails,
        string receiptJson,
        string? accessToken = null,
        CancellationToken cancellationToken = default)
    {
        if (identificationDetails == null)
            throw new ArgumentNullException(nameof(identificationDetails));
        if (string.IsNullOrEmpty(receiptJson))
            throw new ArgumentException("Receipt JSON cannot be null or empty", nameof(receiptJson));

        try
        {
            // Step 1: Match customer
            var matchResult = await _matchingService.MatchCustomerAsync(identificationDetails, accessToken, cancellationToken);

            if (!matchResult.HasRecipients)
            {
                return ReceiptResult.NoRecipients();
            }

            // Step 2: Encrypt for all recipients
            var encryptedRequest = CreateEncryptedReceipts(receiptJson, matchResult.Recipients);

            // Step 3: Submit encrypted receipts
            var response = await SubmitEncryptedReceiptsAsync(encryptedRequest, accessToken, cancellationToken);

            if (!response.Success)
            {
                return ReceiptResult.Failure(response.Message ?? "Receipt submission failed", response.ErrorCode);
            }

            return ReceiptResult.Success(response.ReceiptId ?? "", response.RecipientCount);
        }
        catch (MatchingException ex)
        {
            return ReceiptResult.Failure($"Customer matching failed: {ex.Message}");
        }
        catch (EncryptionException ex)
        {
            return ReceiptResult.Failure($"Encryption failed: {ex.Message}");
        }
        catch (CheqiApiException ex)
        {
            return ReceiptResult.Failure($"API error: {ex.Message}", ex.ErrorCode);
        }
    }
}
