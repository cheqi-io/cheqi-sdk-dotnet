using System.Text.Json.Serialization;

namespace Cheqi.Sdk.Models;

/// <summary>
/// Response from the receipt creation API.
/// </summary>
public sealed class ReceiptCreatedResponse
{
    /// <summary>
    /// Whether the receipt was created successfully.
    /// </summary>
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    /// <summary>
    /// Unique receipt ID assigned by Cheqi.
    /// </summary>
    [JsonPropertyName("receiptId")]
    public string? ReceiptId { get; set; }

    /// <summary>
    /// Number of recipients the receipt was delivered to.
    /// </summary>
    [JsonPropertyName("recipientCount")]
    public int RecipientCount { get; set; }

    /// <summary>
    /// Message providing additional context.
    /// </summary>
    [JsonPropertyName("message")]
    public string? Message { get; set; }

    /// <summary>
    /// Error code if the request failed.
    /// </summary>
    [JsonPropertyName("errorCode")]
    public string? ErrorCode { get; set; }

    /// <summary>
    /// Timestamp when the receipt was processed.
    /// </summary>
    [JsonPropertyName("timestamp")]
    public DateTimeOffset? Timestamp { get; set; }
}
