using Cheqi.Sdk.Exceptions;
using Cheqi.Sdk.Http;
using Cheqi.Sdk.Models;
using Microsoft.Extensions.Logging;

namespace Cheqi.Sdk.Services;

/// <summary>
/// Service for matching customers based on payment identifiers.
/// </summary>
public sealed class MatchingService
{
    private readonly ICheqiApiClient _apiClient;
    private readonly ILogger? _logger;

    public MatchingService(ICheqiApiClient apiClient, ILogger? logger = null)
    {
        _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
        _logger = logger;
    }

    /// <summary>
    /// Matches a customer based on the provided identification details.
    /// </summary>
    /// <param name="identificationDetails">The identification details for matching.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The recipient resolution response.</returns>
    public Task<RecipientResolutionResponse> MatchCustomerAsync(
        IdentificationDetails identificationDetails,
        CancellationToken cancellationToken = default)
    {
        return MatchCustomerAsync(identificationDetails, null, cancellationToken);
    }

    /// <summary>
    /// Matches a customer based on the provided identification details with an explicit access token.
    /// </summary>
    /// <param name="identificationDetails">The identification details for matching.</param>
    /// <param name="accessToken">OAuth access token for authentication.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The recipient resolution response.</returns>
    public async Task<RecipientResolutionResponse> MatchCustomerAsync(
        IdentificationDetails identificationDetails,
        string? accessToken,
        CancellationToken cancellationToken = default)
    {
        if (identificationDetails == null)
            throw new ArgumentNullException(nameof(identificationDetails));

        if (!identificationDetails.HasIdentifiers)
            throw new MatchingException("At least one identifier must be provided");

        ValidateIdentifiers(identificationDetails);

        _logger?.LogDebug("Matching customer with identifiers");

        try
        {
            var response = await _apiClient.ResolveRecipientsAsync(identificationDetails, accessToken, cancellationToken);

            _logger?.LogDebug(
                "Customer matching completed: found={Found}, recipients={Count}",
                response.Found,
                response.Recipients.Count);

            return response;
        }
        catch (CheqiApiException ex)
        {
            _logger?.LogError(ex, "Customer matching failed with API error");
            throw new MatchingException($"Customer matching failed: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Matches a customer by card details.
    /// </summary>
    /// <param name="cardDetails">The card details for matching.</param>
    /// <param name="accessToken">Optional access token.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The recipient resolution response.</returns>
    public Task<RecipientResolutionResponse> MatchByCardAsync(
        CardDetails cardDetails,
        string? accessToken = null,
        CancellationToken cancellationToken = default)
    {
        var identificationDetails = new IdentificationDetails { CardDetails = cardDetails };
        return MatchCustomerAsync(identificationDetails, accessToken, cancellationToken);
    }

    /// <summary>
    /// Matches a customer by payment account details (IBAN).
    /// </summary>
    /// <param name="paymentAccountDetails">The payment account details for matching.</param>
    /// <param name="accessToken">Optional access token.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The recipient resolution response.</returns>
    public Task<RecipientResolutionResponse> MatchByPaymentAccountAsync(
        PaymentAccountDetails paymentAccountDetails,
        string? accessToken = null,
        CancellationToken cancellationToken = default)
    {
        var identificationDetails = new IdentificationDetails { PaymentAccountDetails = paymentAccountDetails };
        return MatchCustomerAsync(identificationDetails, accessToken, cancellationToken);
    }

    /// <summary>
    /// Matches a customer by email address.
    /// </summary>
    /// <param name="email">The email address for matching.</param>
    /// <param name="accessToken">Optional access token.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The recipient resolution response.</returns>
    public Task<RecipientResolutionResponse> MatchByEmailAsync(
        string email,
        string? accessToken = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(email))
            throw new ArgumentException("Email cannot be null or empty", nameof(email));

        var identificationDetails = new IdentificationDetails { Email = email };
        return MatchCustomerAsync(identificationDetails, accessToken, cancellationToken);
    }

    /// <summary>
    /// Matches a customer by Cheqi Receipt ID.
    /// </summary>
    /// <param name="cheqiReceiptId">The Cheqi Receipt ID for matching.</param>
    /// <param name="accessToken">Optional access token.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The recipient resolution response.</returns>
    public Task<RecipientResolutionResponse> MatchByCheqiReceiptIdAsync(
        string cheqiReceiptId,
        string? accessToken = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(cheqiReceiptId))
            throw new ArgumentException("Cheqi Receipt ID cannot be null or empty", nameof(cheqiReceiptId));

        var identificationDetails = new IdentificationDetails { CheqiReceiptId = cheqiReceiptId };
        return MatchCustomerAsync(identificationDetails, accessToken, cancellationToken);
    }

    private void ValidateIdentifiers(IdentificationDetails details)
    {
        if (details.CardDetails != null)
        {
            var card = details.CardDetails;
            if (string.IsNullOrEmpty(card.Pan) && string.IsNullOrEmpty(card.Par) && string.IsNullOrEmpty(card.CardHash))
            {
                throw new MatchingException("Card details must include PAN, PAR, or card hash");
            }
        }

        if (details.PaymentAccountDetails != null)
        {
            var account = details.PaymentAccountDetails;
            if (string.IsNullOrEmpty(account.Iban) && string.IsNullOrEmpty(account.AccountHash))
            {
                throw new MatchingException("Payment account details must include IBAN or account hash");
            }
        }

        if (!string.IsNullOrEmpty(details.Email))
        {
            if (!details.Email.Contains('@'))
            {
                throw new MatchingException("Invalid email address format");
            }
        }
    }
}
