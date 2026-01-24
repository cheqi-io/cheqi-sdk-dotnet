using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Purchase Receipt - represents a complete receipt document.
/// </summary>
[XmlRoot("PurchaseReceipt", Namespace = UblNamespaces.PurchaseReceipt)]
public sealed class PurchaseReceipt
{
    /// <summary>
    /// UBL version identifier.
    /// </summary>
    [JsonPropertyName("ublVersionId")]
    [XmlElement("UBLVersionID", Namespace = UblNamespaces.Cbc)]
    public string UblVersionId { get; set; } = "2.1";

    /// <summary>
    /// Customization identifier.
    /// </summary>
    [JsonPropertyName("customizationId")]
    [XmlElement("CustomizationID", Namespace = UblNamespaces.Cbc)]
    public string? CustomizationId { get; set; }

    /// <summary>
    /// Profile identifier.
    /// </summary>
    [JsonPropertyName("profileId")]
    [XmlElement("ProfileID", Namespace = UblNamespaces.Cbc)]
    public string? ProfileId { get; set; }

    /// <summary>
    /// Unique receipt identifier.
    /// </summary>
    [JsonPropertyName("id")]
    [XmlElement("ID", Namespace = UblNamespaces.Cbc)]
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Issue date of the receipt.
    /// </summary>
    [JsonPropertyName("issueDate")]
    [XmlElement("IssueDate", Namespace = UblNamespaces.Cbc)]
    public DateTimeOffset IssueDate { get; set; }

    /// <summary>
    /// Issue time of the receipt.
    /// </summary>
    [JsonPropertyName("issueTime")]
    [XmlElement("IssueTime", Namespace = UblNamespaces.Cbc)]
    public TimeSpan? IssueTime { get; set; }

    /// <summary>
    /// Receipt type code.
    /// </summary>
    [JsonPropertyName("receiptTypeCode")]
    [XmlElement("ReceiptTypeCode", Namespace = UblNamespaces.Cbc)]
    public string? ReceiptTypeCode { get; set; }

    /// <summary>
    /// Note or additional text.
    /// </summary>
    [JsonPropertyName("note")]
    [XmlElement("Note", Namespace = UblNamespaces.Cbc)]
    public string? Note { get; set; }

    /// <summary>
    /// Document currency code.
    /// </summary>
    [JsonPropertyName("documentCurrencyCode")]
    [XmlElement("DocumentCurrencyCode", Namespace = UblNamespaces.Cbc)]
    public string DocumentCurrencyCode { get; set; } = "EUR";

    /// <summary>
    /// Tax currency code (if different from document currency).
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
    /// Billing period start date.
    /// </summary>
    [JsonPropertyName("invoicePeriodStartDate")]
    [XmlElement("InvoicePeriodStartDate", Namespace = UblNamespaces.Cbc)]
    public DateTimeOffset? InvoicePeriodStartDate { get; set; }

    /// <summary>
    /// Billing period end date.
    /// </summary>
    [JsonPropertyName("invoicePeriodEndDate")]
    [XmlElement("InvoicePeriodEndDate", Namespace = UblNamespaces.Cbc)]
    public DateTimeOffset? InvoicePeriodEndDate { get; set; }

    /// <summary>
    /// Order reference.
    /// </summary>
    [JsonPropertyName("orderReference")]
    [XmlElement("OrderReference", Namespace = UblNamespaces.Cbc)]
    public string? OrderReference { get; set; }

    /// <summary>
    /// Contract reference.
    /// </summary>
    [JsonPropertyName("contractDocumentReference")]
    [XmlElement("ContractDocumentReference", Namespace = UblNamespaces.Cbc)]
    public string? ContractDocumentReference { get; set; }

    /// <summary>
    /// Additional document references.
    /// </summary>
    [JsonPropertyName("additionalDocumentReferences")]
    [XmlElement("AdditionalDocumentReference", Namespace = UblNamespaces.Cac)]
    public List<DocumentReference>? AdditionalDocumentReferences { get; set; }

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
    /// Payee party (if different from supplier).
    /// </summary>
    [JsonPropertyName("payeeParty")]
    [XmlElement("PayeeParty", Namespace = UblNamespaces.Cac)]
    public Party? PayeeParty { get; set; }

    /// <summary>
    /// Delivery information.
    /// </summary>
    [JsonPropertyName("delivery")]
    [XmlElement("Delivery", Namespace = UblNamespaces.Cac)]
    public Delivery? Delivery { get; set; }

    /// <summary>
    /// Payment means information.
    /// </summary>
    [JsonPropertyName("paymentMeans")]
    [XmlElement("PaymentMeans", Namespace = UblNamespaces.Cac)]
    public List<PaymentMeans>? PaymentMeans { get; set; }

    /// <summary>
    /// Payment terms.
    /// </summary>
    [JsonPropertyName("paymentTerms")]
    [XmlElement("PaymentTerms", Namespace = UblNamespaces.Cac)]
    public PaymentTerms? PaymentTerms { get; set; }

    /// <summary>
    /// Document-level allowances and charges.
    /// </summary>
    [JsonPropertyName("allowanceCharges")]
    [XmlElement("AllowanceCharge", Namespace = UblNamespaces.Cac)]
    public List<AllowanceCharge>? AllowanceCharges { get; set; }

    /// <summary>
    /// Tax totals.
    /// </summary>
    [JsonPropertyName("taxTotal")]
    [XmlElement("TaxTotal", Namespace = UblNamespaces.Cac)]
    public List<TaxTotal>? TaxTotal { get; set; }

    /// <summary>
    /// Monetary totals.
    /// </summary>
    [JsonPropertyName("legalMonetaryTotal")]
    [XmlElement("LegalMonetaryTotal", Namespace = UblNamespaces.Cac)]
    public MonetaryTotal? LegalMonetaryTotal { get; set; }

    /// <summary>
    /// Invoice/receipt lines.
    /// </summary>
    [JsonPropertyName("invoiceLines")]
    [XmlElement("InvoiceLine", Namespace = UblNamespaces.Cac)]
    public List<InvoiceLine> InvoiceLines { get; set; } = new();
}

/// <summary>
/// UBL Document Reference.
/// </summary>
[XmlType(Namespace = UblNamespaces.Cac)]
public sealed class DocumentReference
{
    /// <summary>
    /// Document ID.
    /// </summary>
    [JsonPropertyName("id")]
    [XmlElement("ID", Namespace = UblNamespaces.Cbc)]
    public string? Id { get; set; }

    /// <summary>
    /// Document type code.
    /// </summary>
    [JsonPropertyName("documentTypeCode")]
    [XmlElement("DocumentTypeCode", Namespace = UblNamespaces.Cbc)]
    public string? DocumentTypeCode { get; set; }

    /// <summary>
    /// Document description.
    /// </summary>
    [JsonPropertyName("documentDescription")]
    [XmlElement("DocumentDescription", Namespace = UblNamespaces.Cbc)]
    public string? DocumentDescription { get; set; }

    /// <summary>
    /// Attachment (embedded document).
    /// </summary>
    [JsonPropertyName("attachment")]
    [XmlElement("Attachment", Namespace = UblNamespaces.Cac)]
    public Attachment? Attachment { get; set; }
}

/// <summary>
/// UBL Attachment.
/// </summary>
[XmlType(Namespace = UblNamespaces.Cac)]
public sealed class Attachment
{
    /// <summary>
    /// Embedded document binary (Base64).
    /// </summary>
    [JsonPropertyName("embeddedDocumentBinaryObject")]
    [XmlElement("EmbeddedDocumentBinaryObject", Namespace = UblNamespaces.Cbc)]
    public string? EmbeddedDocumentBinaryObject { get; set; }

    /// <summary>
    /// MIME type of the embedded document.
    /// </summary>
    [JsonPropertyName("mimeCode")]
    [XmlIgnore]
    public string? MimeCode { get; set; }

    /// <summary>
    /// Filename of the embedded document.
    /// </summary>
    [JsonPropertyName("filename")]
    [XmlIgnore]
    public string? Filename { get; set; }

    /// <summary>
    /// External reference URL.
    /// </summary>
    [JsonPropertyName("externalReference")]
    [XmlElement("ExternalReference", Namespace = UblNamespaces.Cac)]
    public string? ExternalReference { get; set; }
}

/// <summary>
/// UBL Delivery information.
/// </summary>
[XmlType(Namespace = UblNamespaces.Cac)]
public sealed class Delivery
{
    /// <summary>
    /// Actual delivery date.
    /// </summary>
    [JsonPropertyName("actualDeliveryDate")]
    [XmlElement("ActualDeliveryDate", Namespace = UblNamespaces.Cbc)]
    public DateTimeOffset? ActualDeliveryDate { get; set; }

    /// <summary>
    /// Delivery location.
    /// </summary>
    [JsonPropertyName("deliveryLocation")]
    [XmlElement("DeliveryLocation", Namespace = UblNamespaces.Cac)]
    public DeliveryLocation? DeliveryLocation { get; set; }

    /// <summary>
    /// Delivery party.
    /// </summary>
    [JsonPropertyName("deliveryParty")]
    [XmlElement("DeliveryParty", Namespace = UblNamespaces.Cac)]
    public Party? DeliveryParty { get; set; }
}

/// <summary>
/// UBL Delivery Location.
/// </summary>
[XmlType(Namespace = UblNamespaces.Cac)]
public sealed class DeliveryLocation
{
    /// <summary>
    /// Location ID.
    /// </summary>
    [JsonPropertyName("id")]
    [XmlElement("ID", Namespace = UblNamespaces.Cbc)]
    public string? Id { get; set; }

    /// <summary>
    /// Location description.
    /// </summary>
    [JsonPropertyName("description")]
    [XmlElement("Description", Namespace = UblNamespaces.Cbc)]
    public string? Description { get; set; }

    /// <summary>
    /// Address.
    /// </summary>
    [JsonPropertyName("address")]
    [XmlElement("Address", Namespace = UblNamespaces.Cac)]
    public Cheqi.Sdk.Models.Company.Address? Address { get; set; }
}

/// <summary>
/// UBL Payment Terms.
/// </summary>
[XmlType(Namespace = UblNamespaces.Cac)]
public sealed class PaymentTerms
{
    /// <summary>
    /// Payment terms note.
    /// </summary>
    [JsonPropertyName("note")]
    [XmlElement("Note", Namespace = UblNamespaces.Cbc)]
    public string? Note { get; set; }
}
