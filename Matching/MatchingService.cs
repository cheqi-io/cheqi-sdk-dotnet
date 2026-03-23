using Cheqi.Sdk.Http;
using Cheqi.Sdk.Http.Exceptions;
using Cheqi.Sdk.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Text;
using RecipientResolutionResponse = Cheqi.Sdk.Models.Generated.RecipientResolutionResponse;
using IdentificationDetails = Cheqi.Sdk.Models.Generated.IdentificationDetails;

namespace Cheqi.Sdk.Matching;

/// <summary>
/// Service for secure customer matching using payment details.
/// This service sends PaymentDetails to the backend for recipient resolution.
/// The backend determines the best matching strategy based on available identifiers:
/// - Card details (PAN, PAR)
/// - Payment account details (IBAN)
/// - Customer email
/// Thread-safe and designed for high-throughput matching operations.
/// </summary>
public class MatchingService
{
    private readonly ILogger<MatchingService> _logger;
    private readonly ICheqiApiClient _apiClient;

    public MatchingService(ICheqiApiClient apiClient, ILogger<MatchingService>? logger = null)
    {
        _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
        _logger = logger ?? NullLogger<MatchingService>.Instance;
    }

    /// <summary>
    /// Matches a customer using the provided payment details.
    /// 
    /// The backend determines the best matching strategy based on available identifiers:
    /// - Card details (PAN, PAR)
    /// - Payment account details (IBAN)
    /// - Customer email
    /// </summary>
    /// <param name="identificationDetails">Payment details containing customer identifiers</param>
    /// <param name="accessToken">OAuth access token for API calls</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>RecipientResolutionResponse with matched recipients or empty if not found</returns>
    /// <exception cref="CheqiApiException">if matching operation fails</exception>
    /// <exception cref="ArgumentException">if paymentDetails contains no valid identifiers</exception>
    public async Task<RecipientResolutionResponse> MatchCustomerAsync(
        IdentificationDetails identificationDetails,
        string accessToken,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Starting customer matching");

        // Validate request has at least one identifier
        if (!HasValidIdentifiers(identificationDetails))
        {
            string error = "IdentificationDetails must contain at least one identifier (card details, payment account details, or email)";
            _logger.LogError(error);
            throw new ArgumentException(error, nameof(identificationDetails));
        }

        LogAvailableIdentifiers(identificationDetails);

        try
        {
            var response = await _apiClient.MatchCustomerAsync(identificationDetails, accessToken, cancellationToken);

            if (response.CustomerFound == true)
            {
                _logger.LogInformation("Customer matched successfully: {Count} recipients found", response.Recipients?.Count ?? 0);
            }
            else
            {
                _logger.LogInformation("No customer match found");
            }

            return response;
        }
        catch (CheqiApiException e)
        {
            _logger.LogError(e, "Customer matching failed: {Message}", e.Message);
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unexpected error during customer matching: {Message}", e.Message);
            throw new CheqiApiException(
                "Customer matching failed: " + e.Message,
                e,
                0,
                CheqiApiException.ErrorCodes.UNKNOWN_ERROR,
                null
            );
        }
    }

    /// <summary>
    /// Matches a customer using API key from SDK config.
    /// Uses Bearer token authentication with the API key configured during SDK initialization.
    /// For companies accessing their own data directly.
    /// </summary>
    /// <param name="identificationDetails">Payment details containing customer identifiers</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>RecipientResolutionResponse with matched recipients or empty if not found</returns>
    /// <exception cref="CheqiApiException">if matching operation fails</exception>
    /// <exception cref="ArgumentException">if paymentDetails contains no valid identifiers</exception>
    public async Task<RecipientResolutionResponse> MatchCustomerAsync(
        IdentificationDetails identificationDetails,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Starting customer matching with API key");

        // Validate request has at least one identifier
        if (!HasValidIdentifiers(identificationDetails))
        {
            string error = "Identification details must contain at least one identifier (card details, payment account details, or email)";
            _logger.LogError(error);
            throw new ArgumentException(error, nameof(identificationDetails));
        }

        LogAvailableIdentifiers(identificationDetails);

        try
        {
            var response = await _apiClient.MatchCustomerAsync(identificationDetails, null, cancellationToken);

            if (response.CustomerFound == true)
            {
                _logger.LogInformation("Customer matched successfully: {Count} recipients found", response.Recipients?.Count ?? 0);
            }
            else
            {
                _logger.LogInformation("No customer match found");
            }

            return response;
        }
        catch (CheqiApiException e)
        {
            _logger.LogError(e, "Customer matching failed: {Message}", e.Message);
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unexpected error during customer matching: {Message}", e.Message);
            throw new CheqiApiException(
                "Customer matching failed: " + e.Message,
                e,
                0,
                CheqiApiException.ErrorCodes.UNKNOWN_ERROR,
                null
            );
        }
    }

    /// <summary>
    /// Validates that the payment details contain at least one identifier.
    /// </summary>
    private bool HasValidIdentifiers(IdentificationDetails identificationDetails)
    {
        if (identificationDetails == null)
        {
            return false;
        }

        return HasCardDetails(identificationDetails) ||
               HasPaymentAccountDetails(identificationDetails) ||
               HasCheqiReceiptId(identificationDetails) ||
               HasPairingCode(identificationDetails) ||
               HasEmail(identificationDetails);
    }

    /// <summary>
    /// Checks if card details (PAN or PAR) are present.
    /// </summary>
    private bool HasCardDetails(IdentificationDetails identificationDetails)
    {
        if (identificationDetails.CardDetails == null)
        {
            return false;
        }

        var card = identificationDetails.CardDetails;
        return (!string.IsNullOrWhiteSpace(card.PaymentAccountNumber)) ||
               (!string.IsNullOrWhiteSpace(card.PaymentAccountReference));
    }

    /// <summary>
    /// Checks if Cheqi receipt ID is present.
    /// </summary>
    private bool HasCheqiReceiptId(IdentificationDetails identificationDetails)
    {
        return !string.IsNullOrWhiteSpace(identificationDetails.CheqiReceiptId);
    }

    /// <summary>
    /// Checks if pairing code is present.
    /// </summary>
    private bool HasPairingCode(IdentificationDetails identificationDetails)
    {
        return !string.IsNullOrWhiteSpace(identificationDetails.PairingCode);
    }

    /// <summary>
    /// Checks if payment account details (IBAN) are present.
    /// </summary>
    private bool HasPaymentAccountDetails(IdentificationDetails identificationDetails)
    {
        if (identificationDetails.PaymentAccountDetails == null)
        {
            return false;
        }

        var details = identificationDetails.PaymentAccountDetails;
        return !string.IsNullOrWhiteSpace(details.Identifier);
    }

    /// <summary>
    /// Checks if customer email is present.
    /// </summary>
    private bool HasEmail(IdentificationDetails identificationDetails)
    {
        return !string.IsNullOrWhiteSpace(identificationDetails.RecipientEmail);
    }

    /// <summary>
    /// Logs which identifiers are available for matching.
    /// </summary>
    private void LogAvailableIdentifiers(IdentificationDetails identificationDetails)
    {
        var identifiers = new StringBuilder("Available identifiers: ");

        if (HasCardDetails(identificationDetails))
        {
            identifiers.Append("[Card] ");
        }
        if (HasPaymentAccountDetails(identificationDetails))
        {
            identifiers.Append("[PaymentAccount] ");
        }
        if (HasEmail(identificationDetails))
        {
            identifiers.Append("[Email] ");
        }
        if (HasCheqiReceiptId(identificationDetails))
        {
            identifiers.Append("[CheqiReceiptId] ");
        }
        if (HasPairingCode(identificationDetails))
        {
            identifiers.Append("[PairingCode] ");
        }

        _logger.LogDebug(identifiers.ToString().Trim());
    }
}
