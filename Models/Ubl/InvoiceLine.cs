using System.Text.Json.Serialization;
using Cheqi.Sdk.Models.Enums;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Invoice Line - a line item on an invoice or receipt.
/// </summary>
public sealed class InvoiceLine
{
    /// <summary>
    /// Line identifier.
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>
    /// Line note or comment.
    /// </summary>
    [JsonPropertyName("note")]
    public string? Note { get; set; }

    /// <summary>
    /// Quantity invoiced.
    /// </summary>
    [JsonPropertyName("invoicedQuantity")]
    public decimal InvoicedQuantity { get; set; }

    /// <summary>
    /// Unit code for quantity.
    /// </summary>
    [JsonPropertyName("invoicedQuantityUnitCode")]
    public UnitCode? InvoicedQuantityUnitCode { get; set; }

    /// <summary>
    /// Line extension amount (quantity * price).
    /// </summary>
    [JsonPropertyName("lineExtensionAmount")]
    public decimal LineExtensionAmount { get; set; }

    /// <summary>
    /// Currency code.
    /// </summary>
    [JsonPropertyName("currencyId")]
    public string? CurrencyId { get; set; }

    /// <summary>
    /// Accounting cost code.
    /// </summary>
    [JsonPropertyName("accountingCost")]
    public string? AccountingCost { get; set; }

    /// <summary>
    /// Order line reference.
    /// </summary>
    [JsonPropertyName("orderLineReference")]
    public string? OrderLineReference { get; set; }

    /// <summary>
    /// Document reference (e.g., delivery note).
    /// </summary>
    [JsonPropertyName("documentReference")]
    public string? DocumentReference { get; set; }

    /// <summary>
    /// Allowances and charges for this line.
    /// </summary>
    [JsonPropertyName("allowanceCharges")]
    public List<AllowanceCharge>? AllowanceCharges { get; set; }

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
    /// Tax total for this line.
    /// </summary>
    [JsonPropertyName("taxTotal")]
    public TaxTotal? TaxTotal { get; set; }
}

/// <summary>
/// UBL Item - product or service details.
/// </summary>
public sealed class Item
{
    /// <summary>
    /// Item description.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Item name.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// Buyers item identification.
    /// </summary>
    [JsonPropertyName("buyersItemIdentification")]
    public string? BuyersItemIdentification { get; set; }

    /// <summary>
    /// Sellers item identification.
    /// </summary>
    [JsonPropertyName("sellersItemIdentification")]
    public string? SellersItemIdentification { get; set; }

    /// <summary>
    /// Standard item identification (GTIN/EAN).
    /// </summary>
    [JsonPropertyName("standardItemIdentification")]
    public string? StandardItemIdentification { get; set; }

    /// <summary>
    /// Standard item identification scheme (e.g., "GTIN").
    /// </summary>
    [JsonPropertyName("standardItemIdentificationScheme")]
    public string? StandardItemIdentificationScheme { get; set; }

    /// <summary>
    /// Origin country code.
    /// </summary>
    [JsonPropertyName("originCountry")]
    public string? OriginCountry { get; set; }

    /// <summary>
    /// Commodity classifications.
    /// </summary>
    [JsonPropertyName("commodityClassifications")]
    public List<CommodityClassification>? CommodityClassifications { get; set; }

    /// <summary>
    /// Classified tax category for this item.
    /// </summary>
    [JsonPropertyName("classifiedTaxCategory")]
    public TaxCategory? ClassifiedTaxCategory { get; set; }

    /// <summary>
    /// Additional item properties.
    /// </summary>
    [JsonPropertyName("additionalItemProperties")]
    public List<ItemProperty>? AdditionalItemProperties { get; set; }
}

/// <summary>
/// UBL Price information.
/// </summary>
public sealed class Price
{
    /// <summary>
    /// Price amount per unit.
    /// </summary>
    [JsonPropertyName("priceAmount")]
    public decimal PriceAmount { get; set; }

    /// <summary>
    /// Currency code.
    /// </summary>
    [JsonPropertyName("currencyId")]
    public string? CurrencyId { get; set; }

    /// <summary>
    /// Base quantity for the price.
    /// </summary>
    [JsonPropertyName("baseQuantity")]
    public decimal? BaseQuantity { get; set; }

    /// <summary>
    /// Unit code for base quantity.
    /// </summary>
    [JsonPropertyName("baseQuantityUnitCode")]
    public UnitCode? BaseQuantityUnitCode { get; set; }

    /// <summary>
    /// Allowance/charge applied to get this price.
    /// </summary>
    [JsonPropertyName("allowanceCharge")]
    public AllowanceCharge? AllowanceCharge { get; set; }
}

/// <summary>
/// UBL Allowance or Charge.
/// </summary>
public sealed class AllowanceCharge
{
    /// <summary>
    /// True for charge, false for allowance.
    /// </summary>
    [JsonPropertyName("chargeIndicator")]
    public bool ChargeIndicator { get; set; }

    /// <summary>
    /// Reason code.
    /// </summary>
    [JsonPropertyName("allowanceChargeReasonCode")]
    public string? AllowanceChargeReasonCode { get; set; }

    /// <summary>
    /// Reason description.
    /// </summary>
    [JsonPropertyName("allowanceChargeReason")]
    public string? AllowanceChargeReason { get; set; }

    /// <summary>
    /// Multiplier factor (percentage as decimal).
    /// </summary>
    [JsonPropertyName("multiplierFactorNumeric")]
    public decimal? MultiplierFactorNumeric { get; set; }

    /// <summary>
    /// Amount of the allowance or charge.
    /// </summary>
    [JsonPropertyName("amount")]
    public decimal Amount { get; set; }

    /// <summary>
    /// Currency code.
    /// </summary>
    [JsonPropertyName("currencyId")]
    public string? CurrencyId { get; set; }

    /// <summary>
    /// Base amount for percentage calculation.
    /// </summary>
    [JsonPropertyName("baseAmount")]
    public decimal? BaseAmount { get; set; }

    /// <summary>
    /// Tax category for this allowance/charge.
    /// </summary>
    [JsonPropertyName("taxCategory")]
    public TaxCategory? TaxCategory { get; set; }
}

/// <summary>
/// UBL Commodity Classification.
/// </summary>
public sealed class CommodityClassification
{
    /// <summary>
    /// Classification code.
    /// </summary>
    [JsonPropertyName("itemClassificationCode")]
    public string? ItemClassificationCode { get; set; }

    /// <summary>
    /// Classification list ID (e.g., "UNSPSC").
    /// </summary>
    [JsonPropertyName("listId")]
    public string? ListId { get; set; }

    /// <summary>
    /// Classification list version.
    /// </summary>
    [JsonPropertyName("listVersionId")]
    public string? ListVersionId { get; set; }
}

/// <summary>
/// UBL Item Property.
/// </summary>
public sealed class ItemProperty
{
    /// <summary>
    /// Property name.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// Property value.
    /// </summary>
    [JsonPropertyName("value")]
    public string? Value { get; set; }
}
