using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Address Line - a free-form address line.
/// </summary>
[XmlType(Namespace = UblNamespaces.Cac)]
public sealed class AddressLine
{
    /// <summary>
    /// The address line text.
    /// </summary>
    [JsonPropertyName("line")]
    [XmlElement("Line", Namespace = UblNamespaces.Cbc)]
    public string? Line { get; set; }
}
