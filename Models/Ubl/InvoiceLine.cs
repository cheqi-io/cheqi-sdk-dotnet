using System.Text.Json.Serialization;
using System.Xml.Serialization;
using Cheqi.Sdk.Models.Enums;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Invoice Line - a line item on an invoice or receipt.
/// </summary>
[XmlType(Namespace = UblNamespaces.Cac)]
public sealed class InvoiceLine
{
    /// <summary>
    /// Line identifier.
    /// </summary>
    [JsonPropertyName("id")]
    [XmlElement("ID", Namespace = UblNamespaces.Cbc)]
    public string? Id { get; set; }

    /// <summary>
    /// Line note or comment.
    /// </summary>
    [JsonPropertyName("note")]
    [XmlElement("Note", Namespace = UblNamespaces.Cbc)]
    public string? Note { get; set; }

    /// <summary>
    /// Quantity invoiced.
    /// </summary>
    [JsonPropertyName("invoicedQuantity")]
    [XmlElement("InvoicedQuantity", Namespace = UblNamespaces.Cbc)]
    public decimal InvoicedQuantity { get; set; }

    /// <summary>
    /// Unit code for quantity.
    /// </summary>
    [JsonPropertyName("invoicedQuantityUnitCode")]
    [XmlIgnore]
    public UnitCode? InvoicedQuantityUnitCode { get; set; }

    /// <summary>
    /// Line extension amount (quantity * price).
    /// </summary>
    [JsonPropertyName("lineExtensionAmount")]
    [XmlElement("LineExtensionAmount", Namespace = UblNamespaces.Cbc)]
    public decimal LineExtensionAmount { get; set; }

    /// <summary>
    /// Currency code.
    /// </summary>
    [JsonPropertyName("currencyId")]
    [XmlIgnore]
    public string? CurrencyId { get; set; }

    /// <summary>
    /// Accounting cost code.
    /// </summary>
    [JsonPropertyName("accountingCost")]
    [XmlElement("AccountingCost", Namespace = UblNamespaces.Cbc)]
    public string? AccountingCost { get; set; }

    /// <summary>
    /// Order line reference.
    /// </summary>
    [JsonPropertyName("orderLineReference")]
    [XmlElement("OrderLineReference", Namespace = UblNamespaces.Cac)]
    public string? OrderLineReference { get; set; }

    /// <summary>
    /// Document reference (e.g., delivery note).
    /// </summary>
    [JsonPropertyName("documentReference")]
    [XmlElement("DocumentReference", Namespace = UblNamespaces.Cac)]
    public string? DocumentReference { get; set; }

    /// <summary>
    /// Allowances and charges for this line.
    /// </summary>
    [JsonPropertyName("allowanceCharges")]
    [XmlElement("AllowanceCharge", Namespace = UblNamespaces.Cac)]
    public List<AllowanceCharge>? AllowanceCharges { get; set; }

    /// <summary>
    /// Item details.
    /// </summary>
    [JsonPropertyName("item")]
    [XmlElement("Item", Namespace = UblNamespaces.Cac)]
    public Item? Item { get; set; }

    /// <summary>
    /// Price information.
    /// </summary>
    [JsonPropertyName("price")]
    [XmlElement("Price", Namespace = UblNamespaces.Cac)]
    public Price? Price { get; set; }

    /// <summary>
    /// Tax total for this line.
    /// </summary>
    [JsonPropertyName("taxTotal")]
    [XmlElement("TaxTotal", Namespace = UblNamespaces.Cac)]
    public TaxTotal? TaxTotal { get; set; }
}

/// <summary>
/// UBL Item - product or service details.
/// </summary>
[XmlType(Namespace = UblNamespaces.Cac)]
public sealed class Item
{
    /// <summary>
    /// Item description.
    /// </summary>
    [JsonPropertyName("description")]
    [XmlElement("Description", Namespace = UblNamespaces.Cbc)]
    public string? Description { get; set; }

    /// <summary>
    /// Item name.
    /// </summary>
    [JsonPropertyName("name")]
    [XmlElement("Name", Namespace = UblNamespaces.Cbc)]
    public string? Name { get; set; }

    /// <summary>
    /// Buyers item identification.
    /// </summary>
    [JsonPropertyName("buyersItemIdentification")]
    [XmlElement("BuyersItemIdentification", Namespace = UblNamespaces.Cac)]
    public string? BuyersItemIdentification { get; set; }

    /// <summary>
    /// Sellers item identification.
    /// </summary>
    [JsonPropertyName("sellersItemIdentification")]
    [XmlElement("SellersItemIdentification", Namespace = UblNamespaces.Cac)]
    public string? SellersItemIdentification { get; set; }

    /// <summary>
    /// Standard item identification (GTIN/EAN).
    /// </summary>
    [JsonPropertyName("standardItemIdentification")]
    [XmlElement("StandardItemIdentification", Namespace = UblNamespaces.Cac)]
    public string? StandardItemIdentification { get; set; }

    /// <summary>
    /// Standard item identification scheme (e.g., "GTIN").
    /// </summary>
    [JsonPropertyName("standardItemIdentificationScheme")]
    [XmlIgnore]
    public string? StandardItemIdentificationScheme { get; set; }

    /// <summary>
    /// Origin country code.
    /// </summary>
    [JsonPropertyName("originCountry")]
    [XmlElement("OriginCountry", Namespace = UblNamespaces.Cac)]
    public string? OriginCountry { get; set; }

    /// <summary>
    /// Commodity classifications.
    /// </summary>
    [JsonPropertyName("commodityClassifications")]
    [XmlElement("CommodityClassification", Namespace = UblNamespaces.Cac)]
    public List<CommodityClassification>? CommodityClassifications { get; set; }

    /// <summary>
    /// Classified tax category for this item.
    /// </summary>
    [JsonPropertyName("classifiedTaxCategory")]
    [XmlElement("ClassifiedTaxCategory", Namespace = UblNamespaces.Cac)]
    public TaxCategory? ClassifiedTaxCategory { get; set; }

    /// <summary>
    /// Additional item properties.
    /// </summary>
    [JsonPropertyName("additionalItemProperties")]
    [XmlElement("AdditionalItemProperty", Namespace = UblNamespaces.Cac)]
    public List<ItemProperty>? AdditionalItemProperties { get; set; }
}

/// <summary>
/// UBL Price information.
/// </summary>
[XmlType(Namespace = UblNamespaces.Cac)]
public sealed class Price
{
    /// <summary>
    /// Price amount per unit.
    /// </summary>
    [JsonPropertyName("priceAmount")]
    [XmlElement("PriceAmount", Namespace = UblNamespaces.Cbc)]
    public decimal PriceAmount { get; set; }

    /// <summary>
    /// Currency code.
    /// </summary>
    [JsonPropertyName("currencyId")]
    [XmlIgnore]
    public string? CurrencyId { get; set; }

    /// <summary>
    /// Base quantity for the price.
    /// </summary>
    [JsonPropertyName("baseQuantity")]
    [XmlElement("BaseQuantity", Namespace = UblNamespaces.Cbc)]
    public decimal? BaseQuantity { get; set; }

    /// <summary>
    /// Unit code for base quantity.
    /// </summary>
    [JsonPropertyName("baseQuantityUnitCode")]
    [XmlIgnore]
    public UnitCode? BaseQuantityUnitCode { get; set; }

    /// <summary>
    /// Allowance/charge applied to get this price.
    /// </summary>
    [JsonPropertyName("allowanceCharge")]
    [XmlElement("AllowanceCharge", Namespace = UblNamespaces.Cac)]
    public AllowanceCharge? AllowanceCharge { get; set; }
}

/// <summary>
/// UBL Allowance or Charge.
/// </summary>
[XmlType(Namespace = UblNamespaces.Cac)]
public sealed class AllowanceCharge
{
    /// <summary>
    /// True for charge, false for allowance.
    /// </summary>
    [JsonPropertyName("chargeIndicator")]
    [XmlElement("ChargeIndicator", Namespace = UblNamespaces.Cbc)]
    public bool ChargeIndicator { get; set; }

    /// <summary>
    /// Reason code.
    /// </summary>
    [JsonPropertyName("allowanceChargeReasonCode")]
    [XmlElement("AllowanceChargeReasonCode", Namespace = UblNamespaces.Cbc)]
    public string? AllowanceChargeReasonCode { get; set; }

    /// <summary>
    /// Reason description.
    /// </summary>
    [JsonPropertyName("allowanceChargeReason")]
    [XmlElement("AllowanceChargeReason", Namespace = UblNamespaces.Cbc)]
    public string? AllowanceChargeReason { get; set; }

    /// <summary>
    /// Multiplier factor (percentage as decimal).
    /// </summary>
    [JsonPropertyName("multiplierFactorNumeric")]
    [XmlElement("MultiplierFactorNumeric", Namespace = UblNamespaces.Cbc)]
    public decimal? MultiplierFactorNumeric { get; set; }

    /// <summary>
    /// Amount of the allowance or charge.
    /// </summary>
    [JsonPropertyName("amount")]
    [XmlElement("Amount", Namespace = UblNamespaces.Cbc)]
    public decimal Amount { get; set; }

    /// <summary>
    /// Currency code.
    /// </summary>
    [JsonPropertyName("currencyId")]
    [XmlIgnore]
    public string? CurrencyId { get; set; }

    /// <summary>
    /// Base amount for percentage calculation.
    /// </summary>
    [JsonPropertyName("baseAmount")]
    [XmlElement("BaseAmount", Namespace = UblNamespaces.Cbc)]
    public decimal? BaseAmount { get; set; }

    /// <summary>
    /// Tax category for this allowance/charge.
    /// </summary>
    [JsonPropertyName("taxCategory")]
    [XmlElement("TaxCategory", Namespace = UblNamespaces.Cac)]
    public TaxCategory? TaxCategory { get; set; }
}

/// <summary>
/// UBL Commodity Classification.
/// </summary>
[XmlType(Namespace = UblNamespaces.Cac)]
public sealed class CommodityClassification
{
    /// <summary>
    /// Classification code.
    /// </summary>
    [JsonPropertyName("itemClassificationCode")]
    [XmlElement("ItemClassificationCode", Namespace = UblNamespaces.Cbc)]
    public string? ItemClassificationCode { get; set; }

    /// <summary>
    /// Classification list ID (e.g., "UNSPSC").
    /// </summary>
    [JsonPropertyName("listId")]
    [XmlIgnore]
    public string? ListId { get; set; }

    /// <summary>
    /// Classification list version.
    /// </summary>
    [JsonPropertyName("listVersionId")]
    [XmlIgnore]
    public string? ListVersionId { get; set; }
}

/// <summary>
/// UBL Item Property.
/// </summary>
[XmlType(Namespace = UblNamespaces.Cac)]
public sealed class ItemProperty
{
    /// <summary>
    /// Property name.
    /// </summary>
    [JsonPropertyName("name")]
    [XmlElement("Name", Namespace = UblNamespaces.Cbc)]
    public string? Name { get; set; }

    /// <summary>
    /// Property value.
    /// </summary>
    [JsonPropertyName("value")]
    [XmlElement("Value", Namespace = UblNamespaces.Cbc)]
    public string? Value { get; set; }
}
