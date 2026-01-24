using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Invoice Document Reference - a reference to an invoice document.
/// </summary>
[XmlType(Namespace = UblNamespaces.Cac)]
public sealed class InvoiceDocumentReference
{
    /// <summary>
    /// Document identifier.
    /// </summary>
    [JsonPropertyName("id")]
    [XmlElement("ID", Namespace = UblNamespaces.Cbc)]
    public Identifier? Id { get; set; }

    /// <summary>
    /// Issue date of the referenced document.
    /// </summary>
    [JsonPropertyName("issueDate")]
    [XmlElement("IssueDate", Namespace = UblNamespaces.Cbc)]
    public DateTimeOffset? IssueDate { get; set; }
}
