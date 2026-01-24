using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Monetary Total - represents the totals for an invoice or receipt.
/// </summary>
[XmlType(Namespace = UblNamespaces.Cac)]
public sealed class MonetaryTotal
{
    /// <summary>
    /// Sum of all line extension amounts.
    /// </summary>
    [JsonPropertyName("lineExtensionAmount")]
    [XmlElement("LineExtensionAmount", Namespace = UblNamespaces.Cbc)]
    public decimal? LineExtensionAmount { get; set; }

    /// <summary>
    /// Tax exclusive amount.
    /// </summary>
    [JsonPropertyName("taxExclusiveAmount")]
    [XmlElement("TaxExclusiveAmount", Namespace = UblNamespaces.Cbc)]
    public decimal? TaxExclusiveAmount { get; set; }

    /// <summary>
    /// Tax inclusive amount.
    /// </summary>
    [JsonPropertyName("taxInclusiveAmount")]
    [XmlElement("TaxInclusiveAmount", Namespace = UblNamespaces.Cbc)]
    public decimal? TaxInclusiveAmount { get; set; }

    /// <summary>
    /// Total allowance amount.
    /// </summary>
    [JsonPropertyName("allowanceTotalAmount")]
    [XmlElement("AllowanceTotalAmount", Namespace = UblNamespaces.Cbc)]
    public decimal? AllowanceTotalAmount { get; set; }

    /// <summary>
    /// Total charge amount.
    /// </summary>
    [JsonPropertyName("chargeTotalAmount")]
    [XmlElement("ChargeTotalAmount", Namespace = UblNamespaces.Cbc)]
    public decimal? ChargeTotalAmount { get; set; }

    /// <summary>
    /// Prepaid amount.
    /// </summary>
    [JsonPropertyName("prepaidAmount")]
    [XmlElement("PrepaidAmount", Namespace = UblNamespaces.Cbc)]
    public decimal? PrepaidAmount { get; set; }

    /// <summary>
    /// Payable rounding amount.
    /// </summary>
    [JsonPropertyName("payableRoundingAmount")]
    [XmlElement("PayableRoundingAmount", Namespace = UblNamespaces.Cbc)]
    public decimal? PayableRoundingAmount { get; set; }

    /// <summary>
    /// Amount payable.
    /// </summary>
    [JsonPropertyName("payableAmount")]
    [XmlElement("PayableAmount", Namespace = UblNamespaces.Cbc)]
    public decimal? PayableAmount { get; set; }

    /// <summary>
    /// Currency code.
    /// </summary>
    [JsonPropertyName("currencyId")]
    [XmlIgnore]
    public string? CurrencyId { get; set; }
}
