using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Embedded Document - an embedded binary document.
/// </summary>
[XmlType(Namespace = UblNamespaces.Cac)]
public sealed class EmbeddedDocument
{
    /// <summary>
    /// MIME type of the embedded document.
    /// </summary>
    [JsonPropertyName("mimeCode")]
    [XmlElement("MimeCode", Namespace = UblNamespaces.Cbc)]
    public string? MimeCode { get; set; }

    /// <summary>
    /// Filename of the embedded document.
    /// </summary>
    [JsonPropertyName("filename")]
    [XmlElement("Filename", Namespace = UblNamespaces.Cbc)]
    public string? Filename { get; set; }

    /// <summary>
    /// Embedded document binary object (Base64 encoded).
    /// </summary>
    [JsonPropertyName("embeddedDocumentBinaryObject")]
    [XmlElement("EmbeddedDocumentBinaryObject", Namespace = UblNamespaces.Cbc)]
    public string? EmbeddedDocumentBinaryObject { get; set; }
}
