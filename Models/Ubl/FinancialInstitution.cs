using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Financial Institution - a financial institution (bank).
/// </summary>
[XmlType(Namespace = UblNamespaces.Cac)]
public sealed class FinancialInstitution
{
    /// <summary>
    /// Institution identifier.
    /// </summary>
    [JsonPropertyName("id")]
    [XmlElement("ID", Namespace = UblNamespaces.Cbc)]
    public Identifier? Id { get; set; }

    /// <summary>
    /// Institution name.
    /// </summary>
    [JsonPropertyName("name")]
    [XmlElement("Name", Namespace = UblNamespaces.Cbc)]
    public Name? Name { get; set; }

    /// <summary>
    /// Institution address.
    /// </summary>
    [JsonPropertyName("address")]
    [XmlElement("Address", Namespace = UblNamespaces.Cac)]
    public Address? Address { get; set; }
}
