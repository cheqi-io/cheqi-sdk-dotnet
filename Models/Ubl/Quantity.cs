using System.Text.Json.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Quantity - a numeric quantity with unit information.
/// </summary>
public sealed class Quantity
{
    /// <summary>
    /// The quantity value.
    /// </summary>
    [JsonPropertyName("value")]
    public decimal? Value { get; set; }

    /// <summary>
    /// Unit code (e.g., "EA" for each, "KGM" for kilogram).
    /// </summary>
    [JsonPropertyName("unitCode")]
    public string? UnitCode { get; set; }

    /// <summary>
    /// Unit code list agency ID.
    /// </summary>
    [JsonPropertyName("unitCodeListAgencyId")]
    public string? UnitCodeListAgencyId { get; set; }

    /// <summary>
    /// Unit code list agency name.
    /// </summary>
    [JsonPropertyName("unitCodeListAgencyName")]
    public string? UnitCodeListAgencyName { get; set; }

    /// <summary>
    /// Unit code list ID.
    /// </summary>
    [JsonPropertyName("unitCodeListId")]
    public string? UnitCodeListId { get; set; }
}
