using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Price List - a price list with validity periods.
/// </summary>
[XmlType(Namespace = UblNamespaces.Cac)]
public sealed class PriceList
{
    /// <summary>
    /// Price list identifier.
    /// </summary>
    [JsonPropertyName("id")]
    [XmlElement("ID", Namespace = UblNamespaces.Cbc)]
    public Identifier? Id { get; set; }

    /// <summary>
    /// Status code.
    /// </summary>
    [JsonPropertyName("statusCode")]
    [XmlElement("StatusCode", Namespace = UblNamespaces.Cbc)]
    public Code? StatusCode { get; set; }

    /// <summary>
    /// Validity periods.
    /// </summary>
    [JsonPropertyName("validityPeriod")]
    [XmlElement("ValidityPeriod", Namespace = UblNamespaces.Cac)]
    public List<Period>? ValidityPeriod { get; set; }
}
