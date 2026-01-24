using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Item Property Group - a group of item properties.
/// </summary>
[XmlType(Namespace = UblNamespaces.Cac)]
public sealed class ItemPropertyGroup
{
    /// <summary>
    /// Group identifier.
    /// </summary>
    [JsonPropertyName("id")]
    [XmlElement("ID", Namespace = UblNamespaces.Cbc)]
    public string? Id { get; set; }

    /// <summary>
    /// Group name.
    /// </summary>
    [JsonPropertyName("name")]
    [XmlElement("Name", Namespace = UblNamespaces.Cbc)]
    public string? Name { get; set; }

    /// <summary>
    /// Importance code.
    /// </summary>
    [JsonPropertyName("importanceCode")]
    [XmlElement("ImportanceCode", Namespace = UblNamespaces.Cbc)]
    public string? ImportanceCode { get; set; }
}
