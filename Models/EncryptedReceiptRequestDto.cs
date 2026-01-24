using System.Text.Json.Serialization;

namespace Cheqi.Sdk.Models;

/// <summary>
/// Request DTO for submitting encrypted receipts.
/// </summary>
public sealed class EncryptedReceiptRequestDto
{
    /// <summary>
    /// List of encrypted receipts for each recipient.
    /// </summary>
    [JsonPropertyName("encryptedReceipts")]
    public List<EncryptedReceiptData> EncryptedReceipts { get; set; } = new();
}

/// <summary>
/// Encrypted receipt data for a single recipient.
/// </summary>
public sealed class EncryptedReceiptData
{
    /// <summary>
    /// Recipient ID.
    /// </summary>
    [JsonPropertyName("recipientId")]
    public string RecipientId { get; set; } = string.Empty;

    /// <summary>
    /// Base64-encoded encrypted receipt data.
    /// </summary>
    [JsonPropertyName("encryptedData")]
    public string EncryptedData { get; set; } = string.Empty;

    /// <summary>
    /// Base64-encoded initialization vector.
    /// </summary>
    [JsonPropertyName("iv")]
    public string Iv { get; set; } = string.Empty;

    /// <summary>
    /// Base64-encoded authentication tag.
    /// </summary>
    [JsonPropertyName("authTag")]
    public string AuthTag { get; set; } = string.Empty;

    /// <summary>
    /// Base64-encoded encrypted AES key (encrypted with recipient's public key).
    /// </summary>
    [JsonPropertyName("encryptedKey")]
    public string EncryptedKey { get; set; } = string.Empty;

    /// <summary>
    /// Hash of the original receipt data for integrity verification.
    /// </summary>
    [JsonPropertyName("dataHash")]
    public string? DataHash { get; set; }
}
