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
    public Amount? LineExtensionAmount { get; set; }

    /// <summary>
    /// Tax exclusive amount.
    /// </summary>
    [JsonPropertyName("taxExclusiveAmount")]
    [XmlElement("TaxExclusiveAmount", Namespace = UblNamespaces.Cbc)]
    public Amount? TaxExclusiveAmount { get; set; }

    /// <summary>
    /// Tax inclusive amount.
    /// </summary>
    [JsonPropertyName("taxInclusiveAmount")]
    [XmlElement("TaxInclusiveAmount", Namespace = UblNamespaces.Cbc)]
    public Amount? TaxInclusiveAmount { get; set; }

    /// <summary>
    /// Total allowance amount.
    /// </summary>
    [JsonPropertyName("allowanceTotalAmount")]
    [XmlElement("AllowanceTotalAmount", Namespace = UblNamespaces.Cbc)]
    public Amount? AllowanceTotalAmount { get; set; }

    /// <summary>
    /// Total charge amount.
    /// </summary>
    [JsonPropertyName("chargeTotalAmount")]
    [XmlElement("ChargeTotalAmount", Namespace = UblNamespaces.Cbc)]
    public Amount? ChargeTotalAmount { get; set; }

    /// <summary>
    /// Prepaid amount.
    /// </summary>
    [JsonPropertyName("prepaidAmount")]
    [XmlElement("PrepaidAmount", Namespace = UblNamespaces.Cbc)]
    public Amount? PrepaidAmount { get; set; }

    /// <summary>
    /// Payable rounding amount.
    /// </summary>
    [JsonPropertyName("payableRoundingAmount")]
    [XmlElement("PayableRoundingAmount", Namespace = UblNamespaces.Cbc)]
    public Amount? PayableRoundingAmount { get; set; }

    /// <summary>
    /// Amount payable.
    /// </summary>
    [JsonPropertyName("payableAmount")]
    [XmlElement("PayableAmount", Namespace = UblNamespaces.Cbc)]
    public Amount? PayableAmount { get; set; }
}
