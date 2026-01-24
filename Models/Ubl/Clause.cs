using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Clause - a contractual clause.
/// </summary>
[XmlType(Namespace = UblNamespaces.Cac)]
public sealed class Clause
{
    /// <summary>
    /// Clause identifier.
    /// </summary>
    [JsonPropertyName("id")]
    [XmlElement("ID", Namespace = UblNamespaces.Cbc)]
    public Identifier? Id { get; set; }

    /// <summary>
    /// Clause content.
    /// </summary>
    [JsonPropertyName("content")]
    [XmlElement("Content", Namespace = UblNamespaces.Cbc)]
    public List<string>? Content { get; set; }
}
