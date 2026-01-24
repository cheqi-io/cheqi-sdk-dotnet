using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Purchase Reference - a reference to a purchase.
/// </summary>
[XmlType(Namespace = UblNamespaces.Cac)]
public sealed class PurchaseReference
{
    /// <summary>
    /// Purchase identifier.
    /// </summary>
    [JsonPropertyName("id")]
    [XmlElement("ID", Namespace = UblNamespaces.Cbc)]
    public Identifier? Id { get; set; }

    /// <summary>
    /// Description.
    /// </summary>
    [JsonPropertyName("description")]
    [XmlElement("Description", Namespace = UblNamespaces.Cbc)]
    public string? Description { get; set; }
}
