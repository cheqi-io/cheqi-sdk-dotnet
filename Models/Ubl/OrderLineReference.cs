using System.Text.Json.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Order Line Reference - a reference to an order line.
/// </summary>
public sealed class OrderLineReference
{
    /// <summary>
    /// Line identifier.
    /// </summary>
    [JsonPropertyName("lineId")]
    public string? LineId { get; set; }
}
