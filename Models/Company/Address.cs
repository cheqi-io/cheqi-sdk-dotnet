using System.Text.Json.Serialization;

namespace Cheqi.Sdk.Models.Company;

/// <summary>
/// Physical address for a company or store.
/// </summary>
public sealed class Address
{
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
    /// Address line.
    /// </summary>
    [JsonPropertyName("addressLine")]
    public string? AddressLine { get; set; }

    /// <summary>
    /// Region or state.
    /// </summary>
    [JsonPropertyName("region")]
    public string? Region { get; set; }

    /// <summary>
    /// City name.
    /// </summary>
    [JsonPropertyName("cityName")]
    public string? CityName { get; set; }

    /// <summary>
    /// Postal zone or ZIP code.
    /// </summary>
    [JsonPropertyName("postalZone")]
    public string? PostalZone { get; set; }

    /// <summary>
    /// Country ISO code (2-letter).
    /// </summary>
    [JsonPropertyName("countryIsoCode")]
    public string? CountryIsoCode { get; set; }

    /// <summary>
    /// Validates the address.
    /// </summary>
    public bool IsValid() => GetValidationErrors().Count == 0;

    /// <summary>
    /// Gets validation errors for the address.
    /// </summary>
    public List<string> GetValidationErrors()
    {
        var errors = new List<string>();
        
        if (string.IsNullOrWhiteSpace(StreetName))
            errors.Add("streetName is required");
        
        if (string.IsNullOrWhiteSpace(CityName))
            errors.Add("cityName is required");
        
        if (string.IsNullOrWhiteSpace(PostalZone))
            errors.Add("postalZone is required");
        
        if (string.IsNullOrWhiteSpace(CountryIsoCode))
            errors.Add("countryIsoCode is required");
        else if (CountryIsoCode.Length != 2)
            errors.Add("countryIsoCode must be a 2-letter ISO code");
        
        return errors;
    }
}
