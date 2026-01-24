using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Quantity - a numeric quantity with unit information.
/// </summary>
[XmlType(Namespace = UblNamespaces.Cbc)]
public sealed class Quantity
{
    /// <summary>
    /// The quantity value.
    /// </summary>
    [JsonPropertyName("value")]
    [XmlText]
    public decimal? Value { get; set; }

    /// <summary>
    /// Unit code (e.g., "EA" for each, "KGM" for kilogram).
    /// </summary>
    [JsonPropertyName("unitCode")]
    [XmlAttribute("unitCode")]
    public string? UnitCode { get; set; }

    /// <summary>
    /// Unit code list agency ID.
    /// </summary>
    [JsonPropertyName("unitCodeListAgencyId")]
    [XmlAttribute("unitCodeListAgencyID")]
    public string? UnitCodeListAgencyId { get; set; }

    /// <summary>
    /// Unit code list agency name.
    /// </summary>
    [JsonPropertyName("unitCodeListAgencyName")]
    [XmlAttribute("unitCodeListAgencyName")]
    public string? UnitCodeListAgencyName { get; set; }

    /// <summary>
    /// Unit code list ID.
    /// </summary>
    [JsonPropertyName("unitCodeListId")]
    [XmlAttribute("unitCodeListID")]
    public string? UnitCodeListId { get; set; }
}
