using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Address - a full structured address.
/// </summary>
[XmlType(Namespace = UblNamespaces.Cac)]
public sealed class Address
{
    /// <summary>
    /// Address identifier.
    /// </summary>
    [JsonPropertyName("id")]
    [XmlElement("ID", Namespace = UblNamespaces.Cbc)]
    public Identifier? Id { get; set; }

    /// <summary>
    /// Address type code.
    /// </summary>
    [JsonPropertyName("addressTypeCode")]
    [XmlElement("AddressTypeCode", Namespace = UblNamespaces.Cbc)]
    public Code? AddressTypeCode { get; set; }

    /// <summary>
    /// Address format code.
    /// </summary>
    [JsonPropertyName("addressFormatCode")]
    [XmlElement("AddressFormatCode", Namespace = UblNamespaces.Cbc)]
    public Code? AddressFormatCode { get; set; }

    /// <summary>
    /// Post office box number.
    /// </summary>
    [JsonPropertyName("postbox")]
    [XmlElement("Postbox", Namespace = UblNamespaces.Cbc)]
    public string? Postbox { get; set; }

    /// <summary>
    /// Floor number.
    /// </summary>
    [JsonPropertyName("floor")]
    [XmlElement("Floor", Namespace = UblNamespaces.Cbc)]
    public string? Floor { get; set; }

    /// <summary>
    /// Room number.
    /// </summary>
    [JsonPropertyName("room")]
    [XmlElement("Room", Namespace = UblNamespaces.Cbc)]
    public string? Room { get; set; }

    /// <summary>
    /// Street name.
    /// </summary>
    [JsonPropertyName("streetName")]
    [XmlElement("StreetName", Namespace = UblNamespaces.Cbc)]
    public string? StreetName { get; set; }

    /// <summary>
    /// Additional street name.
    /// </summary>
    [JsonPropertyName("additionalStreetName")]
    [XmlElement("AdditionalStreetName", Namespace = UblNamespaces.Cbc)]
    public string? AdditionalStreetName { get; set; }

    /// <summary>
    /// Block name.
    /// </summary>
    [JsonPropertyName("blockName")]
    [XmlElement("BlockName", Namespace = UblNamespaces.Cbc)]
    public string? BlockName { get; set; }

    /// <summary>
    /// Building name.
    /// </summary>
    [JsonPropertyName("buildingName")]
    [XmlElement("BuildingName", Namespace = UblNamespaces.Cbc)]
    public string? BuildingName { get; set; }

    /// <summary>
    /// Building number.
    /// </summary>
    [JsonPropertyName("buildingNumber")]
    [XmlElement("BuildingNumber", Namespace = UblNamespaces.Cbc)]
    public string? BuildingNumber { get; set; }

    /// <summary>
    /// Address descriptions.
    /// </summary>
    [JsonPropertyName("description")]
    [XmlElement("Description", Namespace = UblNamespaces.Cbc)]
    public List<string>? Description { get; set; }

    /// <summary>
    /// In-house mail location.
    /// </summary>
    [JsonPropertyName("inhouseMail")]
    [XmlElement("InhouseMail", Namespace = UblNamespaces.Cbc)]
    public string? InhouseMail { get; set; }

    /// <summary>
    /// Department name.
    /// </summary>
    [JsonPropertyName("department")]
    [XmlElement("Department", Namespace = UblNamespaces.Cbc)]
    public string? Department { get; set; }

    /// <summary>
    /// Mark attention text.
    /// </summary>
    [JsonPropertyName("markAttention")]
    [XmlElement("MarkAttention", Namespace = UblNamespaces.Cbc)]
    public string? MarkAttention { get; set; }

    /// <summary>
    /// Mark care text.
    /// </summary>
    [JsonPropertyName("markCare")]
    [XmlElement("MarkCare", Namespace = UblNamespaces.Cbc)]
    public string? MarkCare { get; set; }

    /// <summary>
    /// Plot identification.
    /// </summary>
    [JsonPropertyName("plotIdentification")]
    [XmlElement("PlotIdentification", Namespace = UblNamespaces.Cbc)]
    public string? PlotIdentification { get; set; }

    /// <summary>
    /// City subdivision name.
    /// </summary>
    [JsonPropertyName("citySubdivisionName")]
    [XmlElement("CitySubdivisionName", Namespace = UblNamespaces.Cbc)]
    public string? CitySubdivisionName { get; set; }

    /// <summary>
    /// City name.
    /// </summary>
    [JsonPropertyName("cityName")]
    [XmlElement("CityName", Namespace = UblNamespaces.Cbc)]
    public string? CityName { get; set; }

    /// <summary>
    /// Postal zone (postal code).
    /// </summary>
    [JsonPropertyName("postalZone")]
    [XmlElement("PostalZone", Namespace = UblNamespaces.Cbc)]
    public string? PostalZone { get; set; }

    /// <summary>
    /// Country subentity (state/province).
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
    /// Region name.
    /// </summary>
    [JsonPropertyName("region")]
    [XmlElement("Region", Namespace = UblNamespaces.Cbc)]
    public string? Region { get; set; }

    /// <summary>
    /// District name.
    /// </summary>
    [JsonPropertyName("district")]
    [XmlElement("District", Namespace = UblNamespaces.Cbc)]
    public string? District { get; set; }

    /// <summary>
    /// Timezone offset.
    /// </summary>
    [JsonPropertyName("timezoneOffset")]
    [XmlElement("TimezoneOffset", Namespace = UblNamespaces.Cbc)]
    public string? TimezoneOffset { get; set; }

    /// <summary>
    /// Address lines.
    /// </summary>
    [JsonPropertyName("addressLine")]
    [XmlElement("AddressLine", Namespace = UblNamespaces.Cac)]
    public List<AddressLine>? AddressLine { get; set; }

    /// <summary>
    /// Country.
    /// </summary>
    [JsonPropertyName("country")]
    [XmlElement("Country", Namespace = UblNamespaces.Cac)]
    public Country? Country { get; set; }

    /// <summary>
    /// Location coordinates.
    /// </summary>
    [JsonPropertyName("locationCoordinate")]
    [XmlElement("LocationCoordinate", Namespace = UblNamespaces.Cac)]
    public LocationCoordinate? LocationCoordinate { get; set; }
}
