using System.Text.Json;
using System.Text.Json.Nodes;
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

    /// <summary>Prepare an ordinary encrypted receipt link. The receiving wallet matches the invoice locally.</summary>
    public PreparedReceiptDownload PrepareDownload(object payload, string baseUrl = "https://receipt.cheqi.io")
    {
        ArgumentNullException.ThrowIfNull(payload);
        var node = JsonNode.Parse(payload as string ?? JsonSerializer.Serialize(payload, CheqiJson.CreateOptions())) as JsonObject
            ?? throw new ArgumentException("Credit-note payload must be an object");
        var body = node["creditNoteTemplateRequest"] as JsonObject ?? node;
        foreach (var field in new[] { "documentNumber", "originatorDocumentReference" })
            if (body[field] is not JsonValue text || !text.TryGetValue<string>(out var value) || string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"{field} is required");
        if (node["taxesApplied"] is not JsonValue taxes || !taxes.TryGetValue<bool>(out var taxesApplied))
            throw new ArgumentException("taxesApplied must be a boolean");
        var document = body.DeepClone().AsObject();
        document["taxesApplied"] = taxesApplied;
        var downloads = new Download.DownloadService();
        var link = downloads.GenerateDownloadLink(baseUrl);
        var envelope = new ReceiptEnvelope(link.DownloadId, new Dictionary<string, ReceiptEnvelopeDocument> {
            ["CREDIT_NOTE_PAYLOAD"] = new(document.ToJsonString(), ReceiptEnvelopeDocument.MediaTypeEnum.ApplicationJson)
        }, 2, "credit-note-payload-v1", Guid.NewGuid());
        return new PreparedReceiptDownload(link.Url, new ClientReceiptDownloadRequest(link.DownloadId,
            downloads.EncryptDownloadEnvelope(envelope, link.ContentKey)));
    }

    /// <summary>Accepts a serializable object or an already serialized JSON string; values are not recalculated.</summary>
    public async Task<CreditNoteResult> IssueCreditNoteAsync(IdentificationDetails identificationDetails, string parentCheqiReceiptId,
        object creditNotePayload, Guid? storeId = null, RequestOptions? options = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(identificationDetails);
        ArgumentNullException.ThrowIfNull(creditNotePayload);
        ReceiptService.RequireText(parentCheqiReceiptId, nameof(parentCheqiReceiptId));
        var match = await _matching.MatchCustomerAsync(identificationDetails, options, cancellationToken).ConfigureAwait(false);
        if (match.RouteFound != true || match.Recipients == null || match.Recipients.Count == 0)
            throw new ArgumentException("Credit notes require a digital owner-device route");
        ReceiptService.RequireText(match.MatchId, "matchId");
        var plaintext = creditNotePayload as string ?? JsonSerializer.Serialize(creditNotePayload, CheqiJson.CreateOptions());
        var envelope = new EncryptedCreditNoteEnvelope(match.MatchId!, parentCheqiReceiptId,
            match.Recipients.Select(recipient => _encryption.EncryptCreditNoteForRecipient(plaintext, recipient)).ToList());
        if (storeId.HasValue) envelope.StoreId = storeId;
        var response = await _api.SubmitEncryptedCreditNoteAsync(envelope, options, cancellationToken).ConfigureAwait(false);
        return CreditNoteResult.Accepted(parentCheqiReceiptId, response);
    }
}
