using Cheqi.Sdk.Models.Generated;

namespace Cheqi.Sdk.CreditNote;

public sealed class CreditNoteResult
{
    public string? CheqiReceiptId { get; init; }
    public string? ParentCheqiReceiptId { get; init; }
    public string? MatchId { get; init; }
    public ReceiptSubmissionResponse.StatusEnum? Status { get; init; }
    public DateTimeOffset? CreatedAt { get; init; }
    public bool IsAccepted => !string.IsNullOrWhiteSpace(CheqiReceiptId);
    public bool IsSuccess => IsAccepted;
    public static CreditNoteResult Accepted(string parentCheqiReceiptId, ReceiptSubmissionResponse response)
    {
        ArgumentNullException.ThrowIfNull(response);
        return new CreditNoteResult { CheqiReceiptId = response.CheqiReceiptId, ParentCheqiReceiptId = parentCheqiReceiptId,
            MatchId = response.MatchId, Status = response.Status, CreatedAt = response.CreatedAt };
    }
}
