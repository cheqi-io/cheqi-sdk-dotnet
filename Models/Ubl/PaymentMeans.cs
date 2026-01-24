using System.Text.Json.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Payment Means - describes how payment is made.
/// </summary>
public sealed class PaymentMeans
{
    /// <summary>
    /// Payment means code (UNCL 4461).
    /// </summary>
    [JsonPropertyName("paymentMeansCode")]
    public string? PaymentMeansCode { get; set; }

    /// <summary>
    /// Payment means description.
    /// </summary>
    [JsonPropertyName("paymentMeansDescription")]
    public string? PaymentMeansDescription { get; set; }

    /// <summary>
    /// Payment ID or reference.
    /// </summary>
    [JsonPropertyName("paymentId")]
    public string? PaymentId { get; set; }

    /// <summary>
    /// Payment due date.
    /// </summary>
    [JsonPropertyName("paymentDueDate")]
    public DateTimeOffset? PaymentDueDate { get; set; }

    /// <summary>
    /// Instruction ID.
    /// </summary>
    [JsonPropertyName("instructionId")]
    public string? InstructionId { get; set; }

    /// <summary>
    /// Instruction note.
    /// </summary>
    [JsonPropertyName("instructionNote")]
    public string? InstructionNote { get; set; }

    /// <summary>
    /// Card account details.
    /// </summary>
    [JsonPropertyName("cardAccount")]
    public CardAccount? CardAccount { get; set; }

    /// <summary>
    /// Payee financial account.
    /// </summary>
    [JsonPropertyName("payeeFinancialAccount")]
    public FinancialAccount? PayeeFinancialAccount { get; set; }

    /// <summary>
    /// Payer financial account.
    /// </summary>
    [JsonPropertyName("payerFinancialAccount")]
    public FinancialAccount? PayerFinancialAccount { get; set; }
}

/// <summary>
/// UBL Card Account.
/// </summary>
public sealed class CardAccount
{
    /// <summary>
    /// Primary account number (masked).
    /// </summary>
    [JsonPropertyName("primaryAccountNumberId")]
    public string? PrimaryAccountNumberId { get; set; }

    /// <summary>
    /// Card network ID (e.g., "VISA").
    /// </summary>
    [JsonPropertyName("networkId")]
    public string? NetworkId { get; set; }

    /// <summary>
    /// Card type code.
    /// </summary>
    [JsonPropertyName("cardTypeCode")]
    public string? CardTypeCode { get; set; }

    /// <summary>
    /// Card holder name.
    /// </summary>
    [JsonPropertyName("holderName")]
    public string? HolderName { get; set; }

    /// <summary>
    /// Expiry date.
    /// </summary>
    [JsonPropertyName("expiryDate")]
    public string? ExpiryDate { get; set; }
}

/// <summary>
/// UBL Financial Account.
/// </summary>
public sealed class FinancialAccount
{
    /// <summary>
    /// Account ID (e.g., IBAN).
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>
    /// Account ID scheme (e.g., "IBAN").
    /// </summary>
    [JsonPropertyName("schemeId")]
    public string? SchemeId { get; set; }

    /// <summary>
    /// Account name.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// Financial institution branch.
    /// </summary>
    [JsonPropertyName("financialInstitutionBranch")]
    public FinancialInstitutionBranch? FinancialInstitutionBranch { get; set; }
}

/// <summary>
/// UBL Financial Institution Branch.
/// </summary>
public sealed class FinancialInstitutionBranch
{
    /// <summary>
    /// Branch ID (e.g., BIC/SWIFT).
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>
    /// Branch name.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }
}
