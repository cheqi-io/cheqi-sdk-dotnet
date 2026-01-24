using System.Text.Json.Serialization;

namespace Cheqi.Sdk.CreditNote;

/// <summary>
/// Response from the credit note creation API.
/// </summary>
public sealed class CreditNoteCreatedResponse
{
    /// <summary>
    /// Whether the credit note was created successfully.
    /// </summary>
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    /// <summary>
    /// Unique credit note ID assigned by Cheqi.
    /// </summary>
    [JsonPropertyName("creditNoteId")]
    public string? CreditNoteId { get; set; }

    /// <summary>
    /// Number of recipients the credit note was delivered to.
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
    /// Timestamp when the credit note was processed.
    /// </summary>
    [JsonPropertyName("timestamp")]
    public DateTimeOffset? Timestamp { get; set; }
}
