using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Billing Reference - a reference to a billing document.
/// </summary>
[XmlType(Namespace = UblNamespaces.Cac)]
public sealed class BillingReference
{
    /// <summary>
    /// Reference to an invoice document.
    /// </summary>
    [JsonPropertyName("invoiceDocumentReference")]
    [XmlElement("InvoiceDocumentReference", Namespace = UblNamespaces.Cac)]
    public InvoiceDocumentReference? InvoiceDocumentReference { get; set; }
}
