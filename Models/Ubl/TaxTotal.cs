using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Tax Total - tax summary for an invoice or receipt.
/// </summary>
[XmlType(Namespace = UblNamespaces.Cac)]
public sealed class TaxTotal
{
    /// <summary>
    /// Total tax amount.
    /// </summary>
    [JsonPropertyName("taxAmount")]
    [XmlElement("TaxAmount", Namespace = UblNamespaces.Cbc)]
    public decimal TaxAmount { get; set; }

    /// <summary>
    /// Currency code.
    /// </summary>
    [JsonPropertyName("currencyId")]
    [XmlIgnore]
    public string? CurrencyId { get; set; }

    /// <summary>
    /// Tax subtotals by category.
    /// </summary>
    [JsonPropertyName("taxSubtotals")]
    [XmlElement("TaxSubtotal", Namespace = UblNamespaces.Cac)]
    public List<TaxSubtotal>? TaxSubtotals { get; set; }
}

/// <summary>
/// UBL Tax Subtotal - tax breakdown by category.
/// </summary>
[XmlType(Namespace = UblNamespaces.Cac)]
public sealed class TaxSubtotal
{
    /// <summary>
    /// Taxable amount for this category.
    /// </summary>
    [JsonPropertyName("taxableAmount")]
    [XmlElement("TaxableAmount", Namespace = UblNamespaces.Cbc)]
    public decimal TaxableAmount { get; set; }

    /// <summary>
    /// Tax amount for this category.
    /// </summary>
    [JsonPropertyName("taxAmount")]
    [XmlElement("TaxAmount", Namespace = UblNamespaces.Cbc)]
    public decimal TaxAmount { get; set; }

    /// <summary>
    /// Tax category details.
    /// </summary>
    [JsonPropertyName("taxCategory")]
    [XmlElement("TaxCategory", Namespace = UblNamespaces.Cac)]
    public TaxCategory? TaxCategory { get; set; }

    /// <summary>
    /// Currency code.
    /// </summary>
    [JsonPropertyName("currencyId")]
    [XmlIgnore]
    public string? CurrencyId { get; set; }
}

/// <summary>
/// UBL Tax Category.
/// </summary>
[XmlType(Namespace = UblNamespaces.Cac)]
public sealed class TaxCategory
{
    /// <summary>
    /// Category ID (e.g., "S" for standard, "Z" for zero-rated).
    /// </summary>
    [JsonPropertyName("id")]
    [XmlElement("ID", Namespace = UblNamespaces.Cbc)]
    public string? Id { get; set; }

    /// <summary>
    /// Tax percentage.
    /// </summary>
    [JsonPropertyName("percent")]
    [XmlElement("Percent", Namespace = UblNamespaces.Cbc)]
    public decimal? Percent { get; set; }

    /// <summary>
    /// Tax scheme.
    /// </summary>
    [JsonPropertyName("taxScheme")]
    [XmlElement("TaxScheme", Namespace = UblNamespaces.Cac)]
    public TaxScheme? TaxScheme { get; set; }

    /// <summary>
    /// Tax exemption reason code.
    /// </summary>
    [JsonPropertyName("taxExemptionReasonCode")]
    [XmlElement("TaxExemptionReasonCode", Namespace = UblNamespaces.Cbc)]
    public string? TaxExemptionReasonCode { get; set; }

    /// <summary>
    /// Tax exemption reason.
    /// </summary>
    [JsonPropertyName("taxExemptionReason")]
    [XmlElement("TaxExemptionReason", Namespace = UblNamespaces.Cbc)]
    public string? TaxExemptionReason { get; set; }
}

/// <summary>
/// UBL Tax Scheme.
/// </summary>
[XmlType(Namespace = UblNamespaces.Cac)]
public sealed class TaxScheme
{
    /// <summary>
    /// Tax scheme ID (e.g., "VAT").
    /// </summary>
    [JsonPropertyName("id")]
    [XmlElement("ID", Namespace = UblNamespaces.Cbc)]
    public string? Id { get; set; }

    /// <summary>
    /// Tax scheme name.
    /// </summary>
    [JsonPropertyName("name")]
    [XmlElement("Name", Namespace = UblNamespaces.Cbc)]
    public string? Name { get; set; }
}
