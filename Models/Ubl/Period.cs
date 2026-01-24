using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Period - a time period with start and end dates/times.
/// </summary>
[XmlType(Namespace = UblNamespaces.Cac)]
public sealed class Period
{
    /// <summary>
    /// Start date of the period.
    /// </summary>
    [JsonPropertyName("startDate")]
    [XmlElement("StartDate", Namespace = UblNamespaces.Cbc)]
    public string? StartDate { get; set; }

    /// <summary>
    /// Start time of the period.
    /// </summary>
    [JsonPropertyName("startTime")]
    [XmlElement("StartTime", Namespace = UblNamespaces.Cbc)]
    public string? StartTime { get; set; }

    /// <summary>
    /// End date of the period.
    /// </summary>
    [JsonPropertyName("endDate")]
    [XmlElement("EndDate", Namespace = UblNamespaces.Cbc)]
    public string? EndDate { get; set; }

    /// <summary>
    /// End time of the period.
    /// </summary>
    [JsonPropertyName("endTime")]
    [XmlElement("EndTime", Namespace = UblNamespaces.Cbc)]
    public string? EndTime { get; set; }

    /// <summary>
    /// Duration measure.
    /// </summary>
    [JsonPropertyName("durationMeasure")]
    [XmlElement("DurationMeasure", Namespace = UblNamespaces.Cbc)]
    public Measure? DurationMeasure { get; set; }

    /// <summary>
    /// Description code.
    /// </summary>
    [JsonPropertyName("descriptionCode")]
    [XmlElement("DescriptionCode", Namespace = UblNamespaces.Cbc)]
    public Code? DescriptionCode { get; set; }

    /// <summary>
    /// Description text.
    /// </summary>
    [JsonPropertyName("description")]
    [XmlElement("Description", Namespace = UblNamespaces.Cbc)]
    public string? Description { get; set; }
}
