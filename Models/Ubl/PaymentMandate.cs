using System.Text.Json.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Payment Mandate - a mandate for direct debit payments.
/// </summary>
public sealed class PaymentMandate
{
    /// <summary>
    /// Mandate identifier.
    /// </summary>
    [JsonPropertyName("id")]
    public Identifier? Id { get; set; }

    /// <summary>
    /// Mandate type code.
    /// </summary>
    [JsonPropertyName("mandateTypeCode")]
    public Code? MandateTypeCode { get; set; }

    /// <summary>
    /// Maximum number of payment instructions.
    /// </summary>
    [JsonPropertyName("maximumPaymentInstructionsNumeric")]
    public decimal? MaximumPaymentInstructionsNumeric { get; set; }

    /// <summary>
    /// Maximum paid amount.
    /// </summary>
    [JsonPropertyName("maximumPaidAmount")]
    public Amount? MaximumPaidAmount { get; set; }

    /// <summary>
    /// Signature identifier.
    /// </summary>
    [JsonPropertyName("signatureId")]
    public Identifier? SignatureId { get; set; }

    /// <summary>
    /// Payer party.
    /// </summary>
    [JsonPropertyName("payerParty")]
    public Party? PayerParty { get; set; }

    /// <summary>
    /// Payer financial account.
    /// </summary>
    [JsonPropertyName("payerFinancialAccount")]
    public FinancialAccount? PayerFinancialAccount { get; set; }

    /// <summary>
    /// Validity period.
    /// </summary>
    [JsonPropertyName("validityPeriod")]
    public Period? ValidityPeriod { get; set; }

    /// <summary>
    /// Payment reversal period.
    /// </summary>
    [JsonPropertyName("paymentReversalPeriod")]
    public Period? PaymentReversalPeriod { get; set; }

    /// <summary>
    /// Clause.
    /// </summary>
    [JsonPropertyName("clause")]
    public Clause? Clause { get; set; }
}
