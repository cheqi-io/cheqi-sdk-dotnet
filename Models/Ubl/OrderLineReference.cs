using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Order Line Reference - a reference to an order line.
/// </summary>
[XmlType(Namespace = UblNamespaces.Cac)]
public sealed class OrderLineReference
{
    /// <summary>
    /// Line identifier.
    /// </summary>
    [JsonPropertyName("lineId")]
    [XmlElement("LineID", Namespace = UblNamespaces.Cbc)]
    public string? LineId { get; set; }
}
