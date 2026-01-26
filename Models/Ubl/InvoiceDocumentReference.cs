using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Invoice Document Reference - a reference to an invoice document.
/// This is a specialized DocumentReference used in billing contexts.
/// </summary>
[XmlType(Namespace = UblNamespaces.Cac)]
public sealed class InvoiceDocumentReference
{
    /// <summary>
    /// Document ID.
    /// </summary>
    [JsonPropertyName("id")]
    [XmlElement("ID", Namespace = UblNamespaces.Cbc)]
    public Identifier? Id { get; set; }

    /// <summary>
    /// Document type code.
    /// </summary>
    [JsonPropertyName("documentTypeCode")]
    [XmlElement("DocumentTypeCode", Namespace = UblNamespaces.Cbc)]
    public Code? DocumentTypeCode { get; set; }

    /// <summary>
    /// Document description.
    /// </summary>
    [JsonPropertyName("documentDescription")]
    [XmlElement("DocumentDescription", Namespace = UblNamespaces.Cbc)]
    public string? DocumentDescription { get; set; }

    /// <summary>
    /// Issue date.
    /// </summary>
    [JsonPropertyName("issueDate")]
    [XmlElement("IssueDate", Namespace = UblNamespaces.Cbc)]
    public string? IssueDate { get; set; }

    /// <summary>
    /// Attachment (embedded document).
    /// </summary>
    [JsonPropertyName("attachment")]
    [XmlElement("Attachment", Namespace = UblNamespaces.Cac)]
    public Attachment? Attachment { get; set; }
}