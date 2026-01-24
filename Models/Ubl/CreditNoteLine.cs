using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Credit Note Line - a line item on a credit note.
/// </summary>
[XmlType(Namespace = UblNamespaces.Cac)]
public sealed class CreditNoteLine
{
    /// <summary>
    /// Line identifier.
    /// </summary>
    [JsonPropertyName("id")]
    [XmlElement("ID", Namespace = UblNamespaces.Cbc)]
    public Identifier? Id { get; set; }

    /// <summary>
    /// Credited quantity.
    /// </summary>
    [JsonPropertyName("creditedQuantity")]
    [XmlElement("CreditedQuantity", Namespace = UblNamespaces.Cbc)]
    public Quantity? CreditedQuantity { get; set; }

    /// <summary>
    /// Line extension amount.
    /// </summary>
    [JsonPropertyName("lineExtensionAmount")]
    [XmlElement("LineExtensionAmount", Namespace = UblNamespaces.Cbc)]
    public Amount? LineExtensionAmount { get; set; }

    /// <summary>
    /// Invoice period.
    /// </summary>
    [JsonPropertyName("invoicePeriod")]
    [XmlElement("InvoicePeriod", Namespace = UblNamespaces.Cac)]
    public Period? InvoicePeriod { get; set; }

    /// <summary>
    /// Item details.
    /// </summary>
    [JsonPropertyName("item")]
    [XmlElement("Item", Namespace = UblNamespaces.Cac)]
    public Item? Item { get; set; }

    /// <summary>
    /// Price information.
    /// </summary>
    [JsonPropertyName("price")]
    [XmlElement("Price", Namespace = UblNamespaces.Cac)]
    public Price? Price { get; set; }

    /// <summary>
    /// Note.
    /// </summary>
    [JsonPropertyName("note")]
    [XmlElement("Note", Namespace = UblNamespaces.Cbc)]
    public string? Note { get; set; }

    /// <summary>
    /// Tax totals.
    /// </summary>
    [JsonPropertyName("taxTotal")]
    [XmlElement("TaxTotal", Namespace = UblNamespaces.Cac)]
    public List<TaxTotal>? TaxTotal { get; set; }
}
