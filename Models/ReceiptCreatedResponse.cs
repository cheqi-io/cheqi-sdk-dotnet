using System.Text.Json.Serialization;

namespace Cheqi.Sdk.Models;

/// <summary>
/// Response from the receipt creation API.
/// </summary>
public class ReceiptCreatedResponse
{
    /// <summary>
    /// Unique Cheqi receipt ID assigned by the system.
    /// </summary>
    [JsonPropertyName("cheqiReceiptId")]
    public string? cheqiReceiptId { get; set; }

    /// <summary>
    /// Timestamp when the receipt was created.
    /// </summary>
    [JsonPropertyName("createdAt")]
    public DateTimeOffset? createdAt { get; set; }

    /// <summary>
    /// Hash of the receipt template used.
    /// </summary>
    [JsonPropertyName("templateHash")]
    public string? templateHash { get; set; }

    /// <summary>
    /// Default constructor.
    /// </summary>
    public ReceiptCreatedResponse()
    {
    }

    /// <summary>
    /// Constructor with all fields.
    /// </summary>
    public ReceiptCreatedResponse(string cheqiReceiptId, DateTimeOffset createdAt, string templateHash)
    {
        this.cheqiReceiptId = cheqiReceiptId;
        this.createdAt = createdAt;
        this.templateHash = templateHash;
    }
}
