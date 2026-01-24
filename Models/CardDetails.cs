using System.Text.Json.Serialization;
using Cheqi.Sdk.Models.Enums;

namespace Cheqi.Sdk.Models;

/// <summary>
/// Details of a payment card for customer identification.
/// </summary>
public sealed class CardDetails
{
    /// <summary>
    /// Primary Account Number (PAN) - masked or full.
    /// </summary>
    [JsonPropertyName("pan")]
    public string? Pan { get; set; }

    /// <summary>
    /// Payment Account Reference (PAR) - tokenized card identifier.
    /// </summary>
    [JsonPropertyName("par")]
    public string? Par { get; set; }

    /// <summary>
    /// Card network provider.
    /// </summary>
    [JsonPropertyName("cardProvider")]
    public CardProvider? CardProvider { get; set; }

    /// <summary>
    /// Last four digits of the card.
    /// </summary>
    [JsonPropertyName("lastFourDigits")]
    public string? LastFourDigits { get; set; }

    /// <summary>
    /// Card expiration month (1-12).
    /// </summary>
    [JsonPropertyName("expirationMonth")]
    public int? ExpirationMonth { get; set; }

    /// <summary>
    /// Card expiration year.
    /// </summary>
    [JsonPropertyName("expirationYear")]
    public int? ExpirationYear { get; set; }

    /// <summary>
    /// Hash of the card number for matching.
    /// </summary>
    [JsonPropertyName("cardHash")]
    public string? CardHash { get; set; }
}
