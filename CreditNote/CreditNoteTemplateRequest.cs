using System.Text.Json.Serialization;
using Cheqi.Sdk.Models;

namespace Cheqi.Sdk.CreditNote;

/// <summary>
/// Request for generating a credit note template.
/// </summary>
public sealed class CreditNoteTemplateRequest
{
    /// <summary>
    /// Reference to the original receipt/document.
    /// </summary>
    [JsonPropertyName("originatorDocumentReference")]
    public string? OriginalReceiptId { get; set; }

    /// <summary>
    /// Unique credit note/document number from the POS system.
    /// </summary>
    [JsonPropertyName("documentNumber")]
    public string CreditNoteId { get; set; } = string.Empty;

    /// <summary>
    /// Additional identifiers for the credit note.
    /// </summary>
    [JsonPropertyName("identifiers")]
    public List<Identifier>? Identifiers { get; set; }

    /// <summary>
    /// Issue date/time of the credit note.
    /// </summary>
    [JsonPropertyName("issueDate")]
    public DateTimeOffset IssueDateTime { get; set; }

    /// <summary>
    /// Currency code (ISO 4217, e.g., "EUR").
    /// </summary>
    [JsonPropertyName("currency")]
    public string CurrencyCode { get; set; } = "EUR";

    /// <summary>
    /// Invoice subtotal.
    /// </summary>
    [JsonPropertyName("invoiceSubtotal")]
    public decimal? InvoiceSubtotal { get; set; }

    /// <summary>
    /// Total credit amount excluding tax.
    /// </summary>
    [JsonPropertyName("totalBeforeTax")]
    public decimal? TotalAmountExcludingTax { get; set; }

    /// <summary>
    /// Total tax amount.
    /// </summary>
    [JsonPropertyName("totalTaxAmount")]
    public decimal? TotalTaxAmount { get; set; }

    /// <summary>
    /// Total credit amount including tax.
    /// </summary>
    [JsonPropertyName("totalAmount")]
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Line items being credited.
    /// </summary>
    [JsonPropertyName("products")]
    public List<Product> Products { get; set; } = new();

    /// <summary>
    /// Credit note-level discounts.
    /// </summary>
    [JsonPropertyName("discounts")]
    public List<Discount>? Discounts { get; set; }

    /// <summary>
    /// Tax breakdown by category.
    /// </summary>
    [JsonPropertyName("taxes")]
    public List<Tax>? Taxes { get; set; }

    /// <summary>
    /// Additional note or reason for the credit note.
    /// </summary>
    [JsonPropertyName("note")]
    public string? Reason { get; set; }
}
