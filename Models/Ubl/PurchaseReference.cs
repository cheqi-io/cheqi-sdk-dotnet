using System.Text.Json.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Purchase Reference - a reference to a purchase.
/// </summary>
public sealed class PurchaseReference
{
    /// <summary>
    /// Purchase identifier.
    /// </summary>
    [JsonPropertyName("id")]
    public Identifier? Id { get; set; }

    /// <summary>
    /// Description.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }
}
