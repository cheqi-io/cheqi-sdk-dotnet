using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Order Reference - a reference to an order.
/// </summary>
[XmlType(Namespace = UblNamespaces.Cac)]
public sealed class OrderReference
{
    /// <summary>
    /// Order identifier.
    /// </summary>
    [JsonPropertyName("id")]
    [XmlElement("ID", Namespace = UblNamespaces.Cbc)]
    public Identifier? Id { get; set; }

    /// <summary>
    /// Sales order ID.
    /// </summary>
    [JsonPropertyName("salesOrderId")]
    [XmlElement("SalesOrderID", Namespace = UblNamespaces.Cbc)]
    public string? SalesOrderId { get; set; }
}
