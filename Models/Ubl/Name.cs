using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Name - a name value with optional language information.
/// </summary>
[XmlType(Namespace = UblNamespaces.Cbc)]
public sealed class Name
{
    /// <summary>
    /// The name value.
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
    /// Language locale ID.
    /// </summary>
    [JsonPropertyName("languageLocaleId")]
    [XmlAttribute("languageLocaleID")]
    public string? LanguageLocaleId { get; set; }
}
