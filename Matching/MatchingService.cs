using Cheqi.Sdk.Http;
using Cheqi.Sdk.Models.Generated;
using Microsoft.Extensions.Logging;

namespace Cheqi.Sdk.Matching;

/// <summary>Resolves identifiers or anonymous payment context to a device or explicit fallback route.</summary>
public sealed class MatchingService
{
    private readonly ICheqiApiClient _api;
    private readonly ILogger<MatchingService>? _logger;
    public MatchingService(ICheqiApiClient apiClient, ILogger<MatchingService>? logger = null)
    {
        _api = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
        _logger = logger;
    }
    public async Task<RecipientResolutionResponse> MatchCustomerAsync(IdentificationDetails identificationDetails,
        RequestOptions? options = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(identificationDetails);
        if (identificationDetails.PaymentType == null
            && string.IsNullOrWhiteSpace(identificationDetails.PairingCode)
            && string.IsNullOrWhiteSpace(identificationDetails.CheqiReceiptId)
            && string.IsNullOrWhiteSpace(identificationDetails.RecipientEmail)
            && string.IsNullOrWhiteSpace(identificationDetails.CardDetails?.PaymentAccountReference)
            && string.IsNullOrWhiteSpace(identificationDetails.PaymentAccountDetails?.Identifier))
            throw new ArgumentException("IdentificationDetails must contain an identifier or payment type", nameof(identificationDetails));
        var match = await _api.MatchCustomerAsync(identificationDetails, options, cancellationToken).ConfigureAwait(false);
        _logger?.LogDebug("Recipient route found: {RouteFound}", match.RouteFound);
        return match;
    }
}
