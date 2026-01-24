using System.Text.Json.Serialization;
using Cheqi.Sdk.Models.Enums;

namespace Cheqi.Sdk.Models;

/// <summary>
/// Request for generating a receipt template.
/// </summary>
public sealed class ReceiptTemplateRequest
{
    /// <summary>
    /// Unique receipt identifier from the POS system.
    /// </summary>
    [JsonPropertyName("receiptId")]
    public string ReceiptId { get; set; } = string.Empty;

    /// <summary>
    /// Transaction timestamp.
    /// </summary>
    [JsonPropertyName("transactionDateTime")]
    public DateTimeOffset TransactionDateTime { get; set; }

    /// <summary>
    /// Currency code (ISO 4217, e.g., "EUR").
    /// </summary>
    [JsonPropertyName("currencyCode")]
    public string CurrencyCode { get; set; } = "EUR";

    /// <summary>
    /// Total amount including tax.
    /// </summary>
    [JsonPropertyName("totalAmount")]
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Total amount excluding tax.
    /// </summary>
    [JsonPropertyName("totalAmountExcludingTax")]
    public decimal? TotalAmountExcludingTax { get; set; }

    /// <summary>
    /// Total tax amount.
    /// </summary>
    [JsonPropertyName("totalTaxAmount")]
    public decimal? TotalTaxAmount { get; set; }

    /// <summary>
    /// Payment method used.
    /// </summary>
    [JsonPropertyName("paymentType")]
    public PaymentType PaymentType { get; set; }

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
    /// Store address.
    /// </summary>
    [JsonPropertyName("storeAddress")]
    public string? StoreAddress { get; set; }

    /// <summary>
    /// Terminal or register identifier.
    /// </summary>
    [JsonPropertyName("terminalId")]
    public string? TerminalId { get; set; }

    /// <summary>
    /// Cashier or operator identifier.
    /// </summary>
    [JsonPropertyName("cashierId")]
    public string? CashierId { get; set; }

    /// <summary>
    /// Line items on the receipt.
    /// </summary>
    [JsonPropertyName("products")]
    public List<Product> Products { get; set; } = new();

    /// <summary>
    /// Tax breakdown by category.
    /// </summary>
    [JsonPropertyName("taxes")]
    public List<Tax>? Taxes { get; set; }

    /// <summary>
    /// Discount amount applied.
    /// </summary>
    [JsonPropertyName("discountAmount")]
    public decimal? DiscountAmount { get; set; }

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

    /// <summary>
    /// Type of receiver (customer or business).
    /// </summary>
    [JsonPropertyName("receiverType")]
    public ReceiverType ReceiverType { get; set; } = ReceiverType.CUSTOMER;
}
