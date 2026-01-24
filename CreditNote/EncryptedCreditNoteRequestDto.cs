using System.Text.Json.Serialization;

namespace Cheqi.Sdk.CreditNote;

/// <summary>
/// Request DTO for submitting encrypted credit notes.
/// </summary>
public sealed class EncryptedCreditNoteRequestDto
{
    /// <summary>
    /// List of encrypted credit notes for each recipient.
    /// </summary>
    [JsonPropertyName("encryptedCreditNotes")]
    public List<EncryptedCreditNoteData> EncryptedCreditNotes { get; set; } = new();
}

/// <summary>
/// Encrypted credit note data for a single recipient.
/// </summary>
public sealed class EncryptedCreditNoteData
{
    /// <summary>
    /// Recipient ID.
    /// </summary>
    [JsonPropertyName("recipientId")]
    public string RecipientId { get; set; } = string.Empty;

    /// <summary>
    /// Base64-encoded encrypted credit note data.
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
    /// Hash of the original credit note data for integrity verification.
    /// </summary>
    [JsonPropertyName("dataHash")]
    public string? DataHash { get; set; }
}
