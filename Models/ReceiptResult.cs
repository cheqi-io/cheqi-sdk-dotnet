using Cheqi.Sdk.Models.Enums;

namespace Cheqi.Sdk.Models;

/// <summary>
/// Result of a complete receipt processing operation.
/// </summary>
public sealed class ReceiptResult
{
    /// <summary>
    /// Whether the receipt was processed successfully.
    /// </summary>
    public bool IsSuccess { get; init; }

    /// <summary>
    /// Unique receipt ID assigned by Cheqi.
    /// </summary>
    public string? ReceiptId { get; init; }

    /// <summary>
    /// Number of recipients the receipt was delivered to.
    /// </summary>
    public int ReceiptCount { get; init; }

    /// <summary>
    /// Delivery status of the receipt.
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
    public static ReceiptResult Success(string receiptId, int receiptCount) => new()
    {
        IsSuccess = true,
        ReceiptId = receiptId,
        ReceiptCount = receiptCount,
        Status = DeliveryStatus.DELIVERED,
        RecipientsFound = true
    };

    /// <summary>
    /// Creates a result indicating no recipients were found.
    /// </summary>
    public static ReceiptResult NoRecipients() => new()
    {
        IsSuccess = false,
        Status = DeliveryStatus.NO_RECIPIENT,
        RecipientsFound = false,
        ErrorMessage = "No recipients found for the provided identification details"
    };

    /// <summary>
    /// Creates a failed result.
    /// </summary>
    public static ReceiptResult Failure(string errorMessage, string? errorCode = null) => new()
    {
        IsSuccess = false,
        Status = DeliveryStatus.FAILED,
        ErrorMessage = errorMessage,
        ErrorCode = errorCode
    };
}
