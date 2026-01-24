using System.Text.Json.Serialization;
using Cheqi.Sdk.Models;

namespace Cheqi.Sdk.CreditNote;

/// <summary>
/// Request for generating a credit note template.
/// </summary>
public sealed class CreditNoteTemplateRequest
{
    /// <summary>
    /// Unique credit note identifier from the POS system.
    /// </summary>
    [JsonPropertyName("creditNoteId")]
    public string CreditNoteId { get; set; } = string.Empty;

    /// <summary>
    /// Reference to the original receipt ID.
    /// </summary>
    [JsonPropertyName("originalReceiptId")]
    public string? OriginalReceiptId { get; set; }

    /// <summary>
    /// Credit note timestamp.
    /// </summary>
    [JsonPropertyName("issueDateTime")]
    public DateTimeOffset IssueDateTime { get; set; }

    /// <summary>
    /// Currency code (ISO 4217, e.g., "EUR").
    /// </summary>
    [JsonPropertyName("currencyCode")]
    public string CurrencyCode { get; set; } = "EUR";

    /// <summary>
    /// Total credit amount including tax.
    /// </summary>
    [JsonPropertyName("totalAmount")]
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Total credit amount excluding tax.
    /// </summary>
    [JsonPropertyName("totalAmountExcludingTax")]
    public decimal? TotalAmountExcludingTax { get; set; }

    /// <summary>
    /// Total tax amount.
    /// </summary>
    [JsonPropertyName("totalTaxAmount")]
    public decimal? TotalTaxAmount { get; set; }

    /// <summary>
    /// Reason for the credit note.
    /// </summary>
    [JsonPropertyName("reason")]
    public string? Reason { get; set; }

    /// <summary>
    /// Store identifier.
    /// </summary>
    [JsonPropertyName("storeId")]
    public string? StoreId { get; set; }

    /// <summary>
    /// Store name.
    /// </summary>
    [JsonPropertyName("storeName")]
    public string? StoreName { get; set; }

    /// <summary>
    /// Line items being credited.
    /// </summary>
    [JsonPropertyName("products")]
    public List<Product> Products { get; set; } = new();

    /// <summary>
    /// Tax breakdown by category.
    /// </summary>
    [JsonPropertyName("taxes")]
    public List<Tax>? Taxes { get; set; }

    /// <summary>
    /// Additional notes or comments.
    /// </summary>
    [JsonPropertyName("notes")]
    public string? Notes { get; set; }

    /// <summary>
    /// Custom metadata as key-value pairs.
    /// </summary>
    [JsonPropertyName("metadata")]
    public Dictionary<string, string>? Metadata { get; set; }
}
