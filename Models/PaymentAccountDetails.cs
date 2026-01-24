using System.Text.Json.Serialization;

namespace Cheqi.Sdk.Models;

/// <summary>
/// Details of a payment account (bank account) for customer identification.
/// </summary>
public sealed class PaymentAccountDetails
{
    /// <summary>
    /// International Bank Account Number.
    /// </summary>
    [JsonPropertyName("iban")]
    public string? Iban { get; set; }

    /// <summary>
    /// Bank Identifier Code (SWIFT/BIC).
    /// </summary>
    [JsonPropertyName("bic")]
    public string? Bic { get; set; }

    /// <summary>
    /// Account holder name.
    /// </summary>
    [JsonPropertyName("accountHolderName")]
    public string? AccountHolderName { get; set; }

    /// <summary>
    /// Hash of the account number for matching.
    /// </summary>
    [JsonPropertyName("accountHash")]
    public string? AccountHash { get; set; }
}
