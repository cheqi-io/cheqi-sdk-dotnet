using System.Text.Json.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Price List - a price list with validity periods.
/// </summary>
public sealed class PriceList
{
    /// <summary>
    /// Price list identifier.
    /// </summary>
    [JsonPropertyName("id")]
    public Identifier? Id { get; set; }

    /// <summary>
    /// Status code.
    /// </summary>
    [JsonPropertyName("statusCode")]
    public Code? StatusCode { get; set; }

    /// <summary>
    /// Validity periods.
    /// </summary>
    [JsonPropertyName("validityPeriod")]
    public List<Period>? ValidityPeriod { get; set; }
}
