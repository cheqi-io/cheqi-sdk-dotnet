using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Item Property Range - a range for an item property value.
/// </summary>
[XmlType(Namespace = UblNamespaces.Cac)]
public sealed class ItemPropertyRange
{
    /// <summary>
    /// Minimum value.
    /// </summary>
    [JsonPropertyName("minimumValue")]
    [XmlElement("MinimumValue", Namespace = UblNamespaces.Cbc)]
    public string? MinimumValue { get; set; }

    /// <summary>
    /// Maximum value.
    /// </summary>
    [JsonPropertyName("maximumValue")]
    [XmlElement("MaximumValue", Namespace = UblNamespaces.Cbc)]
    public string? MaximumValue { get; set; }
}
