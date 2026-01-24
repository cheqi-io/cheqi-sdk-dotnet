using System.Text.Json.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Payment - payment information.
/// </summary>
public sealed class Payment
{
    /// <summary>
    /// Payment identifier.
    /// </summary>
    [JsonPropertyName("id")]
    public Identifier? Id { get; set; }

    /// <summary>
    /// Paid amount.
    /// </summary>
    [JsonPropertyName("paidAmount")]
    public Amount? PaidAmount { get; set; }

    /// <summary>
    /// Paid cash amount.
    /// </summary>
    [JsonPropertyName("paidCashAmount")]
    public Amount? PaidCashAmount { get; set; }

    /// <summary>
    /// Cash change amount.
    /// </summary>
    [JsonPropertyName("cashChangeAmount")]
    public Amount? CashChangeAmount { get; set; }

    /// <summary>
    /// Received date.
    /// </summary>
    [JsonPropertyName("receivedDate")]
    public DateTimeOffset? ReceivedDate { get; set; }

    /// <summary>
    /// Paid date.
    /// </summary>
    [JsonPropertyName("paidDate")]
    public DateTimeOffset? PaidDate { get; set; }

    /// <summary>
    /// Paid time.
    /// </summary>
    [JsonPropertyName("paidTime")]
    public TimeSpan? PaidTime { get; set; }

    /// <summary>
    /// Instruction identifier.
    /// </summary>
    [JsonPropertyName("instructionId")]
    public Identifier? InstructionId { get; set; }

    /// <summary>
    /// Merchant identifier.
    /// </summary>
    [JsonPropertyName("merchantId")]
    public Identifier? MerchantId { get; set; }

    /// <summary>
    /// Authorization identifier.
    /// </summary>
    [JsonPropertyName("authorizationId")]
    public Identifier? AuthorizationId { get; set; }

    /// <summary>
    /// Transaction identifier.
    /// </summary>
    [JsonPropertyName("transactionId")]
    public Identifier? TransactionId { get; set; }

    /// <summary>
    /// Payment terminal identifier.
    /// </summary>
    [JsonPropertyName("paymentTerminalId")]
    public Identifier? PaymentTerminalId { get; set; }
}
