using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Invoice - a complete invoice document.
/// </summary>
[XmlRoot("Invoice", Namespace = UblNamespaces.Invoice)]
public sealed class Invoice
{
    /// <summary>
    /// Customization identifier (e.g., "urn:cen.eu:en16931:2017#compliant#urn:fdc:peppol.eu:2017:poacc:billing:3.0").
    /// </summary>
    [JsonPropertyName("customizationId")]
    [XmlElement("CustomizationID", Namespace = UblNamespaces.Cbc)]
    public string? CustomizationId { get; set; }

    /// <summary>
    /// Profile identifier (e.g., "urn:fdc:peppol.eu:2017:poacc:billing:01:1.0").
    /// </summary>
    [JsonPropertyName("profileId")]
    [XmlElement("ProfileID", Namespace = UblNamespaces.Cbc)]
    public string? ProfileId { get; set; }

    /// <summary>
    /// Invoice number.
    /// </summary>
    [JsonPropertyName("id")]
    [XmlElement("ID", Namespace = UblNamespaces.Cbc)]
    public string? Id { get; set; }

    /// <summary>
    /// Issue date.
    /// </summary>
    [JsonPropertyName("issueDate")]
    [XmlElement("IssueDate", Namespace = UblNamespaces.Cbc)]
    public string? IssueDate { get; set; }

    /// <summary>
    /// Due date.
    /// </summary>
    [JsonPropertyName("dueDate")]
    [XmlElement("DueDate", Namespace = UblNamespaces.Cbc)]
    public DateTimeOffset? DueDate { get; set; }

    /// <summary>
    /// Invoice type code (see UNCL1001-inv, default is 380).
    /// </summary>
    [JsonPropertyName("invoiceTypeCode")]
    [XmlElement("InvoiceTypeCode", Namespace = UblNamespaces.Cbc)]
    public int? InvoiceTypeCode { get; set; }

    /// <summary>
    /// Note or additional text.
    /// </summary>
    [JsonPropertyName("note")]
    [XmlElement("Note", Namespace = UblNamespaces.Cbc)]
    public string? Note { get; set; }

    /// <summary>
    /// Tax point date.
    /// </summary>
    [JsonPropertyName("taxPointDate")]
    [XmlElement("TaxPointDate", Namespace = UblNamespaces.Cbc)]
    public DateTimeOffset? TaxPointDate { get; set; }

    /// <summary>
    /// Document currency code.
    /// </summary>
    [JsonPropertyName("documentCurrencyCode")]
    [XmlElement("DocumentCurrencyCode", Namespace = UblNamespaces.Cbc)]
    public string? DocumentCurrencyCode { get; set; }

    /// <summary>
    /// Tax currency code.
    /// </summary>
    [JsonPropertyName("taxCurrencyCode")]
    [XmlElement("TaxCurrencyCode", Namespace = UblNamespaces.Cbc)]
    public string? TaxCurrencyCode { get; set; }

    /// <summary>
    /// Accounting cost code.
    /// </summary>
    [JsonPropertyName("accountingCost")]
    [XmlElement("AccountingCost", Namespace = UblNamespaces.Cbc)]
    public string? AccountingCost { get; set; }

    /// <summary>
    /// Buyer reference.
    /// </summary>
    [JsonPropertyName("buyerReference")]
    [XmlElement("BuyerReference", Namespace = UblNamespaces.Cbc)]
    public string? BuyerReference { get; set; }

    /// <summary>
    /// Invoice period.
    /// </summary>
    [JsonPropertyName("invoicePeriod")]
    [XmlElement("InvoicePeriod", Namespace = UblNamespaces.Cac)]
    public Period? InvoicePeriod { get; set; }

    /// <summary>
    /// Order reference.
    /// </summary>
    [JsonPropertyName("orderReference")]
    [XmlElement("OrderReference", Namespace = UblNamespaces.Cac)]
    public OrderReference? OrderReference { get; set; }

    /// <summary>
    /// Billing references.
    /// </summary>
    [JsonPropertyName("billingReference")]
    [XmlElement("BillingReference", Namespace = UblNamespaces.Cac)]
    public List<BillingReference>? BillingReference { get; set; }

    /// <summary>
    /// Additional document references.
    /// </summary>
    [JsonPropertyName("additionalDocumentReference")]
    [XmlElement("AdditionalDocumentReference", Namespace = UblNamespaces.Cac)]
    public List<DocumentReference>? AdditionalDocumentReference { get; set; }

    /// <summary>
    /// Supplier (seller) party.
    /// </summary>
    [JsonPropertyName("accountingSupplierParty")]
    [XmlElement("AccountingSupplierParty", Namespace = UblNamespaces.Cac)]
    public Party? AccountingSupplierParty { get; set; }

    /// <summary>
    /// Customer (buyer) party.
    /// </summary>
    [JsonPropertyName("accountingCustomerParty")]
    [XmlElement("AccountingCustomerParty", Namespace = UblNamespaces.Cac)]
    public Party? AccountingCustomerParty { get; set; }

    /// <summary>
    /// Tax representative party.
    /// </summary>
    [JsonPropertyName("taxRepresentativeParty")]
    [XmlElement("TaxRepresentativeParty", Namespace = UblNamespaces.Cac)]
    public Party? TaxRepresentativeParty { get; set; }

    /// <summary>
    /// Delivery information.
    /// </summary>
    [JsonPropertyName("delivery")]
    [XmlElement("Delivery", Namespace = UblNamespaces.Cac)]
    public Delivery? Delivery { get; set; }

    /// <summary>
    /// Payment means.
    /// </summary>
    [JsonPropertyName("paymentMeans")]
    [XmlElement("PaymentMeans", Namespace = UblNamespaces.Cac)]
    public PaymentMeans? PaymentMeans { get; set; }

    /// <summary>
    /// Payment terms.
    /// </summary>
    [JsonPropertyName("paymentTerms")]
    [XmlElement("PaymentTerms", Namespace = UblNamespaces.Cac)]
    public PaymentTerms? PaymentTerms { get; set; }

    /// <summary>
    /// Document-level allowances and charges.
    /// </summary>
    [JsonPropertyName("allowanceCharge")]
    [XmlElement("AllowanceCharge", Namespace = UblNamespaces.Cac)]
    public List<AllowanceCharge>? AllowanceCharge { get; set; }

    /// <summary>
    /// Tax total.
    /// </summary>
    [JsonPropertyName("taxTotal")]
    [XmlElement("TaxTotal", Namespace = UblNamespaces.Cac)]
    public TaxTotal? TaxTotal { get; set; }

    /// <summary>
    /// Monetary totals.
    /// </summary>
    [JsonPropertyName("legalMonetaryTotal")]
    [XmlElement("LegalMonetaryTotal", Namespace = UblNamespaces.Cac)]
    public MonetaryTotal? LegalMonetaryTotal { get; set; }

    /// <summary>
    /// Invoice lines.
    /// </summary>
    [JsonPropertyName("invoiceLine")]
    [XmlElement("InvoiceLine", Namespace = UblNamespaces.Cac)]
    public List<InvoiceLine>? InvoiceLines { get; set; }
}
