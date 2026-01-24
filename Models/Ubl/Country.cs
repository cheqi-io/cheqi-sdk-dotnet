using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Country - represents a country.
/// </summary>
[XmlType(Namespace = UblNamespaces.Cac)]
public sealed class Country
{
    /// <summary>
    /// Country identification code (ISO 3166-1 alpha-2).
    /// </summary>
    [JsonPropertyName("identificationCode")]
    [XmlElement("IdentificationCode", Namespace = UblNamespaces.Cbc)]
    public string? IdentificationCode { get; set; }

    /// <summary>
    /// Country name.
    /// </summary>
    [JsonPropertyName("name")]
    [XmlElement("Name", Namespace = UblNamespaces.Cbc)]
    public string? Name { get; set; }
}
