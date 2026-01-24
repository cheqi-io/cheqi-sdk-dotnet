using System.Text.Json.Serialization;
using Cheqi.Sdk.Models.Enums;

namespace Cheqi.Sdk.Models;

/// <summary>
/// A product or line item on a receipt.
/// </summary>
public sealed class Product
{
    /// <summary>
    /// Product name or description.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Product description.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Product SKU or identifier.
    /// </summary>
    [JsonPropertyName("sku")]
    public string? Sku { get; set; }

    /// <summary>
    /// EAN/GTIN barcode.
    /// </summary>
    [JsonPropertyName("ean")]
    public string? Ean { get; set; }

    /// <summary>
    /// Quantity purchased.
    /// </summary>
    [JsonPropertyName("quantity")]
    public decimal Quantity { get; set; } = 1;

    /// <summary>
    /// Unit of measurement.
    /// </summary>
    [JsonPropertyName("unitCode")]
    public UnitCode UnitCode { get; set; } = UnitCode.EA;

    /// <summary>
    /// Unit price (before tax).
    /// </summary>
    [JsonPropertyName("unitPrice")]
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Line total (quantity * unit price).
    /// </summary>
    [JsonPropertyName("lineTotal")]
    public decimal? LineTotal { get; set; }

    /// <summary>
    /// Line total including tax.
    /// </summary>
    [JsonPropertyName("lineTotalIncludingTax")]
    public decimal? LineTotalIncludingTax { get; set; }

    /// <summary>
    /// Tax details for this line item.
    /// </summary>
    [JsonPropertyName("tax")]
    public Tax? Tax { get; set; }

    /// <summary>
    /// Discount amount applied to this item.
    /// </summary>
    [JsonPropertyName("discountAmount")]
    public decimal? DiscountAmount { get; set; }

    /// <summary>
    /// Discount percentage applied to this item.
    /// </summary>
    [JsonPropertyName("discountPercent")]
    public decimal? DiscountPercent { get; set; }

    /// <summary>
    /// Product category.
    /// </summary>
    [JsonPropertyName("category")]
    public string? Category { get; set; }

    /// <summary>
    /// Product image URL.
    /// </summary>
    [JsonPropertyName("imageUrl")]
    public string? ImageUrl { get; set; }
}
