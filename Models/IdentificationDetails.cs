using System.Text.Json.Serialization;

namespace Cheqi.Sdk.Models;

/// <summary>
/// Container for customer identification details used for matching.
/// </summary>
public sealed class IdentificationDetails
{
    /// <summary>
    /// Card payment details.
    /// </summary>
    [JsonPropertyName("cardDetails")]
    public CardDetails? CardDetails { get; set; }

    /// <summary>
    /// Payment account details (bank account).
    /// </summary>
    [JsonPropertyName("paymentAccountDetails")]
    public PaymentAccountDetails? PaymentAccountDetails { get; set; }

    /// <summary>
    /// Customer email address.
    /// </summary>
    [JsonPropertyName("email")]
    public string? Email { get; set; }

    /// <summary>
    /// Cheqi Receipt ID for direct delivery.
    /// </summary>
    [JsonPropertyName("cheqiReceiptId")]
    public string? CheqiReceiptId { get; set; }

    /// <summary>
    /// Phone number for identification.
    /// </summary>
    [JsonPropertyName("phoneNumber")]
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Returns true if any identification details are present.
    /// </summary>
    [JsonIgnore]
    public bool HasIdentifiers =>
        CardDetails != null ||
        PaymentAccountDetails != null ||
        !string.IsNullOrEmpty(Email) ||
        !string.IsNullOrEmpty(CheqiReceiptId) ||
        !string.IsNullOrEmpty(PhoneNumber);
}
