using System.Text.Json.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Credit Note Line - a line item on a credit note.
/// </summary>
public sealed class CreditNoteLine
{
    /// <summary>
    /// Line identifier.
    /// </summary>
    [JsonPropertyName("id")]
    public Identifier? Id { get; set; }

    /// <summary>
    /// Credited quantity.
    /// </summary>
    [JsonPropertyName("creditedQuantity")]
    public Quantity? CreditedQuantity { get; set; }

    /// <summary>
    /// Line extension amount.
    /// </summary>
    [JsonPropertyName("lineExtensionAmount")]
    public Amount? LineExtensionAmount { get; set; }

    /// <summary>
    /// Invoice period.
    /// </summary>
    [JsonPropertyName("invoicePeriod")]
    public Period? InvoicePeriod { get; set; }

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

    /// <summary>
    /// Note.
    /// </summary>
    [JsonPropertyName("note")]
    public string? Note { get; set; }

    /// <summary>
    /// Tax totals.
    /// </summary>
    [JsonPropertyName("taxTotal")]
    public List<TaxTotal>? TaxTotal { get; set; }
}
