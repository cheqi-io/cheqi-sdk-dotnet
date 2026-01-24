using System.Text.Json.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Embedded Document - an embedded binary document.
/// </summary>
public sealed class EmbeddedDocument
{
    /// <summary>
    /// MIME type of the embedded document.
    /// </summary>
    [JsonPropertyName("mimeCode")]
    public string? MimeCode { get; set; }

    /// <summary>
    /// Filename of the embedded document.
    /// </summary>
    [JsonPropertyName("filename")]
    public string? Filename { get; set; }

    /// <summary>
    /// Embedded document binary object (Base64 encoded).
    /// </summary>
    [JsonPropertyName("embeddedDocumentBinaryObject")]
    public string? EmbeddedDocumentBinaryObject { get; set; }
}
