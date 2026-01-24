using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL External Reference - a reference to an external resource.
/// </summary>
[XmlType(Namespace = UblNamespaces.Cac)]
public sealed class ExternalReference
{
    /// <summary>
    /// URI of the external reference.
    /// </summary>
    [JsonPropertyName("uri")]
    [XmlElement("URI", Namespace = UblNamespaces.Cbc)]
    public string? Uri { get; set; }
}
