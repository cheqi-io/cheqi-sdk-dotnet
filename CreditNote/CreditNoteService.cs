using System.Text.Json;
using Cheqi.Sdk.Encryption;
using Cheqi.Sdk.Http;
using Cheqi.Sdk.Matching;
using Cheqi.Sdk.Models.Generated;
using Cheqi.Sdk.Receipt;
using Cheqi.Sdk.Serialization;

namespace Cheqi.Sdk.CreditNote;

/// <summary>Issues caller-supplied definitive credit-note values through the separate encrypted endpoint.</summary>
public sealed class CreditNoteService
{
    private readonly ICheqiApiClient _api;
    private readonly EncryptionService _encryption;
    private readonly MatchingService _matching;
    public CreditNoteService(ICheqiApiClient apiClient, EncryptionService encryptionService, MatchingService matchingService)
    {
        _api = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
        _encryption = encryptionService ?? throw new ArgumentNullException(nameof(encryptionService));
        _matching = matchingService ?? throw new ArgumentNullException(nameof(matchingService));
    }

    /// <summary>Accepts a serializable object or an already serialized JSON string; values are not recalculated.</summary>
    public async Task<CreditNoteResult> IssueCreditNoteAsync(IdentificationDetails identificationDetails, string parentCheqiReceiptId,
        object creditNotePayload, Guid? storeId = null, string? accessToken = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(identificationDetails);
        ArgumentNullException.ThrowIfNull(creditNotePayload);
        ReceiptService.RequireText(parentCheqiReceiptId, nameof(parentCheqiReceiptId));
        ReceiptService.ValidateToken(accessToken);
        var match = accessToken == null
            ? await _matching.MatchCustomerAsync(identificationDetails, cancellationToken).ConfigureAwait(false)
            : await _matching.MatchCustomerAsync(identificationDetails, accessToken, cancellationToken).ConfigureAwait(false);
        if (match.RouteFound != true || match.Recipients == null || match.Recipients.Count == 0)
            throw new ArgumentException("Credit notes require a digital owner-device route");
        ReceiptService.RequireText(match.MatchId, "matchId");
        var plaintext = creditNotePayload as string ?? JsonSerializer.Serialize(creditNotePayload, CheqiJson.CreateOptions());
        var envelope = new EncryptedCreditNoteEnvelope(match.MatchId!, parentCheqiReceiptId,
            match.Recipients.Select(recipient => _encryption.EncryptCreditNoteForRecipient(plaintext, recipient)).ToList());
        if (storeId.HasValue) envelope.StoreId = storeId;
        var response = await _api.SubmitEncryptedCreditNoteAsync(envelope, accessToken, cancellationToken).ConfigureAwait(false);
        return CreditNoteResult.Accepted(parentCheqiReceiptId, response);
    }
}
