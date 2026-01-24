using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Lot Identification - identification of a lot/batch.
/// </summary>
[XmlType(Namespace = UblNamespaces.Cac)]
public sealed class LotIdentification
{
    /// <summary>
    /// Lot number identifier.
    /// </summary>
    [JsonPropertyName("lotNumberId")]
    [XmlElement("LotNumberID", Namespace = UblNamespaces.Cbc)]
    public Identifier? LotNumberId { get; set; }

    /// <summary>
    /// Expiry date.
    /// </summary>
    [JsonPropertyName("expiryDate")]
    [XmlElement("ExpiryDate", Namespace = UblNamespaces.Cbc)]
    public DateTimeOffset? ExpiryDate { get; set; }

    /// <summary>
    /// Additional item properties.
    /// </summary>
    [JsonPropertyName("additionalItemProperty")]
    [XmlElement("AdditionalItemProperty", Namespace = UblNamespaces.Cac)]
    public List<ItemProperty>? AdditionalItemProperty { get; set; }
}
