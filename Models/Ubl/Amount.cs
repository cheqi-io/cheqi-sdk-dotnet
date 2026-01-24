using System.Text.Json.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Amount - a monetary amount with currency.
/// </summary>
public sealed class Amount
{
    /// <summary>
    /// The currency ID (e.g., "EUR", "USD").
    /// </summary>
    [JsonPropertyName("currencyId")]
    public string? CurrencyId { get; set; }

    /// <summary>
    /// The amount value.
    /// </summary>
    [JsonPropertyName("value")]
    public decimal? Value { get; set; }
}
