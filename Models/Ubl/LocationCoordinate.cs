using System.Text.Json.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Location Coordinate - geographic coordinates.
/// </summary>
public sealed class LocationCoordinate
{
    /// <summary>
    /// Coordinate system code.
    /// </summary>
    [JsonPropertyName("coordinateSystemCode")]
    public Code? CoordinateSystemCode { get; set; }

    /// <summary>
    /// Latitude degrees measure.
    /// </summary>
    [JsonPropertyName("latitudeDegreesMeasure")]
    public Measure? LatitudeDegreesMeasure { get; set; }

    /// <summary>
    /// Latitude minutes measure.
    /// </summary>
    [JsonPropertyName("latitudeMinutesMeasure")]
    public Measure? LatitudeMinutesMeasure { get; set; }

    /// <summary>
    /// Latitude direction code (N/S).
    /// </summary>
    [JsonPropertyName("latitudeDirectionCode")]
    public Code? LatitudeDirectionCode { get; set; }

    /// <summary>
    /// Longitude degrees measure.
    /// </summary>
    [JsonPropertyName("longitudeDegreesMeasure")]
    public Measure? LongitudeDegreesMeasure { get; set; }

    /// <summary>
    /// Longitude minutes measure.
    /// </summary>
    [JsonPropertyName("longitudeMinutesMeasure")]
    public Measure? LongitudeMinutesMeasure { get; set; }

    /// <summary>
    /// Longitude direction code (E/W).
    /// </summary>
    [JsonPropertyName("longitudeDirectionCode")]
    public Code? LongitudeDirectionCode { get; set; }

    /// <summary>
    /// Altitude measure.
    /// </summary>
    [JsonPropertyName("altitudeMeasure")]
    public Measure? AltitudeMeasure { get; set; }
}
