using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Emission Calculation Method - method for calculating emissions.
/// </summary>
[XmlType(Namespace = UblNamespaces.Cac)]
public sealed class EmissionCalculationMethod
{
    /// <summary>
    /// Calculation method code.
    /// </summary>
    [JsonPropertyName("calculationMethodCode")]
    [XmlElement("CalculationMethodCode", Namespace = UblNamespaces.Cbc)]
    public string? CalculationMethodCode { get; set; }

    /// <summary>
    /// Fullness indication code.
    /// </summary>
    [JsonPropertyName("fullnessIndicationCode")]
    [XmlElement("FullnessIndicationCode", Namespace = UblNamespaces.Cbc)]
    public string? FullnessIndicationCode { get; set; }

    /// <summary>
    /// Measurement from location.
    /// </summary>
    [JsonPropertyName("measurementFromLocation")]
    [XmlElement("MeasurementFromLocation", Namespace = UblNamespaces.Cac)]
    public Location? MeasurementFromLocation { get; set; }

    /// <summary>
    /// Measurement to location.
    /// </summary>
    [JsonPropertyName("measurementToLocation")]
    [XmlElement("MeasurementToLocation", Namespace = UblNamespaces.Cac)]
    public Location? MeasurementToLocation { get; set; }
}
