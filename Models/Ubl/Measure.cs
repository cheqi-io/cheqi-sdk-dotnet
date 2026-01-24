using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Measure - a measurement with unit.
/// </summary>
[XmlType(Namespace = UblNamespaces.Cbc)]
public sealed class Measure
{
    /// <summary>
    /// The measurement value.
    /// </summary>
    [JsonPropertyName("value")]
    [XmlText]
    public decimal? Value { get; set; }

    /// <summary>
    /// Unit code for the measurement.
    /// </summary>
    [JsonPropertyName("unitCode")]
    [XmlAttribute("unitCode")]
    public string? UnitCode { get; set; }

    /// <summary>
    /// Unit code list version ID.
    /// </summary>
    [JsonPropertyName("unitCodeListVersionId")]
    [XmlAttribute("unitCodeListVersionID")]
    public string? UnitCodeListVersionId { get; set; }
}
