using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Location - a physical or logical location.
/// </summary>
[XmlType(Namespace = UblNamespaces.Cac)]
public sealed class Location
{
    /// <summary>
    /// Location identifier.
    /// </summary>
    [JsonPropertyName("id")]
    [XmlElement("ID", Namespace = UblNamespaces.Cbc)]
    public Identifier? Id { get; set; }

    /// <summary>
    /// Location descriptions.
    /// </summary>
    [JsonPropertyName("description")]
    [XmlElement("Description", Namespace = UblNamespaces.Cbc)]
    public List<string>? Description { get; set; }

    /// <summary>
    /// Location conditions.
    /// </summary>
    [JsonPropertyName("conditions")]
    [XmlElement("Conditions", Namespace = UblNamespaces.Cbc)]
    public List<string>? Conditions { get; set; }

    /// <summary>
    /// Country subentity.
    /// </summary>
    [JsonPropertyName("countrySubentity")]
    [XmlElement("CountrySubentity", Namespace = UblNamespaces.Cbc)]
    public string? CountrySubentity { get; set; }

    /// <summary>
    /// Country subentity code.
    /// </summary>
    [JsonPropertyName("countrySubentityCode")]
    [XmlElement("CountrySubentityCode", Namespace = UblNamespaces.Cbc)]
    public Code? CountrySubentityCode { get; set; }

    /// <summary>
    /// Location type code.
    /// </summary>
    [JsonPropertyName("locationTypeCode")]
    [XmlElement("LocationTypeCode", Namespace = UblNamespaces.Cbc)]
    public Code? LocationTypeCode { get; set; }

    /// <summary>
    /// Information URI.
    /// </summary>
    [JsonPropertyName("informationUri")]
    [XmlElement("InformationURI", Namespace = UblNamespaces.Cbc)]
    public Identifier? InformationUri { get; set; }

    /// <summary>
    /// Location name.
    /// </summary>
    [JsonPropertyName("name")]
    [XmlElement("Name", Namespace = UblNamespaces.Cbc)]
    public Name? Name { get; set; }

    /// <summary>
    /// Validity periods.
    /// </summary>
    [JsonPropertyName("validityPeriod")]
    [XmlElement("ValidityPeriod", Namespace = UblNamespaces.Cac)]
    public List<Period>? ValidityPeriod { get; set; }

    /// <summary>
    /// Address of the location.
    /// </summary>
    [JsonPropertyName("address")]
    [XmlElement("Address", Namespace = UblNamespaces.Cac)]
    public Address? Address { get; set; }

    /// <summary>
    /// Location coordinates.
    /// </summary>
    [JsonPropertyName("locationCoordinate")]
    [XmlElement("LocationCoordinate", Namespace = UblNamespaces.Cac)]
    public List<LocationCoordinate>? LocationCoordinate { get; set; }
}
