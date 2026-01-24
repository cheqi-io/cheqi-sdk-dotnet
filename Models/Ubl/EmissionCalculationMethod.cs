using System.Text.Json.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Emission Calculation Method - method for calculating emissions.
/// </summary>
public sealed class EmissionCalculationMethod
{
    /// <summary>
    /// Calculation method code.
    /// </summary>
    [JsonPropertyName("calculationMethodCode")]
    public string? CalculationMethodCode { get; set; }

    /// <summary>
    /// Fullness indication code.
    /// </summary>
    [JsonPropertyName("fullnessIndicationCode")]
    public string? FullnessIndicationCode { get; set; }

    /// <summary>
    /// Measurement from location.
    /// </summary>
    [JsonPropertyName("measurementFromLocation")]
    public Location? MeasurementFromLocation { get; set; }

    /// <summary>
    /// Measurement to location.
    /// </summary>
    [JsonPropertyName("measurementToLocation")]
    public Location? MeasurementToLocation { get; set; }
}
