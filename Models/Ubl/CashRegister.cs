using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Cash Register - cash register information.
/// </summary>
[XmlType(Namespace = UblNamespaces.Cac)]
public sealed class CashRegister
{
    /// <summary>
    /// Cash register identifier.
    /// </summary>
    [JsonPropertyName("id")]
    [XmlElement("ID", Namespace = UblNamespaces.Cbc)]
    public Identifier? Id { get; set; }

    /// <summary>
    /// Serial number identifier.
    /// </summary>
    [JsonPropertyName("serialNumberId")]
    [XmlElement("SerialNumberID", Namespace = UblNamespaces.Cbc)]
    public Identifier? SerialNumberId { get; set; }
}
