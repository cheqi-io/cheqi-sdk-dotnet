using System.Text.Json.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Country - represents a country.
/// </summary>
public sealed class Country
{
    /// <summary>
    /// Country identification code (ISO 3166-1 alpha-2).
    /// </summary>
    [JsonPropertyName("identificationCode")]
    public string? IdentificationCode { get; set; }

    /// <summary>
    /// Country name.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }
}
