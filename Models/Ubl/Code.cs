using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Code - a coded value with optional list information.
/// </summary>
[XmlType(Namespace = UblNamespaces.Cbc)]
public sealed class Code
{
    /// <summary>
    /// The code value.
    /// </summary>
    [JsonPropertyName("value")]
    [XmlText]
    public string? Value { get; set; }

    /// <summary>
    /// Language ID.
    /// </summary>
    [JsonPropertyName("languageId")]
    [XmlAttribute("languageID")]
    public string? LanguageId { get; set; }

    /// <summary>
    /// List agency ID.
    /// </summary>
    [JsonPropertyName("listAgencyId")]
    [XmlAttribute("listAgencyID")]
    public string? ListAgencyId { get; set; }

    /// <summary>
    /// List agency name.
    /// </summary>
    [JsonPropertyName("listAgencyName")]
    [XmlAttribute("listAgencyName")]
    public string? ListAgencyName { get; set; }

    /// <summary>
    /// List ID.
    /// </summary>
    [JsonPropertyName("listId")]
    [XmlAttribute("listID")]
    public string? ListId { get; set; }

    /// <summary>
    /// List name.
    /// </summary>
    [JsonPropertyName("listName")]
    [XmlAttribute("listName")]
    public string? ListName { get; set; }

    /// <summary>
    /// List scheme URI.
    /// </summary>
    [JsonPropertyName("listSchemeUri")]
    [XmlAttribute("listSchemeURI")]
    public string? ListSchemeUri { get; set; }

    /// <summary>
    /// List URI.
    /// </summary>
    [JsonPropertyName("listUri")]
    [XmlAttribute("listURI")]
    public string? ListUri { get; set; }

    /// <summary>
    /// List version ID.
    /// </summary>
    [JsonPropertyName("listVersionId")]
    [XmlAttribute("listVersionID")]
    public string? ListVersionId { get; set; }

    /// <summary>
    /// Code name.
    /// </summary>
    [JsonPropertyName("name")]
    [XmlAttribute("name")]
    public string? Name { get; set; }
}
