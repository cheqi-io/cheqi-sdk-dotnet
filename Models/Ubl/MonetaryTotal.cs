using System.Text.Json.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Monetary Total - represents the totals for an invoice or receipt.
/// </summary>
public sealed class MonetaryTotal
{
    /// <summary>
    /// Sum of all line extension amounts.
    /// </summary>
    [JsonPropertyName("lineExtensionAmount")]
    public decimal? LineExtensionAmount { get; set; }

    /// <summary>
    /// Tax exclusive amount.
    /// </summary>
    [JsonPropertyName("taxExclusiveAmount")]
    public decimal? TaxExclusiveAmount { get; set; }

    /// <summary>
    /// Tax inclusive amount.
    /// </summary>
    [JsonPropertyName("taxInclusiveAmount")]
    public decimal? TaxInclusiveAmount { get; set; }

    /// <summary>
    /// Total allowance amount.
    /// </summary>
    [JsonPropertyName("allowanceTotalAmount")]
    public decimal? AllowanceTotalAmount { get; set; }

    /// <summary>
    /// Total charge amount.
    /// </summary>
    [JsonPropertyName("chargeTotalAmount")]
    public decimal? ChargeTotalAmount { get; set; }

    /// <summary>
    /// Prepaid amount.
    /// </summary>
    [JsonPropertyName("prepaidAmount")]
    public decimal? PrepaidAmount { get; set; }

    /// <summary>
    /// Payable rounding amount.
    /// </summary>
    [JsonPropertyName("payableRoundingAmount")]
    public decimal? PayableRoundingAmount { get; set; }

    /// <summary>
    /// Amount payable.
    /// </summary>
    [JsonPropertyName("payableAmount")]
    public decimal? PayableAmount { get; set; }

    /// <summary>
    /// Currency code.
    /// </summary>
    [JsonPropertyName("currencyId")]
    public string? CurrencyId { get; set; }
}
