using System.Text.Json.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Address - a full structured address.
/// </summary>
public sealed class Address
{
    /// <summary>
    /// Address identifier.
    /// </summary>
    [JsonPropertyName("id")]
    public Identifier? Id { get; set; }

    /// <summary>
    /// Address type code.
    /// </summary>
    [JsonPropertyName("addressTypeCode")]
    public Code? AddressTypeCode { get; set; }

    /// <summary>
    /// Address format code.
    /// </summary>
    [JsonPropertyName("addressFormatCode")]
    public Code? AddressFormatCode { get; set; }

    /// <summary>
    /// Post office box number.
    /// </summary>
    [JsonPropertyName("postbox")]
    public string? Postbox { get; set; }

    /// <summary>
    /// Floor number.
    /// </summary>
    [JsonPropertyName("floor")]
    public string? Floor { get; set; }

    /// <summary>
    /// Room number.
    /// </summary>
    [JsonPropertyName("room")]
    public string? Room { get; set; }

    /// <summary>
    /// Street name.
    /// </summary>
    [JsonPropertyName("streetName")]
    public string? StreetName { get; set; }

    /// <summary>
    /// Additional street name.
    /// </summary>
    [JsonPropertyName("additionalStreetName")]
    public string? AdditionalStreetName { get; set; }

    /// <summary>
    /// Block name.
    /// </summary>
    [JsonPropertyName("blockName")]
    public string? BlockName { get; set; }

    /// <summary>
    /// Building name.
    /// </summary>
    [JsonPropertyName("buildingName")]
    public string? BuildingName { get; set; }

    /// <summary>
    /// Building number.
    /// </summary>
    [JsonPropertyName("buildingNumber")]
    public string? BuildingNumber { get; set; }

    /// <summary>
    /// Address descriptions.
    /// </summary>
    [JsonPropertyName("description")]
    public List<string>? Description { get; set; }

    /// <summary>
    /// In-house mail location.
    /// </summary>
    [JsonPropertyName("inhouseMail")]
    public string? InhouseMail { get; set; }

    /// <summary>
    /// Department name.
    /// </summary>
    [JsonPropertyName("department")]
    public string? Department { get; set; }

    /// <summary>
    /// Mark attention text.
    /// </summary>
    [JsonPropertyName("markAttention")]
    public string? MarkAttention { get; set; }

    /// <summary>
    /// Mark care text.
    /// </summary>
    [JsonPropertyName("markCare")]
    public string? MarkCare { get; set; }

    /// <summary>
    /// Plot identification.
    /// </summary>
    [JsonPropertyName("plotIdentification")]
    public string? PlotIdentification { get; set; }

    /// <summary>
    /// City subdivision name.
    /// </summary>
    [JsonPropertyName("citySubdivisionName")]
    public string? CitySubdivisionName { get; set; }

    /// <summary>
    /// City name.
    /// </summary>
    [JsonPropertyName("cityName")]
    public string? CityName { get; set; }

    /// <summary>
    /// Postal zone (postal code).
    /// </summary>
    [JsonPropertyName("postalZone")]
    public string? PostalZone { get; set; }

    /// <summary>
    /// Country subentity (state/province).
    /// </summary>
    [JsonPropertyName("countrySubentity")]
    public string? CountrySubentity { get; set; }

    /// <summary>
    /// Country subentity code.
    /// </summary>
    [JsonPropertyName("countrySubentityCode")]
    public Code? CountrySubentityCode { get; set; }

    /// <summary>
    /// Region name.
    /// </summary>
    [JsonPropertyName("region")]
    public string? Region { get; set; }

    /// <summary>
    /// District name.
    /// </summary>
    [JsonPropertyName("district")]
    public string? District { get; set; }

    /// <summary>
    /// Timezone offset.
    /// </summary>
    [JsonPropertyName("timezoneOffset")]
    public string? TimezoneOffset { get; set; }

    /// <summary>
    /// Address lines.
    /// </summary>
    [JsonPropertyName("addressLine")]
    public List<AddressLine>? AddressLine { get; set; }

    /// <summary>
    /// Country.
    /// </summary>
    [JsonPropertyName("country")]
    public Country? Country { get; set; }

    /// <summary>
    /// Location coordinates.
    /// </summary>
    [JsonPropertyName("locationCoordinate")]
    public LocationCoordinate? LocationCoordinate { get; set; }
}
