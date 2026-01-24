using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Payment Mandate - a mandate for direct debit payments.
/// </summary>
[XmlType(Namespace = UblNamespaces.Cac)]
public sealed class PaymentMandate
{
    /// <summary>
    /// Mandate identifier.
    /// </summary>
    [JsonPropertyName("id")]
    [XmlElement("ID", Namespace = UblNamespaces.Cbc)]
    public Identifier? Id { get; set; }

    /// <summary>
    /// Mandate type code.
    /// </summary>
    [JsonPropertyName("mandateTypeCode")]
    [XmlElement("MandateTypeCode", Namespace = UblNamespaces.Cbc)]
    public Code? MandateTypeCode { get; set; }

    /// <summary>
    /// Maximum number of payment instructions.
    /// </summary>
    [JsonPropertyName("maximumPaymentInstructionsNumeric")]
    [XmlElement("MaximumPaymentInstructionsNumeric", Namespace = UblNamespaces.Cbc)]
    public decimal? MaximumPaymentInstructionsNumeric { get; set; }

    /// <summary>
    /// Maximum paid amount.
    /// </summary>
    [JsonPropertyName("maximumPaidAmount")]
    [XmlElement("MaximumPaidAmount", Namespace = UblNamespaces.Cbc)]
    public Amount? MaximumPaidAmount { get; set; }

    /// <summary>
    /// Signature identifier.
    /// </summary>
    [JsonPropertyName("signatureId")]
    [XmlElement("SignatureID", Namespace = UblNamespaces.Cbc)]
    public Identifier? SignatureId { get; set; }

    /// <summary>
    /// Payer party.
    /// </summary>
    [JsonPropertyName("payerParty")]
    [XmlElement("PayerParty", Namespace = UblNamespaces.Cac)]
    public Party? PayerParty { get; set; }

    /// <summary>
    /// Payer financial account.
    /// </summary>
    [JsonPropertyName("payerFinancialAccount")]
    [XmlElement("PayerFinancialAccount", Namespace = UblNamespaces.Cac)]
    public FinancialAccount? PayerFinancialAccount { get; set; }

    /// <summary>
    /// Validity period.
    /// </summary>
    [JsonPropertyName("validityPeriod")]
    [XmlElement("ValidityPeriod", Namespace = UblNamespaces.Cac)]
    public Period? ValidityPeriod { get; set; }

    /// <summary>
    /// Payment reversal period.
    /// </summary>
    [JsonPropertyName("paymentReversalPeriod")]
    [XmlElement("PaymentReversalPeriod", Namespace = UblNamespaces.Cac)]
    public Period? PaymentReversalPeriod { get; set; }

    /// <summary>
    /// Clause.
    /// </summary>
    [JsonPropertyName("clause")]
    [XmlElement("Clause", Namespace = UblNamespaces.Cac)]
    public Clause? Clause { get; set; }
}
