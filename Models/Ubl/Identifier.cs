using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Identifier - a typed identifier with optional scheme information.
/// </summary>
[XmlType(Namespace = UblNamespaces.Cbc)]
public sealed class Identifier
{
    /// <summary>
    /// The identifier value.
    /// </summary>
    [JsonPropertyName("value")]
    [XmlText]
    public string? Value { get; set; }

    /// <summary>
    /// Scheme agency ID.
    /// </summary>
    [JsonPropertyName("schemeAgencyId")]
    [XmlAttribute("schemeAgencyID")]
    public string? SchemeAgencyId { get; set; }

    /// <summary>
    /// Scheme agency name.
    /// </summary>
    [JsonPropertyName("schemeAgencyName")]
    [XmlAttribute("schemeAgencyName")]
    public string? SchemeAgencyName { get; set; }

    /// <summary>
    /// Scheme data URI.
    /// </summary>
    [JsonPropertyName("schemeDataUri")]
    [XmlAttribute("schemeDataURI")]
    public string? SchemeDataUri { get; set; }

    /// <summary>
    /// Scheme ID.
    /// </summary>
    [JsonPropertyName("schemeId")]
    [XmlAttribute("schemeID")]
    public string? SchemeId { get; set; }

    /// <summary>
    /// Scheme name.
    /// </summary>
    [JsonPropertyName("schemeName")]
    [XmlAttribute("schemeName")]
    public string? SchemeName { get; set; }

    /// <summary>
    /// Scheme URI.
    /// </summary>
    [JsonPropertyName("schemeUri")]
    [XmlAttribute("schemeURI")]
    public string? SchemeUri { get; set; }

    /// <summary>
    /// Scheme version ID.
    /// </summary>
    [JsonPropertyName("schemeVersionId")]
    [XmlAttribute("schemeVersionID")]
    public string? SchemeVersionId { get; set; }
}
