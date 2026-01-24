using System.Text.Json.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Location - a physical or logical location.
/// </summary>
public sealed class Location
{
    /// <summary>
    /// Location identifier.
    /// </summary>
    [JsonPropertyName("id")]
    public Identifier? Id { get; set; }

    /// <summary>
    /// Location descriptions.
    /// </summary>
    [JsonPropertyName("description")]
    public List<string>? Description { get; set; }

    /// <summary>
    /// Location conditions.
    /// </summary>
    [JsonPropertyName("conditions")]
    public List<string>? Conditions { get; set; }

    /// <summary>
    /// Country subentity.
    /// </summary>
    [JsonPropertyName("countrySubentity")]
    public string? CountrySubentity { get; set; }

    /// <summary>
    /// Country subentity code.
    /// </summary>
    [JsonPropertyName("countrySubentityCode")]
    public Code? CountrySubentityCode { get; set; }

    /// <summary>
    /// Location type code.
    /// </summary>
    [JsonPropertyName("locationTypeCode")]
    public Code? LocationTypeCode { get; set; }

    /// <summary>
    /// Information URI.
    /// </summary>
    [JsonPropertyName("informationUri")]
    public Identifier? InformationUri { get; set; }

    /// <summary>
    /// Location name.
    /// </summary>
    [JsonPropertyName("name")]
    public Name? Name { get; set; }

    /// <summary>
    /// Validity periods.
    /// </summary>
    [JsonPropertyName("validityPeriod")]
    public List<Period>? ValidityPeriod { get; set; }

    /// <summary>
    /// Address of the location.
    /// </summary>
    [JsonPropertyName("address")]
    public Address? Address { get; set; }

    /// <summary>
    /// Location coordinates.
    /// </summary>
    [JsonPropertyName("locationCoordinate")]
    public List<LocationCoordinate>? LocationCoordinate { get; set; }
}
