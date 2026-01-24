using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Branch - a branch of a financial institution.
/// </summary>
[XmlType(Namespace = UblNamespaces.Cac)]
public sealed class Branch
{
    /// <summary>
    /// Branch identifier.
    /// </summary>
    [JsonPropertyName("id")]
    [XmlElement("ID", Namespace = UblNamespaces.Cbc)]
    public Identifier? Id { get; set; }

    /// <summary>
    /// Branch name.
    /// </summary>
    [JsonPropertyName("name")]
    [XmlElement("Name", Namespace = UblNamespaces.Cbc)]
    public Name? Name { get; set; }

    /// <summary>
    /// Financial institution.
    /// </summary>
    [JsonPropertyName("financialInstitution")]
    [XmlElement("FinancialInstitution", Namespace = UblNamespaces.Cac)]
    public FinancialInstitution? FinancialInstitution { get; set; }

    /// <summary>
    /// Branch address.
    /// </summary>
    [JsonPropertyName("address")]
    [XmlElement("Address", Namespace = UblNamespaces.Cac)]
    public Address? Address { get; set; }
}
