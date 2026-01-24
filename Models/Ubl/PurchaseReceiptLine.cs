using System.Text.Json.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Purchase Receipt Line - a line item on a purchase receipt.
/// </summary>
public sealed class PurchaseReceiptLine
{
    /// <summary>
    /// Line identifier.
    /// </summary>
    [JsonPropertyName("id")]
    public Identifier? Id { get; set; }

    /// <summary>
    /// Universally unique identifier.
    /// </summary>
    [JsonPropertyName("uuid")]
    public Guid? Uuid { get; set; }

    /// <summary>
    /// Notes.
    /// </summary>
    [JsonPropertyName("note")]
    public List<string>? Note { get; set; }

    /// <summary>
    /// Quantity.
    /// </summary>
    [JsonPropertyName("quantity")]
    public Quantity? Quantity { get; set; }

    /// <summary>
    /// Line extension amount (quantity * price).
    /// </summary>
    [JsonPropertyName("lineExtensionAmount")]
    public Amount? LineExtensionAmount { get; set; }

    /// <summary>
    /// Tax inclusive line extension amount.
    /// </summary>
    [JsonPropertyName("taxInclusiveLineExtensionAmount")]
    public Amount? TaxInclusiveLineExtensionAmount { get; set; }

    /// <summary>
    /// Purchase line period.
    /// </summary>
    [JsonPropertyName("purchaseLinePeriod")]
    public Period? PurchaseLinePeriod { get; set; }

    /// <summary>
    /// Purchase reference.
    /// </summary>
    [JsonPropertyName("purchaseReference")]
    public PurchaseReference? PurchaseReference { get; set; }

    /// <summary>
    /// Allowances and charges.
    /// </summary>
    [JsonPropertyName("allowanceCharges")]
    public List<AllowanceCharge>? AllowanceCharges { get; set; }

    /// <summary>
    /// Tax totals.
    /// </summary>
    [JsonPropertyName("taxTotals")]
    public List<TaxTotal>? TaxTotals { get; set; }

    /// <summary>
    /// Item details.
    /// </summary>
    [JsonPropertyName("item")]
    public Item? Item { get; set; }

    /// <summary>
    /// Price information.
    /// </summary>
    [JsonPropertyName("price")]
    public Price? Price { get; set; }
}
