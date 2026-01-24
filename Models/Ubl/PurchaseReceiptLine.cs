using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Purchase Receipt Line - a line item on a purchase receipt.
/// </summary>
[XmlType(Namespace = UblNamespaces.Cac)]
public sealed class PurchaseReceiptLine
{
    /// <summary>
    /// Line identifier.
    /// </summary>
    [JsonPropertyName("id")]
    [XmlElement("ID", Namespace = UblNamespaces.Cbc)]
    public Identifier? Id { get; set; }

    /// <summary>
    /// Universally unique identifier.
    /// </summary>
    [JsonPropertyName("uuid")]
    [XmlElement("UUID", Namespace = UblNamespaces.Cbc)]
    public Guid? Uuid { get; set; }

    /// <summary>
    /// Notes.
    /// </summary>
    [JsonPropertyName("note")]
    [XmlElement("Note", Namespace = UblNamespaces.Cbc)]
    public List<string>? Note { get; set; }

    /// <summary>
    /// Quantity.
    /// </summary>
    [JsonPropertyName("quantity")]
    [XmlElement("Quantity", Namespace = UblNamespaces.Cbc)]
    public Quantity? Quantity { get; set; }

    /// <summary>
    /// Line extension amount (quantity * price).
    /// </summary>
    [JsonPropertyName("lineExtensionAmount")]
    [XmlElement("LineExtensionAmount", Namespace = UblNamespaces.Cbc)]
    public Amount? LineExtensionAmount { get; set; }

    /// <summary>
    /// Tax inclusive line extension amount.
    /// </summary>
    [JsonPropertyName("taxInclusiveLineExtensionAmount")]
    [XmlElement("TaxInclusiveLineExtensionAmount", Namespace = UblNamespaces.Cbc)]
    public Amount? TaxInclusiveLineExtensionAmount { get; set; }

    /// <summary>
    /// Purchase line period.
    /// </summary>
    [JsonPropertyName("purchaseLinePeriod")]
    [XmlElement("PurchaseLinePeriod", Namespace = UblNamespaces.Cac)]
    public Period? PurchaseLinePeriod { get; set; }

    /// <summary>
    /// Purchase reference.
    /// </summary>
    [JsonPropertyName("purchaseReference")]
    [XmlElement("PurchaseReference", Namespace = UblNamespaces.Cac)]
    public PurchaseReference? PurchaseReference { get; set; }

    /// <summary>
    /// Allowances and charges.
    /// </summary>
    [JsonPropertyName("allowanceCharges")]
    [XmlElement("AllowanceCharge", Namespace = UblNamespaces.Cac)]
    public List<AllowanceCharge>? AllowanceCharges { get; set; }

    /// <summary>
    /// Tax totals.
    /// </summary>
    [JsonPropertyName("taxTotals")]
    [XmlElement("TaxTotal", Namespace = UblNamespaces.Cac)]
    public List<TaxTotal>? TaxTotals { get; set; }

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
}
