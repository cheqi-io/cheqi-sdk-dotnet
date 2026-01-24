using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Dimension - a measurement dimension.
/// </summary>
[XmlType(Namespace = UblNamespaces.Cac)]
public sealed class Dimension
{
    /// <summary>
    /// Attribute identifier.
    /// </summary>
    [JsonPropertyName("attributeId")]
    [XmlElement("AttributeID", Namespace = UblNamespaces.Cbc)]
    public Identifier? AttributeId { get; set; }

    /// <summary>
    /// Measurement value.
    /// </summary>
    [JsonPropertyName("measure")]
    [XmlElement("Measure", Namespace = UblNamespaces.Cbc)]
    public Measure? Measure { get; set; }

    /// <summary>
    /// Description.
    /// </summary>
    [JsonPropertyName("description")]
    [XmlElement("Description", Namespace = UblNamespaces.Cbc)]
    public string? Description { get; set; }

    /// <summary>
    /// Minimum measure.
    /// </summary>
    [JsonPropertyName("minimumMeasure")]
    [XmlElement("MinimumMeasure", Namespace = UblNamespaces.Cbc)]
    public Measure? MinimumMeasure { get; set; }

    /// <summary>
    /// Maximum measure.
    /// </summary>
    [JsonPropertyName("maximumMeasure")]
    [XmlElement("MaximumMeasure", Namespace = UblNamespaces.Cbc)]
    public Measure? MaximumMeasure { get; set; }
}
