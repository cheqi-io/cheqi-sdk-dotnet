using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Item Instance - a specific instance of an item.
/// </summary>
[XmlType(Namespace = UblNamespaces.Cac)]
public sealed class ItemInstance
{
    /// <summary>
    /// Product trace identifier.
    /// </summary>
    [JsonPropertyName("productTraceId")]
    [XmlElement("ProductTraceID", Namespace = UblNamespaces.Cbc)]
    public Identifier? ProductTraceId { get; set; }

    /// <summary>
    /// Manufacture date.
    /// </summary>
    [JsonPropertyName("manufactureDate")]
    [XmlElement("ManufactureDate", Namespace = UblNamespaces.Cbc)]
    public DateTimeOffset? ManufactureDate { get; set; }

    /// <summary>
    /// Manufacture time.
    /// </summary>
    [JsonPropertyName("manufactureTime")]
    [XmlElement("ManufactureTime", Namespace = UblNamespaces.Cbc)]
    public TimeSpan? ManufactureTime { get; set; }

    /// <summary>
    /// Best before date.
    /// </summary>
    [JsonPropertyName("bestBeforeDate")]
    [XmlElement("BestBeforeDate", Namespace = UblNamespaces.Cbc)]
    public DateTimeOffset? BestBeforeDate { get; set; }

    /// <summary>
    /// Registration identifier.
    /// </summary>
    [JsonPropertyName("registrationId")]
    [XmlElement("RegistrationID", Namespace = UblNamespaces.Cbc)]
    public Identifier? RegistrationId { get; set; }

    /// <summary>
    /// Serial identifier.
    /// </summary>
    [JsonPropertyName("serialId")]
    [XmlElement("SerialID", Namespace = UblNamespaces.Cbc)]
    public Identifier? SerialId { get; set; }

    /// <summary>
    /// Additional item properties.
    /// </summary>
    [JsonPropertyName("additionalItemProperty")]
    [XmlElement("AdditionalItemProperty", Namespace = UblNamespaces.Cac)]
    public List<ItemProperty>? AdditionalItemProperty { get; set; }

    /// <summary>
    /// Lot identification.
    /// </summary>
    [JsonPropertyName("lotIdentification")]
    [XmlElement("LotIdentification", Namespace = UblNamespaces.Cac)]
    public LotIdentification? LotIdentification { get; set; }
}
