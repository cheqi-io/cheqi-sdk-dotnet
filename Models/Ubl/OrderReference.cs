using System.Text.Json.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Order Reference - a reference to an order.
/// </summary>
public sealed class OrderReference
{
    /// <summary>
    /// Order identifier.
    /// </summary>
    [JsonPropertyName("id")]
    public Identifier? Id { get; set; }

    /// <summary>
    /// Sales order ID.
    /// </summary>
    [JsonPropertyName("salesOrderId")]
    public string? SalesOrderId { get; set; }
}
