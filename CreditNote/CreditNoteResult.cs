using Cheqi.Sdk.Models.Enums;

namespace Cheqi.Sdk.CreditNote;

/// <summary>
/// Result of a complete credit note processing operation.
/// </summary>
public sealed class CreditNoteResult
{
    /// <summary>
    /// Whether the credit note was processed successfully.
    /// </summary>
    public bool IsSuccess { get; init; }

    /// <summary>
    /// Unique credit note ID assigned by Cheqi.
    /// </summary>
    public string? CreditNoteId { get; init; }

    /// <summary>
    /// Number of recipients the credit note was delivered to.
    /// </summary>
    public int DeliveryCount { get; init; }

    /// <summary>
    /// Delivery status of the credit note.
    /// </summary>
    public DeliveryStatus Status { get; init; }

    /// <summary>
    /// Error message if processing failed.
    /// </summary>
    public string? ErrorMessage { get; init; }

    /// <summary>
    /// Error code if processing failed.
    /// </summary>
    public string? ErrorCode { get; init; }

    /// <summary>
    /// Whether recipients were found for the identifiers.
    /// </summary>
    public bool RecipientsFound { get; init; }

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    public static CreditNoteResult Success(string creditNoteId, int deliveryCount) => new()
    {
        IsSuccess = true,
        CreditNoteId = creditNoteId,
        DeliveryCount = deliveryCount,
        Status = DeliveryStatus.DELIVERED_DIGITAL,
        RecipientsFound = true
    };

    /// <summary>
    /// Creates a result indicating no recipients were found.
    /// </summary>
    public static CreditNoteResult NoRecipients() => new()
    {
        IsSuccess = false,
        Status = DeliveryStatus.CUSTOMER_NOT_FOUND,
        RecipientsFound = false,
        ErrorMessage = "No recipients found for the provided identification details"
    };

    /// <summary>
    /// Creates a failed result.
    /// </summary>
    public static CreditNoteResult Failure(string errorMessage, string? errorCode = null) => new()
    {
        IsSuccess = false,
        Status = DeliveryStatus.FAILED,
        ErrorMessage = errorMessage,
        ErrorCode = errorCode
    };
}
