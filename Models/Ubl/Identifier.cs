using System.Text.Json.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Identifier - a typed identifier with optional scheme information.
/// </summary>
public sealed class Identifier
{
    /// <summary>
    /// The identifier value.
    /// </summary>
    [JsonPropertyName("value")]
    public string? Value { get; set; }

    /// <summary>
    /// Scheme agency ID.
    /// </summary>
    [JsonPropertyName("schemeAgencyId")]
    public string? SchemeAgencyId { get; set; }

    /// <summary>
    /// Scheme agency name.
    /// </summary>
    [JsonPropertyName("schemeAgencyName")]
    public string? SchemeAgencyName { get; set; }

    /// <summary>
    /// Scheme data URI.
    /// </summary>
    [JsonPropertyName("schemeDataUri")]
    public string? SchemeDataUri { get; set; }

    /// <summary>
    /// Scheme ID.
    /// </summary>
    [JsonPropertyName("schemeId")]
    public string? SchemeId { get; set; }

    /// <summary>
    /// Scheme name.
    /// </summary>
    [JsonPropertyName("schemeName")]
    public string? SchemeName { get; set; }

    /// <summary>
    /// Scheme URI.
    /// </summary>
    [JsonPropertyName("schemeUri")]
    public string? SchemeUri { get; set; }

    /// <summary>
    /// Scheme version ID.
    /// </summary>
    [JsonPropertyName("schemeVersionId")]
    public string? SchemeVersionId { get; set; }
}
