using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Credit Note - a complete credit note document.
/// </summary>
[XmlRoot("CreditNote", Namespace = UblNamespaces.CreditNote)]
public sealed class CreditNote
{
    /// <summary>
    /// Credit note identifier.
    /// </summary>
    [JsonPropertyName("id")]
    [XmlElement("ID", Namespace = UblNamespaces.Cbc)]
    public Identifier? Id { get; set; }

    /// <summary>
    /// Issue date.
    /// </summary>
    [JsonPropertyName("issueDate")]
    [XmlElement("IssueDate", Namespace = UblNamespaces.Cbc)]
    public string? IssueDate { get; set; }

    /// <summary>
    /// Issue time.
    /// </summary>
    [JsonPropertyName("issueTime")]
    [XmlElement("IssueTime", Namespace = UblNamespaces.Cbc)]
    public string? IssueTime { get; set; }

    /// <summary>
    /// Document currency code.
    /// </summary>
    [JsonPropertyName("documentCurrencyCode")]
    [XmlElement("DocumentCurrencyCode", Namespace = UblNamespaces.Cbc)]
    public string? DocumentCurrencyCode { get; set; }

    /// <summary>
    /// Note or additional text.
    /// </summary>
    [JsonPropertyName("note")]
    [XmlElement("Note", Namespace = UblNamespaces.Cbc)]
    public string? Note { get; set; }

    /// <summary>
    /// Credit note type code.
    /// </summary>
    [JsonPropertyName("creditNoteTypeCode")]
    [XmlElement("CreditNoteTypeCode", Namespace = UblNamespaces.Cbc)]
    public Code? CreditNoteTypeCode { get; set; }

    /// <summary>
    /// Supplier (seller) party.
    /// </summary>
    [JsonPropertyName("accountingSupplierParty")]
    [XmlElement("AccountingSupplierParty", Namespace = UblNamespaces.Cac)]
    public Party? AccountingSupplierParty { get; set; }

    /// <summary>
    /// Customer (buyer) party.
    /// </summary>
    [JsonPropertyName("accountingCustomerParty")]
    [XmlElement("AccountingCustomerParty", Namespace = UblNamespaces.Cac)]
    public Party? AccountingCustomerParty { get; set; }

    /// <summary>
    /// Legal monetary total.
    /// </summary>
    [JsonPropertyName("legalMonetaryTotal")]
    [XmlElement("LegalMonetaryTotal", Namespace = UblNamespaces.Cac)]
    public MonetaryTotal? LegalMonetaryTotal { get; set; }

    /// <summary>
    /// Originator document references.
    /// </summary>
    [JsonPropertyName("originatorDocumentReference")]
    [XmlElement("OriginatorDocumentReference", Namespace = UblNamespaces.Cac)]
    public List<DocumentReference>? OriginatorDocumentReference { get; set; }

    /// <summary>
    /// Credit note lines.
    /// </summary>
    [JsonPropertyName("creditNoteLine")]
    [XmlElement("CreditNoteLine", Namespace = UblNamespaces.Cac)]
    public List<CreditNoteLine>? CreditNoteLine { get; set; }

    /// <summary>
    /// Tax totals.
    /// </summary>
    [JsonPropertyName("taxTotal")]
    [XmlElement("TaxTotal", Namespace = UblNamespaces.Cac)]
    public List<TaxTotal>? TaxTotal { get; set; }

    /// <summary>
    /// Verification nonce.
    /// </summary>
    [JsonPropertyName("verificationNonce")]
    [XmlIgnore]
    public string? VerificationNonce { get; set; }
}
