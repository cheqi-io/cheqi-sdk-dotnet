using System.Text.Json.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Purchase Receipt - represents a complete receipt document.
/// </summary>
public sealed class PurchaseReceipt
{
    /// <summary>
    /// UBL version identifier.
    /// </summary>
    [JsonPropertyName("ublVersionId")]
    public string UblVersionId { get; set; } = "2.1";

    /// <summary>
    /// Customization identifier.
    /// </summary>
    [JsonPropertyName("customizationId")]
    public string? CustomizationId { get; set; }

    /// <summary>
    /// Profile identifier.
    /// </summary>
    [JsonPropertyName("profileId")]
    public string? ProfileId { get; set; }

    /// <summary>
    /// Unique receipt identifier.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Issue date of the receipt.
    /// </summary>
    [JsonPropertyName("issueDate")]
    public DateTimeOffset IssueDate { get; set; }

    /// <summary>
    /// Issue time of the receipt.
    /// </summary>
    [JsonPropertyName("issueTime")]
    public TimeSpan? IssueTime { get; set; }

    /// <summary>
    /// Receipt type code.
    /// </summary>
    [JsonPropertyName("receiptTypeCode")]
    public string? ReceiptTypeCode { get; set; }

    /// <summary>
    /// Note or additional text.
    /// </summary>
    [JsonPropertyName("note")]
    public string? Note { get; set; }

    /// <summary>
    /// Document currency code.
    /// </summary>
    [JsonPropertyName("documentCurrencyCode")]
    public string DocumentCurrencyCode { get; set; } = "EUR";

    /// <summary>
    /// Tax currency code (if different from document currency).
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
    /// Billing period start date.
    /// </summary>
    [JsonPropertyName("invoicePeriodStartDate")]
    public DateTimeOffset? InvoicePeriodStartDate { get; set; }

    /// <summary>
    /// Billing period end date.
    /// </summary>
    [JsonPropertyName("invoicePeriodEndDate")]
    public DateTimeOffset? InvoicePeriodEndDate { get; set; }

    /// <summary>
    /// Order reference.
    /// </summary>
    [JsonPropertyName("orderReference")]
    public string? OrderReference { get; set; }

    /// <summary>
    /// Contract reference.
    /// </summary>
    [JsonPropertyName("contractDocumentReference")]
    public string? ContractDocumentReference { get; set; }

    /// <summary>
    /// Additional document references.
    /// </summary>
    [JsonPropertyName("additionalDocumentReferences")]
    public List<DocumentReference>? AdditionalDocumentReferences { get; set; }

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
    /// Payee party (if different from supplier).
    /// </summary>
    [JsonPropertyName("payeeParty")]
    public Party? PayeeParty { get; set; }

    /// <summary>
    /// Delivery information.
    /// </summary>
    [JsonPropertyName("delivery")]
    public Delivery? Delivery { get; set; }

    /// <summary>
    /// Payment means information.
    /// </summary>
    [JsonPropertyName("paymentMeans")]
    public List<PaymentMeans>? PaymentMeans { get; set; }

    /// <summary>
    /// Payment terms.
    /// </summary>
    [JsonPropertyName("paymentTerms")]
    public PaymentTerms? PaymentTerms { get; set; }

    /// <summary>
    /// Document-level allowances and charges.
    /// </summary>
    [JsonPropertyName("allowanceCharges")]
    public List<AllowanceCharge>? AllowanceCharges { get; set; }

    /// <summary>
    /// Tax totals.
    /// </summary>
    [JsonPropertyName("taxTotal")]
    public List<TaxTotal>? TaxTotal { get; set; }

    /// <summary>
    /// Monetary totals.
    /// </summary>
    [JsonPropertyName("legalMonetaryTotal")]
    public MonetaryTotal? LegalMonetaryTotal { get; set; }

    /// <summary>
    /// Invoice/receipt lines.
    /// </summary>
    [JsonPropertyName("invoiceLines")]
    public List<InvoiceLine> InvoiceLines { get; set; } = new();
}

/// <summary>
/// UBL Document Reference.
/// </summary>
public sealed class DocumentReference
{
    /// <summary>
    /// Document ID.
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>
    /// Document type code.
    /// </summary>
    [JsonPropertyName("documentTypeCode")]
    public string? DocumentTypeCode { get; set; }

    /// <summary>
    /// Document description.
    /// </summary>
    [JsonPropertyName("documentDescription")]
    public string? DocumentDescription { get; set; }

    /// <summary>
    /// Attachment (embedded document).
    /// </summary>
    [JsonPropertyName("attachment")]
    public Attachment? Attachment { get; set; }
}

/// <summary>
/// UBL Attachment.
/// </summary>
public sealed class Attachment
{
    /// <summary>
    /// Embedded document binary (Base64).
    /// </summary>
    [JsonPropertyName("embeddedDocumentBinaryObject")]
    public string? EmbeddedDocumentBinaryObject { get; set; }

    /// <summary>
    /// MIME type of the embedded document.
    /// </summary>
    [JsonPropertyName("mimeCode")]
    public string? MimeCode { get; set; }

    /// <summary>
    /// Filename of the embedded document.
    /// </summary>
    [JsonPropertyName("filename")]
    public string? Filename { get; set; }

    /// <summary>
    /// External reference URL.
    /// </summary>
    [JsonPropertyName("externalReference")]
    public string? ExternalReference { get; set; }
}

/// <summary>
/// UBL Delivery information.
/// </summary>
public sealed class Delivery
{
    /// <summary>
    /// Actual delivery date.
    /// </summary>
    [JsonPropertyName("actualDeliveryDate")]
    public DateTimeOffset? ActualDeliveryDate { get; set; }

    /// <summary>
    /// Delivery location.
    /// </summary>
    [JsonPropertyName("deliveryLocation")]
    public DeliveryLocation? DeliveryLocation { get; set; }

    /// <summary>
    /// Delivery party.
    /// </summary>
    [JsonPropertyName("deliveryParty")]
    public Party? DeliveryParty { get; set; }
}

/// <summary>
/// UBL Delivery Location.
/// </summary>
public sealed class DeliveryLocation
{
    /// <summary>
    /// Location ID.
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>
    /// Location description.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Address.
    /// </summary>
    [JsonPropertyName("address")]
    public Cheqi.Sdk.Models.Company.Address? Address { get; set; }
}

/// <summary>
/// UBL Payment Terms.
/// </summary>
public sealed class PaymentTerms
{
    /// <summary>
    /// Payment terms note.
    /// </summary>
    [JsonPropertyName("note")]
    public string? Note { get; set; }
}
