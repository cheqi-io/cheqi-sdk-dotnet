using System.Text.Json.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Credit Account - a credit account.
/// </summary>
public sealed class CreditAccount
{
    /// <summary>
    /// Account identifier.
    /// </summary>
    [JsonPropertyName("accountId")]
    public Identifier? AccountId { get; set; }
}
