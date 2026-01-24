using System.Text.Json.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Measure - a measurement with unit.
/// </summary>
public sealed class Measure
{
    /// <summary>
    /// The measurement value.
    /// </summary>
    [JsonPropertyName("value")]
    public decimal? Value { get; set; }

    /// <summary>
    /// Unit code for the measurement.
    /// </summary>
    [JsonPropertyName("unitCode")]
    public string? UnitCode { get; set; }

    /// <summary>
    /// Unit code list version ID.
    /// </summary>
    [JsonPropertyName("unitCodeListVersionId")]
    public string? UnitCodeListVersionId { get; set; }
}
