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
    /// Customization identifier.
    /// </summary>
    [JsonPropertyName("customizationId")]
    [XmlElement("CustomizationID", Namespace = UblNamespaces.Cbc)]
    public Identifier? CustomizationId { get; set; }

    /// <summary>
    /// Profile identifier.
    /// </summary>
    [JsonPropertyName("profileId")]
    [XmlElement("ProfileID", Namespace = UblNamespaces.Cbc)]
    public Identifier? ProfileId { get; set; }

    /// <summary>
    /// Profile execution identifier.
    /// </summary>
    [JsonPropertyName("profileExecutionId")]
    [XmlElement("ProfileExecutionID", Namespace = UblNamespaces.Cbc)]
    public Identifier? ProfileExecutionId { get; set; }

    /// <summary>
    /// Unique receipt identifier.
    /// </summary>
    [JsonPropertyName("id")]
    [XmlElement("ID", Namespace = UblNamespaces.Cbc)]
    public Identifier? Id { get; set; }

    /// <summary>
    /// UUID identifier.
    /// </summary>
    [JsonPropertyName("uuid")]
    [XmlElement("UUID", Namespace = UblNamespaces.Cbc)]
    public Identifier? Uuid { get; set; }

    /// <summary>
    /// Issue date of the receipt.
    /// </summary>
    [JsonPropertyName("issueDate")]
    [XmlElement("IssueDate", Namespace = UblNamespaces.Cbc)]
    public string? IssueDate { get; set; }

    /// <summary>
    /// Issue time of the receipt.
    /// </summary>
    [JsonPropertyName("issueTime")]
    [XmlElement("IssueTime", Namespace = UblNamespaces.Cbc)]
    public string? IssueTime { get; set; }

    /// <summary>
    /// Transaction date.
    /// </summary>
    [JsonPropertyName("transactionDate")]
    [XmlElement("TransactionDate", Namespace = UblNamespaces.Cbc)]
    public string? TransactionDate { get; set; }

    /// <summary>
    /// Transaction time.
    /// </summary>
    [JsonPropertyName("transactionTime")]
    [XmlElement("TransactionTime", Namespace = UblNamespaces.Cbc)]
    public string? TransactionTime { get; set; }

    /// <summary>
    /// Purchase date.
    /// </summary>
    [JsonPropertyName("purchaseDate")]
    [XmlElement("PurchaseDate", Namespace = UblNamespaces.Cbc)]
    public string? PurchaseDate { get; set; }

    /// <summary>
    /// Purchase time.
    /// </summary>
    [JsonPropertyName("purchaseTime")]
    [XmlElement("PurchaseTime", Namespace = UblNamespaces.Cbc)]
    public string? PurchaseTime { get; set; }

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
    public string? DocumentCurrencyCode { get; set; }

    /// <summary>
    /// Purchase reference.
    /// </summary>
    [JsonPropertyName("purchaseReference")]
    [XmlElement("PurchaseReference", Namespace = UblNamespaces.Cac)]
    public PurchaseReference? PurchaseReference { get; set; }

    /// <summary>
    /// Sales document reference.
    /// </summary>
    [JsonPropertyName("salesDocumentReference")]
    [XmlElement("SalesDocumentReference", Namespace = UblNamespaces.Cac)]
    public DocumentReference? SalesDocumentReference { get; set; }

    /// <summary>
    /// Additional document reference.
    /// </summary>
    [JsonPropertyName("additionalDocumentReference")]
    [XmlElement("AdditionalDocumentReference", Namespace = UblNamespaces.Cac)]
    public DocumentReference? AdditionalDocumentReference { get; set; }

    /// <summary>
    /// Digital signature.
    /// </summary>
    [JsonPropertyName("signature")]
    [XmlElement("Signature", Namespace = UblNamespaces.Cac)]
    public Signature? Signature { get; set; }

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
    /// Cashier contact information.
    /// </summary>
    [JsonPropertyName("cashierContact")]
    [XmlElement("CashierContact", Namespace = UblNamespaces.Cac)]
    public Contact? CashierContact { get; set; }

    /// <summary>
    /// Cash register information.
    /// </summary>
    [JsonPropertyName("cashRegister")]
    [XmlElement("ShopperContact", Namespace = UblNamespaces.Cac)]
    public CashRegister? CashRegister { get; set; }

    /// <summary>
    /// Point of sale location.
    /// </summary>
    [JsonPropertyName("pointOfSaleLocation")]
    [XmlElement("PointOfSaleLocation", Namespace = UblNamespaces.Cac)]
    public Location? PointOfSaleLocation { get; set; }

    /// <summary>
    /// Point of sale contact.
    /// </summary>
    [JsonPropertyName("pointOfSaleContact")]
    [XmlElement("PointOfSaleContact", Namespace = UblNamespaces.Cac)]
    public Contact? PointOfSaleContact { get; set; }

    /// <summary>
    /// Payment information.
    /// </summary>
    [JsonPropertyName("payment")]
    [XmlElement("Payment", Namespace = UblNamespaces.Cac)]
    public Payment? Payment { get; set; }

    /// <summary>
    /// Payment means information.
    /// </summary>
    [JsonPropertyName("paymentMeans")]
    [XmlElement("PaymentMeans", Namespace = UblNamespaces.Cac)]
    public List<PaymentMeans>? PaymentMeans { get; set; }

    /// <summary>
    /// Document-level allowances and charges.
    /// </summary>
    [JsonPropertyName("allowanceCharge")]
    [XmlElement("AllowanceCharge", Namespace = UblNamespaces.Cac)]
    public List<AllowanceCharge>? AllowanceCharge { get; set; }

    /// <summary>
    /// Tax totals.
    /// </summary>
    [JsonPropertyName("taxTotals")]
    [XmlElement("TaxTotal", Namespace = UblNamespaces.Cac)]
    public List<TaxTotal>? TaxTotal { get; set; }

    /// <summary>
    /// Monetary totals.
    /// </summary>
    [JsonPropertyName("legalMonetaryTotal")]
    [XmlElement("LegalMonetaryTotal", Namespace = UblNamespaces.Cac)]
    public MonetaryTotal? LegalMonetaryTotal { get; set; }

    /// <summary>
    /// Receipt lines.
    /// </summary>
    [JsonPropertyName("purchaseReceiptLines")]
    [XmlElement("PurchaseReceiptLine", Namespace = UblNamespaces.Cac)]
    public List<PurchaseReceiptLine>? PurchaseReceiptLine { get; set; }

    /// <summary>
    /// Verification nonce for receipt validation.
    /// </summary>
    [JsonPropertyName("verificationNonce")]
    [XmlElement("VerificationNonce", Namespace = UblNamespaces.Cbc)]
    public string? VerificationNonce { get; set; }
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
    public Identifier? Id { get; set; }

    /// <summary>
    /// Document type code.
    /// </summary>
    [JsonPropertyName("documentTypeCode")]
    [XmlElement("DocumentTypeCode", Namespace = UblNamespaces.Cbc)]
    public Code? DocumentTypeCode { get; set; }

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
    public ExternalReference? ExternalReference { get; set; }
}
