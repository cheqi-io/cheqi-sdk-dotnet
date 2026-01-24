using System.Text.Json.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Tax Total - tax summary for an invoice or receipt.
/// </summary>
public sealed class TaxTotal
{
    /// <summary>
    /// Total tax amount.
    /// </summary>
    [JsonPropertyName("taxAmount")]
    public decimal TaxAmount { get; set; }

    /// <summary>
    /// Currency code.
    /// </summary>
    [JsonPropertyName("currencyId")]
    public string? CurrencyId { get; set; }

    /// <summary>
    /// Tax subtotals by category.
    /// </summary>
    [JsonPropertyName("taxSubtotals")]
    public List<TaxSubtotal>? TaxSubtotals { get; set; }
}

/// <summary>
/// UBL Tax Subtotal - tax breakdown by category.
/// </summary>
public sealed class TaxSubtotal
{
    /// <summary>
    /// Taxable amount for this category.
    /// </summary>
    [JsonPropertyName("taxableAmount")]
    public decimal TaxableAmount { get; set; }

    /// <summary>
    /// Tax amount for this category.
    /// </summary>
    [JsonPropertyName("taxAmount")]
    public decimal TaxAmount { get; set; }

    /// <summary>
    /// Tax category details.
    /// </summary>
    [JsonPropertyName("taxCategory")]
    public TaxCategory? TaxCategory { get; set; }

    /// <summary>
    /// Currency code.
    /// </summary>
    [JsonPropertyName("currencyId")]
    public string? CurrencyId { get; set; }
}

/// <summary>
/// UBL Tax Category.
/// </summary>
public sealed class TaxCategory
{
    /// <summary>
    /// Category ID (e.g., "S" for standard, "Z" for zero-rated).
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>
    /// Tax percentage.
    /// </summary>
    [JsonPropertyName("percent")]
    public decimal? Percent { get; set; }

    /// <summary>
    /// Tax scheme.
    /// </summary>
    [JsonPropertyName("taxScheme")]
    public TaxScheme? TaxScheme { get; set; }

    /// <summary>
    /// Tax exemption reason code.
    /// </summary>
    [JsonPropertyName("taxExemptionReasonCode")]
    public string? TaxExemptionReasonCode { get; set; }

    /// <summary>
    /// Tax exemption reason.
    /// </summary>
    [JsonPropertyName("taxExemptionReason")]
    public string? TaxExemptionReason { get; set; }
}

/// <summary>
/// UBL Tax Scheme.
/// </summary>
public sealed class TaxScheme
{
    /// <summary>
    /// Tax scheme ID (e.g., "VAT").
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>
    /// Tax scheme name.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }
}
