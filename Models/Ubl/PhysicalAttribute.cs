using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Physical Attribute - a physical attribute of an item.
/// </summary>
[XmlType(Namespace = UblNamespaces.Cac)]
public sealed class PhysicalAttribute
{
    /// <summary>
    /// Attribute identifier.
    /// </summary>
    [JsonPropertyName("attributeId")]
    [XmlElement("AttributeID", Namespace = UblNamespaces.Cbc)]
    public Identifier? AttributeId { get; set; }

    /// <summary>
    /// Position code.
    /// </summary>
    [JsonPropertyName("positionCode")]
    [XmlElement("PositionCode", Namespace = UblNamespaces.Cbc)]
    public Code? PositionCode { get; set; }

    /// <summary>
    /// Description code.
    /// </summary>
    [JsonPropertyName("descriptionCode")]
    [XmlElement("DescriptionCode", Namespace = UblNamespaces.Cbc)]
    public Code? DescriptionCode { get; set; }

    /// <summary>
    /// Descriptions.
    /// </summary>
    [JsonPropertyName("description")]
    [XmlElement("Description", Namespace = UblNamespaces.Cbc)]
    public List<string>? Description { get; set; }
}
