using System.Text.Json.Serialization;

namespace Cheqi.Sdk.Models;

/// <summary>
/// Tax information for a receipt item or total.
/// </summary>
public sealed class Tax
{
    /// <summary>
    /// Tax category code (e.g., "S" for standard rate).
    /// </summary>
    [JsonPropertyName("categoryCode")]
    public string? CategoryCode { get; set; }

    /// <summary>
    /// Tax percentage rate.
    /// </summary>
    [JsonPropertyName("percent")]
    public decimal? Percent { get; set; }

    /// <summary>
    /// Tax amount.
    /// </summary>
    [JsonPropertyName("taxAmount")]
    public decimal? TaxAmount { get; set; }

    /// <summary>
    /// Taxable amount (base for tax calculation).
    /// </summary>
    [JsonPropertyName("taxableAmount")]
    public decimal? TaxableAmount { get; set; }

    /// <summary>
    /// Tax exemption reason code.
    /// </summary>
    [JsonPropertyName("exemptionReasonCode")]
    public string? ExemptionReasonCode { get; set; }

    /// <summary>
    /// Tax exemption reason text.
    /// </summary>
    [JsonPropertyName("exemptionReason")]
    public string? ExemptionReason { get; set; }
}
