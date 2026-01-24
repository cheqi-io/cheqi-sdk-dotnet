using System.Text.Json.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Period - a time period with start and end dates/times.
/// </summary>
public sealed class Period
{
    /// <summary>
    /// Start date of the period.
    /// </summary>
    [JsonPropertyName("startDate")]
    public string? StartDate { get; set; }

    /// <summary>
    /// Start time of the period.
    /// </summary>
    [JsonPropertyName("startTime")]
    public string? StartTime { get; set; }

    /// <summary>
    /// End date of the period.
    /// </summary>
    [JsonPropertyName("endDate")]
    public string? EndDate { get; set; }

    /// <summary>
    /// End time of the period.
    /// </summary>
    [JsonPropertyName("endTime")]
    public string? EndTime { get; set; }

    /// <summary>
    /// Duration measure.
    /// </summary>
    [JsonPropertyName("durationMeasure")]
    public Measure? DurationMeasure { get; set; }

    /// <summary>
    /// Description code.
    /// </summary>
    [JsonPropertyName("descriptionCode")]
    public Code? DescriptionCode { get; set; }

    /// <summary>
    /// Description text.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }
}
