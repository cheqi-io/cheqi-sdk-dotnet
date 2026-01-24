using System.Text.Json.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Environmental Emission - environmental emission information.
/// </summary>
public sealed class EnvironmentalEmission
{
    /// <summary>
    /// Environmental emission type code.
    /// </summary>
    [JsonPropertyName("environmentalEmissionTypeCode")]
    public string? EnvironmentalEmissionTypeCode { get; set; }

    /// <summary>
    /// Value measure.
    /// </summary>
    [JsonPropertyName("valueMeasure")]
    public Measure? ValueMeasure { get; set; }

    /// <summary>
    /// Descriptions.
    /// </summary>
    [JsonPropertyName("description")]
    public List<string>? Description { get; set; }

    /// <summary>
    /// Emission calculation methods.
    /// </summary>
    [JsonPropertyName("emissionCalculationMethod")]
    public List<EmissionCalculationMethod>? EmissionCalculationMethod { get; set; }
}
