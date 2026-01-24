using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Party Name - a party name.
/// </summary>
[XmlType(Namespace = UblNamespaces.Cac)]
public sealed class PartyName
{
    /// <summary>
    /// The party name.
    /// </summary>
    [JsonPropertyName("name")]
    [XmlElement("Name", Namespace = UblNamespaces.Cbc)]
    public string? Name { get; set; }
}
