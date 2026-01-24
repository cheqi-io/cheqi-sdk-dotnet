using System.Text.Json.Serialization;

namespace Cheqi.Sdk.Models.Company;

/// <summary>
/// Physical address for a company or store.
/// </summary>
public sealed class Address
{
    /// <summary>
    /// Street address line 1.
    /// </summary>
    [JsonPropertyName("streetAddress")]
    public string? StreetAddress { get; set; }

    /// <summary>
    /// Street address line 2.
    /// </summary>
    [JsonPropertyName("streetAddress2")]
    public string? StreetAddress2 { get; set; }

    /// <summary>
    /// City name.
    /// </summary>
    [JsonPropertyName("city")]
    public string? City { get; set; }

    /// <summary>
    /// State or province.
    /// </summary>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>
    /// Postal or ZIP code.
    /// </summary>
    [JsonPropertyName("postalCode")]
    public string? PostalCode { get; set; }

    /// <summary>
    /// Country code (ISO 3166-1 alpha-2).
    /// </summary>
    [JsonPropertyName("countryCode")]
    public string? CountryCode { get; set; }

    /// <summary>
    /// Country name.
    /// </summary>
    [JsonPropertyName("country")]
    public string? Country { get; set; }
}
