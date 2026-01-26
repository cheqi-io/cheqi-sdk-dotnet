using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Allowance Charge - represents discounts or additional charges.
/// </summary>
[XmlType(Namespace = UblNamespaces.Cac)]
public sealed class AllowanceCharge
{
    /// <summary>
    /// Allowance/Charge ID.
    /// </summary>
    [JsonPropertyName("id")]
    [XmlElement("ID", Namespace = UblNamespaces.Cbc)]
    public Identifier? Id { get; set; }

    /// <summary>
    /// Indicates if this is a charge (true) or an allowance/discount (false).
    /// </summary>
    [JsonPropertyName("chargeIndicator")]
    [XmlElement("ChargeIndicator", Namespace = UblNamespaces.Cbc)]
    public bool? ChargeIndicator { get; set; }

    /// <summary>
    /// Reason code for the allowance or charge.
    /// </summary>
    [JsonPropertyName("allowanceChargeReasonCode")]
    [XmlElement("AllowanceChargeReasonCode", Namespace = UblNamespaces.Cbc)]
    public Code? AllowanceChargeReasonCode { get; set; }

    /// <summary>
    /// Reason text for the allowance or charge.
    /// </summary>
    [JsonPropertyName("allowanceChargeReason")]
    [XmlElement("AllowanceChargeReason", Namespace = UblNamespaces.Cbc)]
    public List<string>? AllowanceChargeReason { get; set; }

    /// <summary>
    /// Multiplier factor (percentage as decimal, e.g., 0.21 for 21%).
    /// </summary>
    [JsonPropertyName("multiplierFactorNumeric")]
    [XmlElement("MultiplierFactorNumeric", Namespace = UblNamespaces.Cbc)]
    public double? MultiplierFactorNumeric { get; set; }

    /// <summary>
    /// Prepaid indicator.
    /// </summary>
    [JsonPropertyName("prepaidIndicator")]
    [XmlElement("PrepaidIndicator", Namespace = UblNamespaces.Cbc)]
    public bool? PrepaidIndicator { get; set; }

    /// <summary>
    /// Sequence number.
    /// </summary>
    [JsonPropertyName("sequenceNumeric")]
    [XmlElement("SequenceNumeric", Namespace = UblNamespaces.Cbc)]
    public int? SequenceNumeric { get; set; }

    /// <summary>
    /// Amount of the allowance or charge.
    /// </summary>
    [JsonPropertyName("amount")]
    [XmlElement("Amount", Namespace = UblNamespaces.Cbc)]
    public Amount? Amount { get; set; }

    /// <summary>
    /// Tax inclusive amount.
    /// </summary>
    [JsonPropertyName("taxInclusiveAmount")]
    [XmlElement("TaxInclusiveAmount", Namespace = UblNamespaces.Cbc)]
    public Amount? TaxInclusiveAmount { get; set; }

    /// <summary>
    /// Base amount for calculation.
    /// </summary>
    [JsonPropertyName("baseAmount")]
    [XmlElement("BaseAmount", Namespace = UblNamespaces.Cbc)]
    public Amount? BaseAmount { get; set; }

    /// <summary>
    /// Accounting cost code.
    /// </summary>
    [JsonPropertyName("accountingCostCode")]
    [XmlElement("AccountingCostCode", Namespace = UblNamespaces.Cbc)]
    public Code? AccountingCostCode { get; set; }

    /// <summary>
    /// Accounting cost.
    /// </summary>
    [JsonPropertyName("accountingCost")]
    [XmlElement("AccountingCost", Namespace = UblNamespaces.Cbc)]
    public string? AccountingCost { get; set; }

    /// <summary>
    /// Per unit amount.
    /// </summary>
    [JsonPropertyName("perUnitAmount")]
    [XmlElement("PerUnitAmount", Namespace = UblNamespaces.Cbc)]
    public Amount? PerUnitAmount { get; set; }

    /// <summary>
    /// Tax category.
    /// </summary>
    [JsonPropertyName("taxCategory")]
    [XmlElement("TaxCategory", Namespace = UblNamespaces.Cac)]
    public TaxCategory? TaxCategory { get; set; }

    /// <summary>
    /// Tax total.
    /// </summary>
    [JsonPropertyName("taxTotal")]
    [XmlElement("TaxTotal", Namespace = UblNamespaces.Cac)]
    public TaxTotal? TaxTotal { get; set; }

    /// <summary>
    /// Payment means.
    /// </summary>
    [JsonPropertyName("paymentMeans")]
    [XmlElement("PaymentMeans", Namespace = UblNamespaces.Cac)]
    public List<PaymentMeans>? PaymentMeans { get; set; }
}