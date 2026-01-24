using System.Text.Json.Serialization;

namespace Cheqi.Sdk.Models;

/// <summary>
/// A recipient for encrypted receipts, containing their public key.
/// </summary>
public sealed class Recipient
{
    /// <summary>
    /// Unique identifier for the recipient.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// RSA public key in PEM format for encrypting receipts.
    /// </summary>
    [JsonPropertyName("publicKey")]
    public string PublicKey { get; set; } = string.Empty;

    /// <summary>
    /// Device or app identifier for delivery.
    /// </summary>
    [JsonPropertyName("deviceId")]
    public string? DeviceId { get; set; }

    /// <summary>
    /// Platform type (iOS, Android, Web).
    /// </summary>
    [JsonPropertyName("platform")]
    public string? Platform { get; set; }
}
