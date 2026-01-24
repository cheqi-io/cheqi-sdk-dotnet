using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Party Identification - party identification.
/// </summary>
[XmlType(Namespace = UblNamespaces.Cac)]
public sealed class PartyIdentification
{
    /// <summary>
    /// Party identifier.
    /// </summary>
    [JsonPropertyName("id")]
    [XmlElement("ID", Namespace = UblNamespaces.Cbc)]
    public Identifier? Id { get; set; }
}
