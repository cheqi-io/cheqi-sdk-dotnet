using System.Text.Json.Serialization;
using System.Xml.Serialization;
using Cheqi.Sdk.Utils;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Amount - a monetary amount with currency.
/// </summary>
[XmlType(Namespace = UblNamespaces.Cbc)]
public sealed class Amount
{
    /// <summary>
    /// The currency ID (e.g., "EUR", "USD").
    /// </summary>
    [JsonPropertyName("currencyID")]
    [XmlAttribute("currencyID")]
    public string? CurrencyId { get; set; }

    /// <summary>
    /// The amount value.
    /// </summary>
    [JsonPropertyName("value")]
    [JsonConverter(typeof(NullableDecimalJsonConverter))]
    [XmlText]
    public decimal? Value { get; set; }
}
