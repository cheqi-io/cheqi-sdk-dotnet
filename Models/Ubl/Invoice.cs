using System.Text.Json.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Invoice - a complete invoice document.
/// </summary>
public sealed class Invoice
{
    /// <summary>
    /// Customization identifier (e.g., "urn:cen.eu:en16931:2017#compliant#urn:fdc:peppol.eu:2017:poacc:billing:3.0").
    /// </summary>
    [JsonPropertyName("customizationId")]
    public string? CustomizationId { get; set; }

    /// <summary>
    /// Profile identifier (e.g., "urn:fdc:peppol.eu:2017:poacc:billing:01:1.0").
    /// </summary>
    [JsonPropertyName("profileId")]
    public string? ProfileId { get; set; }

    /// <summary>
    /// Invoice number.
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>
    /// Issue date.
    /// </summary>
    [JsonPropertyName("issueDate")]
    public string? IssueDate { get; set; }

    /// <summary>
    /// Due date.
    /// </summary>
    [JsonPropertyName("dueDate")]
    public DateTimeOffset? DueDate { get; set; }

    /// <summary>
    /// Invoice type code (see UNCL1001-inv, default is 380).
    /// </summary>
    [JsonPropertyName("invoiceTypeCode")]
    public int? InvoiceTypeCode { get; set; }

    /// <summary>
    /// Note or additional text.
    /// </summary>
    [JsonPropertyName("note")]
    public string? Note { get; set; }

    /// <summary>
    /// Tax point date.
    /// </summary>
    [JsonPropertyName("taxPointDate")]
    public DateTimeOffset? TaxPointDate { get; set; }

    /// <summary>
    /// Document currency code.
    /// </summary>
    [JsonPropertyName("documentCurrencyCode")]
    public string? DocumentCurrencyCode { get; set; }

    /// <summary>
    /// Tax currency code.
    /// </summary>
    [JsonPropertyName("taxCurrencyCode")]
    public string? TaxCurrencyCode { get; set; }

    /// <summary>
    /// Accounting cost code.
    /// </summary>
    [JsonPropertyName("accountingCost")]
    public string? AccountingCost { get; set; }

    /// <summary>
    /// Buyer reference.
    /// </summary>
    [JsonPropertyName("buyerReference")]
    public string? BuyerReference { get; set; }

    /// <summary>
    /// Invoice period.
    /// </summary>
    [JsonPropertyName("invoicePeriod")]
    public Period? InvoicePeriod { get; set; }

    /// <summary>
    /// Order reference.
    /// </summary>
    [JsonPropertyName("orderReference")]
    public OrderReference? OrderReference { get; set; }

    /// <summary>
    /// Billing references.
    /// </summary>
    [JsonPropertyName("billingReference")]
    public List<BillingReference>? BillingReference { get; set; }

    /// <summary>
    /// Additional document references.
    /// </summary>
    [JsonPropertyName("additionalDocumentReference")]
    public List<DocumentReference>? AdditionalDocumentReference { get; set; }

    /// <summary>
    /// Supplier (seller) party.
    /// </summary>
    [JsonPropertyName("accountingSupplierParty")]
    public Party? AccountingSupplierParty { get; set; }

    /// <summary>
    /// Customer (buyer) party.
    /// </summary>
    [JsonPropertyName("accountingCustomerParty")]
    public Party? AccountingCustomerParty { get; set; }

    /// <summary>
    /// Tax representative party.
    /// </summary>
    [JsonPropertyName("taxRepresentativeParty")]
    public Party? TaxRepresentativeParty { get; set; }

    /// <summary>
    /// Delivery information.
    /// </summary>
    [JsonPropertyName("delivery")]
    public Delivery? Delivery { get; set; }

    /// <summary>
    /// Payment means.
    /// </summary>
    [JsonPropertyName("paymentMeans")]
    public PaymentMeans? PaymentMeans { get; set; }

    /// <summary>
    /// Payment terms.
    /// </summary>
    [JsonPropertyName("paymentTerms")]
    public PaymentTerms? PaymentTerms { get; set; }

    /// <summary>
    /// Document-level allowances and charges.
    /// </summary>
    [JsonPropertyName("allowanceCharge")]
    public List<AllowanceCharge>? AllowanceCharge { get; set; }

    /// <summary>
    /// Tax total.
    /// </summary>
    [JsonPropertyName("taxTotal")]
    public TaxTotal? TaxTotal { get; set; }

    /// <summary>
    /// Monetary totals.
    /// </summary>
    [JsonPropertyName("monetaryTotal")]
    public MonetaryTotal? MonetaryTotal { get; set; }

    /// <summary>
    /// Invoice lines.
    /// </summary>
    [JsonPropertyName("invoiceLines")]
    public List<InvoiceLine>? InvoiceLines { get; set; }
}
