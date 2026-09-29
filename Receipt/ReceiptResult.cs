using Cheqi.Sdk.Models.Generated;
using Route = Cheqi.Sdk.Models.Generated.RecipientResolutionResponse.DeliveryRouteTypeEnum;

namespace Cheqi.Sdk.Receipt;

/// <summary>An accepted submission, explicit fallback, or an uncertain digital submission to recover.</summary>
public sealed class ReceiptResult
{
    public ReceiptIssueState State { get; init; }
    public string? CheqiReceiptId { get; init; }
    public string? MatchId { get; init; }
    public Route DeliveryRouteType { get; init; }
    public ReceiptSubmissionResponse.StatusEnum? Status { get; init; }
    public DateTimeOffset? CreatedAt { get; init; }
    public DateTimeOffset? ExpiresAt { get; init; }
    public string? DownloadUrl { get; init; }
    public bool DownloadEnvelopeRequired { get; init; }
    public bool EmailReceiptRequired { get; init; }
    public ReceiptSubmissionResponse? Submission { get; init; }
    public PreparedReceiptDownload? PreparedDownload { get; init; }
    public bool Retryable { get; init; }
    public int? RetryAfterSeconds { get; init; }
    public bool IsAccepted => !string.IsNullOrWhiteSpace(CheqiReceiptId);
    public bool IsSuccess => IsAccepted;

    public static ReceiptResult Accepted(ReceiptSubmissionResponse response) => new()
    {
        State = ReceiptIssueState.DigitalSubmitted, DeliveryRouteType = Route.DIGITAL,
        CheqiReceiptId = response.CheqiReceiptId, MatchId = response.MatchId,
        Status = response.Status, CreatedAt = response.CreatedAt, Submission = response
    };
    internal static ReceiptResult Pending(string matchId, PreparedReceiptDownload? download, int? retryAfter) => new()
    {
        State = ReceiptIssueState.DigitalPending, MatchId = matchId, DeliveryRouteType = Route.DIGITAL,
        PreparedDownload = download, DownloadUrl = download?.DownloadUrl, Retryable = true, RetryAfterSeconds = retryAfter
    };
    internal static ReceiptResult Expired(string matchId) => new()
    { State = ReceiptIssueState.DigitalExpired, MatchId = matchId, DeliveryRouteType = Route.DIGITAL };
    internal static ReceiptResult DownloadPrepared(PreparedReceiptDownload download) => new()
    {
        State = ReceiptIssueState.DownloadFallback, DeliveryRouteType = Route.DOWNLOADFALLBACK,
        PreparedDownload = download, DownloadUrl = download.DownloadUrl, Retryable = true
    };
    internal static ReceiptResult DownloadAccepted(string? matchId, ClientReceiptDownloadResponse response, string url) => new()
    {
        State = ReceiptIssueState.DownloadFallback, DeliveryRouteType = Route.DOWNLOADFALLBACK,
        MatchId = matchId, CheqiReceiptId = response.CheqiReceiptId,
        CreatedAt = response.CreatedAt, ExpiresAt = response.ExpiresAt, DownloadUrl = url
    };
    public override string ToString() => $"ReceiptResult {{ State = {State}, IsAccepted = {IsAccepted}, DownloadUrl = <redacted> }}";
}
