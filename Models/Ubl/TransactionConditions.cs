using System.Text.Json.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Transaction Conditions - conditions for a transaction.
/// </summary>
public sealed class TransactionConditions
{
    /// <summary>
    /// Condition identifier.
    /// </summary>
    [JsonPropertyName("id")]
    public Identifier? Id { get; set; }

    /// <summary>
    /// Action code.
    /// </summary>
    [JsonPropertyName("actionCode")]
    public Code? ActionCode { get; set; }

    /// <summary>
    /// Descriptions.
    /// </summary>
    [JsonPropertyName("description")]
    public List<string>? Description { get; set; }

    /// <summary>
    /// Document references.
    /// </summary>
    [JsonPropertyName("documentReference")]
    public List<DocumentReference>? DocumentReference { get; set; }
}
