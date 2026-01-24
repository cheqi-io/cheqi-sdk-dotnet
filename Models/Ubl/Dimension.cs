using System.Text.Json.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Dimension - a measurement dimension.
/// </summary>
public sealed class Dimension
{
    /// <summary>
    /// Attribute identifier.
    /// </summary>
    [JsonPropertyName("attributeId")]
    public Identifier? AttributeId { get; set; }

    /// <summary>
    /// Measurement value.
    /// </summary>
    [JsonPropertyName("measure")]
    public Measure? Measure { get; set; }

    /// <summary>
    /// Description.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Minimum measure.
    /// </summary>
    [JsonPropertyName("minimumMeasure")]
    public Measure? MinimumMeasure { get; set; }

    /// <summary>
    /// Maximum measure.
    /// </summary>
    [JsonPropertyName("maximumMeasure")]
    public Measure? MaximumMeasure { get; set; }
}
