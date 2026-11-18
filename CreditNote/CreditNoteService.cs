using System.Text.Json;
using System.Text.Json.Nodes;
using Cheqi.Sdk.Download;
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

    /// <summary>Resolve originatorDocumentReference and prepare encrypted delivery to the original recipient.</summary>
    public async Task<PreparedCreditNote> PrepareCreditNoteAsync(object payload, Guid? storeId = null,
        RequestOptions? options = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(payload);
        var json = CheqiJson.CreateOptions();
        var node = JsonNode.Parse(payload as string ?? JsonSerializer.Serialize(payload, json)) as JsonObject
            ?? throw new ArgumentException("Credit-note payload must be an object");
        var body = node["creditNoteTemplateRequest"] as JsonObject ?? node;
        if (body["originatorDocumentReference"] is not JsonValue referenceValue || !referenceValue.TryGetValue<string>(out var reference)
            || string.IsNullOrWhiteSpace(reference)) throw new ArgumentException("originatorDocumentReference must contain the original documentNumber");
        var original = await _api.ResolveCreditNoteReferenceAsync(new CreditNoteDocumentReference(DocumentReferences.Hash(reference)),
            options, cancellationToken).ConfigureAwait(false);
        ReceiptService.RequireText(original.ParentCheqiReceiptId, "resolved parentCheqiReceiptId");
        var parent = original.ParentCheqiReceiptId!;
        if (original.DeliveryRoute == "DIGITAL")
        {
            if (node["taxesApplied"] is not JsonValue taxes || !taxes.TryGetValue<bool>(out var taxesApplied))
                throw new ArgumentException("taxesApplied must be a boolean for device generation");
            var match = await _matching.MatchCustomerAsync(new IdentificationDetails { CheqiReceiptId = parent }, options, cancellationToken).ConfigureAwait(false);
            if (match.RouteFound != true || match.DeliveryRouteType != RecipientResolutionResponse.DeliveryRouteTypeEnum.DIGITAL
                || string.IsNullOrWhiteSpace(match.MatchId) || match.Recipients == null || match.Recipients.Count == 0)
                throw new ArgumentException("Original owner-device route is unavailable");
            var generation = node.ContainsKey("creditNoteTemplateRequest") ? node : new JsonObject {
                ["taxesApplied"] = taxesApplied, ["creditNoteTemplateRequest"] = body.DeepClone() };
            var deliveries = match.Recipients.Select(r => _encryption.EncryptForDevice(generation.ToJsonString(), r)).ToList();
            var submission = new EncryptedCreditNoteEnvelope(match.MatchId, parent, deliveries);
            if ((storeId ?? original.StoreId) is Guid store) submission.StoreId = store;
            return new PreparedCreditNote(parent) { DeviceSubmission = submission };
        }
        if (original.DeliveryRoute != "DOWNLOAD") throw new ArgumentException("Unsupported original document route");
        if (storeId.HasValue) throw new ArgumentException("Download delivery does not support store-scoped issuance");
        ReceiptService.RequireText(original.CreditNotePublicKey, "creditNotePublicKey");
        var downloads = new DownloadService();
        var child = downloads.GenerateDownloadLink("https://receipt.cheqi.io");
        var link = _encryption.EncryptForDevice(JsonSerializer.Serialize(new { version = 2, parentCheqiReceiptId = parent,
            downloadId = child.DownloadId, contentKey = child.ContentKey }), new MatchedRecipient { Id = parent, PublicKey = original.CreditNotePublicKey });
        var envelope = new ReceiptEnvelope(child.DownloadId, new Dictionary<string, ReceiptEnvelopeDocument> {
            ["CREDIT_NOTE_PAYLOAD"] = new(body.ToJsonString(), ReceiptEnvelopeDocument.MediaTypeEnum.ApplicationJson)
        }, 2, "credit-note-payload-v1", Guid.NewGuid());
        var request = new ClientReceiptDownloadRequest(child.DownloadId, downloads.EncryptDownloadEnvelope(envelope, child.ContentKey)) {
            ParentCheqiReceiptId = parent, EncryptedParentLink = link.EncryptedContent, EncryptedParentLinkKey = link.EncryptedAesKey };
        return new PreparedCreditNote(parent) { DownloadSubmission = request };
    }

    /// <summary>Submit the persisted ciphertext without resolving or encrypting again.</summary>
    public async Task<CreditNoteResult> SubmitPreparedCreditNoteAsync(PreparedCreditNote prepared,
        RequestOptions? options = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(prepared);
        if ((prepared.DeviceSubmission == null) == (prepared.DownloadSubmission == null))
            throw new ArgumentException("Exactly one prepared submission is required");
        if (string.IsNullOrWhiteSpace(prepared.ParentCheqiReceiptId) || prepared.ParentCheqiReceiptId !=
            (prepared.DeviceSubmission?.ParentCheqiReceiptId ?? prepared.DownloadSubmission?.ParentCheqiReceiptId))
            throw new ArgumentException("Credit-note parent mismatch");
        if (prepared.DeviceSubmission is { } device)
        {
            var response = await _api.SubmitEncryptedCreditNoteAsync(device, options, cancellationToken).ConfigureAwait(false);
            if (string.IsNullOrWhiteSpace(response?.CheqiReceiptId) || response.MatchId != device.MatchId)
                throw new InvalidOperationException("Credit-note acceptance is not confirmed");
            return CreditNoteResult.Accepted(prepared.ParentCheqiReceiptId, response);
        }
        var accepted = await _api.UploadEncryptedDownloadReceiptAsync(prepared.DownloadSubmission!, options, cancellationToken).ConfigureAwait(false);
        if (string.IsNullOrWhiteSpace(accepted?.CheqiReceiptId)) throw new InvalidOperationException("Credit-note acceptance is not confirmed");
        return new CreditNoteResult { CheqiReceiptId = accepted.CheqiReceiptId, ParentCheqiReceiptId = prepared.ParentCheqiReceiptId,
            CreatedAt = accepted.CreatedAt, Status = ReceiptSubmissionResponse.StatusEnum.COMPLETED };
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
