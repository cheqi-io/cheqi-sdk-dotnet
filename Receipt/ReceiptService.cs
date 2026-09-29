using System.Text.Json;
using System.Text.Json.Nodes;
using Cheqi.Sdk.Download;
using Cheqi.Sdk.Encryption;
using Cheqi.Sdk.Exceptions;
using Cheqi.Sdk.Http;
using Cheqi.Sdk.Http.Exceptions;
using Cheqi.Sdk.Matching;
using Cheqi.Sdk.Models.Generated;
using Cheqi.Sdk.Serialization;
using Cheqi.Sdk.Verification;
using Route = Cheqi.Sdk.Models.Generated.RecipientResolutionResponse.DeliveryRouteTypeEnum;

namespace Cheqi.Sdk.Receipt;

/// <summary>Encrypts definitive caller-supplied values without calculating or enriching receipt amounts.</summary>
public sealed class ReceiptService
{
    private readonly ICheqiApiClient _api;
    private readonly EncryptionService _encryption;
    private readonly MatchingService _matching;
    private readonly DownloadService _downloads;
    private readonly string? _downloadBaseUrl;
    private readonly JsonSerializerOptions _json = CheqiJson.CreateOptions();

    public ReceiptService(ICheqiApiClient apiClient, EncryptionService encryptionService, MatchingService matchingService)
        : this(apiClient, encryptionService, matchingService, new DownloadService(), DownloadService.ProductionBaseUrl) { }

    public ReceiptService(ICheqiApiClient apiClient, EncryptionService encryptionService, MatchingService matchingService,
        DownloadService downloadService, string? downloadBaseUrl)
    {
        _api = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
        _encryption = encryptionService ?? throw new ArgumentNullException(nameof(encryptionService));
        _matching = matchingService ?? throw new ArgumentNullException(nameof(matchingService));
        _downloads = downloadService ?? throw new ArgumentNullException(nameof(downloadService));
        if (downloadBaseUrl != null) DownloadService.ValidateBaseUrl(downloadBaseUrl);
        _downloadBaseUrl = downloadBaseUrl;
    }

    public async Task<ReceiptResult> IssueReceiptAsync(IdentificationDetails identificationDetails, ReceiptPayload receiptPayload,
        Guid? storeId = null, string? accessToken = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(identificationDetails);
        ValidatePayload(receiptPayload);
        ValidateToken(accessToken);
        cancellationToken.ThrowIfCancellationRequested();
        RecipientResolutionResponse match;
        try
        {
            match = accessToken == null
                ? await _matching.MatchCustomerAsync(identificationDetails, cancellationToken).ConfigureAwait(false)
                : await _matching.MatchCustomerAsync(identificationDetails, accessToken, cancellationToken).ConfigureAwait(false);
        }
        catch (CheqiApiException exception) when (IsRecoverable(exception))
        { return ReceiptResult.DownloadPrepared(PrepareDownload(receiptPayload)); }
        ValidateRoute(match);
        switch (ResolveRoute(match))
        {
            case Route.DOWNLOADFALLBACK:
                if (identificationDetails.PaymentType == null)
                    return new ReceiptResult { State = ReceiptIssueState.DownloadFallback, MatchId = match.MatchId,
                        DeliveryRouteType = Route.DOWNLOADFALLBACK, ExpiresAt = match.ExpiresAt, DownloadEnvelopeRequired = true };
                return await DeliverDownloadAsync(match.MatchId, receiptPayload, identificationDetails, accessToken, cancellationToken).ConfigureAwait(false);
            case Route.EMAILFALLBACK:
                return new ReceiptResult { State = ReceiptIssueState.EmailFallback, MatchId = match.MatchId,
                    DeliveryRouteType = Route.EMAILFALLBACK, ExpiresAt = match.ExpiresAt, EmailReceiptRequired = true };
        }
        var envelope = GenerateEncryptedReceiptEnvelope(match, receiptPayload, storeId);
        try { return ReceiptResult.Accepted(await SubmitPayloadAsync(envelope, accessToken, cancellationToken).ConfigureAwait(false)); }
        catch (CheqiApiException exception) when (exception.HttpStatusCode == 410)
        { return ReceiptResult.Expired(match.MatchId!); }
        catch (CheqiApiException exception) when (IsAmbiguous(exception))
        { return ReceiptResult.Pending(match.MatchId!, PrepareDownload(receiptPayload), RetryAfter(exception)); }
    }

    /// <summary>Retrieves only the original opaque match ID; never rematches the customer.</summary>
    public Task<MatchStatusResponse> GetMatchAsync(string matchId, string? accessToken = null, CancellationToken cancellationToken = default)
    {
        RequireText(matchId, nameof(matchId));
        ValidateToken(accessToken);
        return _api.GetMatchAsync(matchId, accessToken, cancellationToken);
    }

    /// <summary>One recovery attempt. Retain the original persisted fallback URL and ciphertext.</summary>
    public async Task<ReceiptResult> ResumeReceiptAsync(string matchId, ReceiptPayload receiptPayload, Guid? storeId = null,
        string? accessToken = null, CancellationToken cancellationToken = default)
    {
        RequireText(matchId, nameof(matchId));
        ValidateToken(accessToken);
        ValidatePayload(receiptPayload);
        try
        {
            var match = await _api.GetMatchAsync(matchId, accessToken, cancellationToken).ConfigureAwait(false);
            if (match == null || match.State == null || match.MatchId != matchId)
                throw new CheqiSdkException("Invalid match status response");
            switch (match.State)
            {
                case MatchState.SUBMITTED:
                    ValidateSubmission(match.Submission, matchId);
                    return ReceiptResult.Accepted(match.Submission!);
                case MatchState.INPROGRESS:
                    return ReceiptResult.Pending(matchId, null, match.RetryAfterSeconds);
                case MatchState.EXPIRED:
                    return ReceiptResult.Expired(matchId);
                case MatchState.MATCHED:
                    return ReceiptResult.Accepted(await SubmitPayloadAsync(
                        GenerateEncryptedReceiptEnvelope(match, receiptPayload, storeId), accessToken, cancellationToken).ConfigureAwait(false));
                default: throw new CheqiSdkException("Unknown match state");
            }
        }
        catch (CheqiApiException exception) when (exception.HttpStatusCode == 410) { return ReceiptResult.Expired(matchId); }
        catch (CheqiApiException exception) when (IsAmbiguous(exception)) { return ReceiptResult.Pending(matchId, null, RetryAfter(exception)); }
    }

    public EncryptedReceiptEnvelope GenerateEncryptedReceiptEnvelope(RecipientResolutionResponse match, ReceiptPayload receiptPayload, Guid? storeId = null)
    {
        ValidateRoute(match);
        if (ResolveRoute(match) != Route.DIGITAL) throw new ArgumentException("Match must have a digital owner-device route", nameof(match));
        return GenerateEnvelope(match.MatchId!, match.Recipients, receiptPayload, storeId);
    }

    public EncryptedReceiptEnvelope GenerateEncryptedReceiptEnvelope(MatchStatusResponse match, ReceiptPayload receiptPayload, Guid? storeId = null)
    {
        ArgumentNullException.ThrowIfNull(match);
        if (match.State != MatchState.MATCHED || match.Route != MatchStatusResponse.RouteEnum.DIGITAL)
            throw new ArgumentException("Match must be MATCHED with a digital owner-device route", nameof(match));
        return GenerateEnvelope(match.MatchId!, match.Recipients, receiptPayload, storeId);
    }

    private EncryptedReceiptEnvelope GenerateEnvelope(string matchId, List<MatchedRecipient>? recipients, ReceiptPayload payload, Guid? storeId)
    {
        RequireText(matchId, nameof(matchId));
        ValidatePayload(payload);
        if (recipients == null || recipients.Count == 0 || recipients.Any(recipient => recipient == null))
            throw new ArgumentException("Match must contain owner devices", nameof(recipients));
        var envelope = new EncryptedReceiptEnvelope(matchId, _encryption.EncryptReceiptForRecipients(JsonSerializer.Serialize(payload, _json), recipients));
        if (storeId.HasValue) envelope.StoreId = storeId;
        return envelope;
    }

    public Task<ReceiptSubmissionResponse> SubmitEncryptedReceiptAsync(EncryptedReceiptEnvelope envelope, string? accessToken = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(envelope);
        ValidateToken(accessToken);
        return _api.SubmitEncryptedReceiptAsync(envelope, accessToken, cancellationToken);
    }

    private async Task<ReceiptSubmissionResponse> SubmitPayloadAsync(EncryptedReceiptEnvelope envelope, string? accessToken, CancellationToken cancellationToken)
    {
        var response = await SubmitEncryptedReceiptAsync(envelope, accessToken, cancellationToken).ConfigureAwait(false);
        ValidateSubmission(response, envelope.MatchId);
        return response;
    }

    public Task<ClientReceiptDownloadResponse> UploadPreparedDownloadAsync(PreparedReceiptDownload download, string? accessToken = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(download);
        ValidateToken(accessToken);
        return _api.UploadEncryptedDownloadReceiptAsync(download.UploadRequest, accessToken, cancellationToken);
    }

    public Task<ReceiptResult> IssueDownloadReceiptAsync(IdentificationDetails identificationDetails, ReceiptPayload receiptPayload,
        string? accessToken = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(identificationDetails);
        ValidatePayload(receiptPayload);
        ValidateToken(accessToken);
        if (identificationDetails.PaymentType == null) throw new ArgumentException("PaymentType is required", nameof(identificationDetails));
        cancellationToken.ThrowIfCancellationRequested();
        return DeliverDownloadAsync(null, receiptPayload, identificationDetails, accessToken, cancellationToken);
    }

    public async Task<ReceiptResult> CompleteDownloadFallbackAsync(ReceiptResult fallbackResult, ReceiptEnvelope receiptEnvelope,
        string? templateHash = null, string? accessToken = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(fallbackResult);
        ArgumentNullException.ThrowIfNull(receiptEnvelope);
        ValidateToken(accessToken);
        if (!fallbackResult.DownloadEnvelopeRequired || fallbackResult.DeliveryRouteType != Route.DOWNLOADFALLBACK)
            throw new ArgumentException("Result does not require a download envelope", nameof(fallbackResult));
        cancellationToken.ThrowIfCancellationRequested();
        var link = _downloads.GenerateDownloadLink(RequireDownloadBaseUrl());
        var request = new ClientReceiptDownloadRequest(link.DownloadId, _downloads.EncryptDownloadEnvelope(receiptEnvelope, link.ContentKey));
        if (templateHash != null) request.TemplateHash = templateHash;
        var response = await _api.UploadEncryptedDownloadReceiptAsync(request, accessToken, cancellationToken).ConfigureAwait(false);
        return ReceiptResult.DownloadAccepted(fallbackResult.MatchId, response, link.Url);
    }

    public Task SendReceiptViaEmailAsync(string customerEmail, CheqiReceipt receipt, string? accessToken = null,
        CancellationToken cancellationToken = default) => _api.SendReceiptViaEmailAsync(customerEmail, receipt, accessToken, cancellationToken);

    private async Task<ReceiptResult> DeliverDownloadAsync(string? matchId, ReceiptPayload payload, IdentificationDetails identification,
        string? accessToken, CancellationToken cancellationToken)
    {
        var prepared = PrepareDownload(payload, identification);
        try
        {
            var response = await UploadPreparedDownloadAsync(prepared, accessToken, cancellationToken).ConfigureAwait(false);
            return ReceiptResult.DownloadAccepted(matchId, response, prepared.DownloadUrl);
        }
        catch (CheqiApiException exception) when (IsRecoverable(exception)) { return ReceiptResult.DownloadPrepared(prepared); }
    }

    private PreparedReceiptDownload PrepareDownload(ReceiptPayload payload, IdentificationDetails? identification = null)
    {
        var link = _downloads.GenerateDownloadLink(RequireDownloadBaseUrl());
        var document = JsonSerializer.SerializeToNode(payload, _json)!.AsObject();
        if (identification != null) document["identificationDetails"] = JsonSerializer.SerializeToNode(identification, _json);
        var plaintext = document.ToJsonString(_json);
        var envelope = new ReceiptEnvelope(link.DownloadId,
            new Dictionary<string, ReceiptEnvelopeDocument> { ["CHEQI"] = new(plaintext, ReceiptEnvelopeDocument.MediaTypeEnum.ApplicationJson) },
            1, "receipt-payload-v1", Guid.NewGuid());
        var request = new ClientReceiptDownloadRequest(link.DownloadId, _downloads.EncryptDownloadEnvelope(envelope, link.ContentKey))
        { TemplateHash = new VerificationService().CalculateCheqiReceiptHash(plaintext) };
        return new PreparedReceiptDownload(link.Url, request);
    }

    private string RequireDownloadBaseUrl() => _downloadBaseUrl ?? throw new InvalidOperationException("Configure ReceiptDownloadBaseUrl for a custom API endpoint to prepare downloads");
    internal static bool IsRecoverable(CheqiApiException exception) => exception.Retryable || exception.HttpStatusCode == 0 &&
        (exception.ErrorCode is CheqiApiException.ErrorCodes.NETWORK_ERROR or CheqiApiException.ErrorCodes.TIMEOUT_ERROR || exception.InnerException is HttpRequestException);
    private static bool IsAmbiguous(CheqiApiException exception) => IsRecoverable(exception) ||
        exception.ErrorCode == CheqiApiException.ErrorCodes.INVALID_RESPONSE && (exception.HttpStatusCode == 0 || exception.HttpStatusCode is >= 200 and < 300);
    private static int? RetryAfter(CheqiApiException exception) => (exception as SubmissionInProgressException)?.RetryAfterSeconds;
    private static void ValidateSubmission(ReceiptSubmissionResponse? response, string matchId)
    {
        if (response == null || string.IsNullOrWhiteSpace(response.CheqiReceiptId) || response.MatchId != matchId)
            throw new CheqiApiException("Invalid receipt submission response", 0, CheqiApiException.ErrorCodes.INVALID_RESPONSE, null);
    }
    private static void ValidateRoute(RecipientResolutionResponse match)
    {
        ArgumentNullException.ThrowIfNull(match);
        if (match.RouteFound != true) throw new CheqiSdkException("No Cheqi owner-device route was found", CheqiSdkException.ErrorCodes.CUSTOMER_NOT_FOUND, 404, null);
        RequireText(match.MatchId, "matchId");
    }
    private static Route ResolveRoute(RecipientResolutionResponse match) => match.DeliveryRouteType ??
        (match.Recipients?.Count > 0 ? Route.DIGITAL : throw new CheqiSdkException("Resolution did not include deliveryRouteType"));
    internal static void RequireText(string? value, string name)
    { if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException($"{name} is required", name); }
    internal static void ValidateToken(string? token)
    { if (token != null && string.IsNullOrWhiteSpace(token)) throw new ArgumentException("accessToken cannot be empty", nameof(token)); }
    private static void ValidatePayload(ReceiptPayload payload)
    {
        ArgumentNullException.ThrowIfNull(payload);
        RequireText(payload.DocumentNumber, "documentNumber");
        RequireText(payload.Currency, "currency");
        if (payload.Products == null || payload.Products.Count == 0 || payload.Products.Any(product => product == null))
            throw new ArgumentException("Receipt must contain products", nameof(payload));
    }
}
