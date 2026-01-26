using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Price - represents pricing information for an item.
/// </summary>
[XmlType(Namespace = UblNamespaces.Cac)]
public sealed class Price
{
    /// <summary>
    /// Price amount (required).
    /// </summary>
    [JsonPropertyName("priceAmount")]
    [XmlElement("PriceAmount", Namespace = UblNamespaces.Cbc)]
    public Amount? PriceAmount { get; set; }

    /// <summary>
    /// Tax inclusive price amount.
    /// </summary>
    [JsonPropertyName("taxInclusivePriceAmount")]
    [XmlElement("TaxInclusivePriceAmount", Namespace = UblNamespaces.Cbc)]
    public Amount? TaxInclusivePriceAmount { get; set; }

    /// <summary>
    /// Base quantity.
    /// </summary>
    [JsonPropertyName("baseQuantity")]
    [XmlElement("BaseQuantity", Namespace = UblNamespaces.Cbc)]
    public Quantity? BaseQuantity { get; set; }

    /// <summary>
    /// Price change reasons.
    /// </summary>
    [JsonPropertyName("priceChangeReason")]
    [XmlElement("PriceChangeReason", Namespace = UblNamespaces.Cbc)]
    public List<string>? PriceChangeReason { get; set; }

    /// <summary>
    /// Price type code.
    /// </summary>
    [JsonPropertyName("priceTypeCode")]
    [XmlElement("PriceTypeCode", Namespace = UblNamespaces.Cbc)]
    public Code? PriceTypeCode { get; set; }

    /// <summary>
    /// Price type.
    /// </summary>
    [JsonPropertyName("priceType")]
    [XmlElement("PriceType", Namespace = UblNamespaces.Cbc)]
    public string? PriceType { get; set; }

    /// <summary>
    /// Orderable unit factor rate.
    /// </summary>
    [JsonPropertyName("orderableUnitFactorRate")]
    [XmlElement("OrderableUnitFactorRate", Namespace = UblNamespaces.Cbc)]
    public decimal? OrderableUnitFactorRate { get; set; }

    /// <summary>
    /// Validity periods.
    /// </summary>
    [JsonPropertyName("validityPeriod")]
    [XmlElement("ValidityPeriod", Namespace = UblNamespaces.Cac)]
    public List<Period>? ValidityPeriod { get; set; }

    /// <summary>
    /// Price list.
    /// </summary>
    [JsonPropertyName("priceList")]
    [XmlElement("PriceList", Namespace = UblNamespaces.Cac)]
    public PriceList? PriceList { get; set; }

    /// <summary>
    /// Allowances and charges.
    /// </summary>
    [JsonPropertyName("allowanceCharge")]
    [XmlElement("AllowanceCharge", Namespace = UblNamespaces.Cac)]
    public List<AllowanceCharge>? AllowanceCharge { get; set; }
}
