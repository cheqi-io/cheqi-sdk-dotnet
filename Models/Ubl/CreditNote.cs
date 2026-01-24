using System.Text.Json.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Credit Note - a complete credit note document.
/// </summary>
public sealed class CreditNote
{
    /// <summary>
    /// Credit note identifier.
    /// </summary>
    [JsonPropertyName("id")]
    public Identifier? Id { get; set; }

    /// <summary>
    /// Issue date.
    /// </summary>
    [JsonPropertyName("issueDate")]
    public string? IssueDate { get; set; }

    /// <summary>
    /// Issue time.
    /// </summary>
    [JsonPropertyName("issueTime")]
    public string? IssueTime { get; set; }

    /// <summary>
    /// Document currency code.
    /// </summary>
    [JsonPropertyName("documentCurrencyCode")]
    public string? DocumentCurrencyCode { get; set; }

    /// <summary>
    /// Note or additional text.
    /// </summary>
    [JsonPropertyName("note")]
    public string? Note { get; set; }

    /// <summary>
    /// Credit note type code.
    /// </summary>
    [JsonPropertyName("creditNoteTypeCode")]
    public Code? CreditNoteTypeCode { get; set; }

    /// <summary>
    /// Supplier (seller) party.
    /// </summary>
    [JsonPropertyName("accountingSupplierParty")]
    public Party? AccountingSupplierParty { get; set; }

    /// <summary>
    /// Customer (buyer) party.
    /// </summary>
    [JsonPropertyName("accountingCustomerParty")]
    public Party? AccountingCustomerParty { get; set; }

    /// <summary>
    /// Legal monetary total.
    /// </summary>
    [JsonPropertyName("legalMonetaryTotal")]
    public MonetaryTotal? LegalMonetaryTotal { get; set; }

    /// <summary>
    /// Originator document references.
    /// </summary>
    [JsonPropertyName("originatorDocumentReference")]
    public List<DocumentReference>? OriginatorDocumentReference { get; set; }

    /// <summary>
    /// Credit note lines.
    /// </summary>
    [JsonPropertyName("creditNoteLine")]
    public List<CreditNoteLine>? CreditNoteLine { get; set; }

    /// <summary>
    /// Tax totals.
    /// </summary>
    [JsonPropertyName("taxTotal")]
    public List<TaxTotal>? TaxTotal { get; set; }

    /// <summary>
    /// Verification nonce.
    /// </summary>
    [JsonPropertyName("verificationNonce")]
    public string? VerificationNonce { get; set; }
}
