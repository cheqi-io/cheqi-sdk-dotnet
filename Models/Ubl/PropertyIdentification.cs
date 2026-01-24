using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Property Identification - identification of a property.
/// </summary>
[XmlType(Namespace = UblNamespaces.Cac)]
public sealed class PropertyIdentification
{
    /// <summary>
    /// Property identifier.
    /// </summary>
    [JsonPropertyName("id")]
    [XmlElement("ID", Namespace = UblNamespaces.Cbc)]
    public Identifier? Id { get; set; }

    /// <summary>
    /// Issuer scope identifier.
    /// </summary>
    [JsonPropertyName("issuerScopeId")]
    [XmlElement("IssuerScopeID", Namespace = UblNamespaces.Cbc)]
    public Identifier? IssuerScopeId { get; set; }

    /// <summary>
    /// Issuer party.
    /// </summary>
    [JsonPropertyName("issuerParty")]
    [XmlElement("IssuerParty", Namespace = UblNamespaces.Cac)]
    public Party? IssuerParty { get; set; }
}
