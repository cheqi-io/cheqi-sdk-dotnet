using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Environmental Emission - environmental emission information.
/// </summary>
[XmlType(Namespace = UblNamespaces.Cac)]
public sealed class EnvironmentalEmission
{
    /// <summary>
    /// Environmental emission type code.
    /// </summary>
    [JsonPropertyName("environmentalEmissionTypeCode")]
    [XmlElement("EnvironmentalEmissionTypeCode", Namespace = UblNamespaces.Cbc)]
    public string? EnvironmentalEmissionTypeCode { get; set; }

    /// <summary>
    /// Value measure.
    /// </summary>
    [JsonPropertyName("valueMeasure")]
    [XmlElement("ValueMeasure", Namespace = UblNamespaces.Cbc)]
    public Measure? ValueMeasure { get; set; }

    /// <summary>
    /// Descriptions.
    /// </summary>
    [JsonPropertyName("description")]
    [XmlElement("Description", Namespace = UblNamespaces.Cbc)]
    public List<string>? Description { get; set; }

    /// <summary>
    /// Emission calculation methods.
    /// </summary>
    [JsonPropertyName("emissionCalculationMethod")]
    [XmlElement("EmissionCalculationMethod", Namespace = UblNamespaces.Cac)]
    public List<EmissionCalculationMethod>? EmissionCalculationMethod { get; set; }
}
