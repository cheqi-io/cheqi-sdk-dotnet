using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Location Coordinate - geographic coordinates.
/// </summary>
[XmlType(Namespace = UblNamespaces.Cac)]
public sealed class LocationCoordinate
{
    /// <summary>
    /// Coordinate system code.
    /// </summary>
    [JsonPropertyName("coordinateSystemCode")]
    [XmlElement("CoordinateSystemCode", Namespace = UblNamespaces.Cbc)]
    public Code? CoordinateSystemCode { get; set; }

    /// <summary>
    /// Latitude degrees measure.
    /// </summary>
    [JsonPropertyName("latitudeDegreesMeasure")]
    [XmlElement("LatitudeDegreesMeasure", Namespace = UblNamespaces.Cbc)]
    public Measure? LatitudeDegreesMeasure { get; set; }

    /// <summary>
    /// Latitude minutes measure.
    /// </summary>
    [JsonPropertyName("latitudeMinutesMeasure")]
    [XmlElement("LatitudeMinutesMeasure", Namespace = UblNamespaces.Cbc)]
    public Measure? LatitudeMinutesMeasure { get; set; }

    /// <summary>
    /// Latitude direction code (N/S).
    /// </summary>
    [JsonPropertyName("latitudeDirectionCode")]
    [XmlElement("LatitudeDirectionCode", Namespace = UblNamespaces.Cbc)]
    public Code? LatitudeDirectionCode { get; set; }

    /// <summary>
    /// Longitude degrees measure.
    /// </summary>
    [JsonPropertyName("longitudeDegreesMeasure")]
    [XmlElement("LongitudeDegreesMeasure", Namespace = UblNamespaces.Cbc)]
    public Measure? LongitudeDegreesMeasure { get; set; }

    /// <summary>
    /// Longitude minutes measure.
    /// </summary>
    [JsonPropertyName("longitudeMinutesMeasure")]
    [XmlElement("LongitudeMinutesMeasure", Namespace = UblNamespaces.Cbc)]
    public Measure? LongitudeMinutesMeasure { get; set; }

    /// <summary>
    /// Longitude direction code (E/W).
    /// </summary>
    [JsonPropertyName("longitudeDirectionCode")]
    [XmlElement("LongitudeDirectionCode", Namespace = UblNamespaces.Cbc)]
    public Code? LongitudeDirectionCode { get; set; }

    /// <summary>
    /// Altitude measure.
    /// </summary>
    [JsonPropertyName("altitudeMeasure")]
    [XmlElement("AltitudeMeasure", Namespace = UblNamespaces.Cbc)]
    public Measure? AltitudeMeasure { get; set; }
}
