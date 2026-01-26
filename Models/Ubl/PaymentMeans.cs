using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Payment Means - describes how payment is made.
/// </summary>
[XmlType(Namespace = UblNamespaces.Cac)]
public sealed class PaymentMeans
{
    /// <summary>
    /// Payment means code (UNCL 4461).
    /// </summary>
    [JsonPropertyName("paymentMeansCode")]
    [XmlElement("PaymentMeansCode", Namespace = UblNamespaces.Cbc)]
    public Code? PaymentMeansCode { get; set; }

    /// <summary>
    /// Payment means description.
    /// </summary>
    [JsonPropertyName("paymentMeansDescription")]
    [XmlElement("PaymentMeansDescription", Namespace = UblNamespaces.Cbc)]
    public List<string>? PaymentMeansDescription { get; set; }

    /// <summary>
    /// Payment ID or reference.
    /// </summary>
    [JsonPropertyName("paymentId")]
    [XmlElement("PaymentID", Namespace = UblNamespaces.Cbc)]
    public List<Identifier>? PaymentId { get; set; }

    /// <summary>
    /// Payment due date.
    /// </summary>
    [JsonPropertyName("paymentDueDate")]
    [XmlElement("PaymentDueDate", Namespace = UblNamespaces.Cbc)]
    public DateTimeOffset? PaymentDueDate { get; set; }

    /// <summary>
    /// Instruction ID.
    /// </summary>
    [JsonPropertyName("instructionId")]
    [XmlElement("InstructionID", Namespace = UblNamespaces.Cbc)]
    public Identifier? InstructionId { get; set; }

    /// <summary>
    /// Instruction note.
    /// </summary>
    [JsonPropertyName("instructionNote")]
    [XmlElement("InstructionNote", Namespace = UblNamespaces.Cbc)]
    public List<string>? InstructionNote { get; set; }

    /// <summary>
    /// Card account details.
    /// </summary>
    [JsonPropertyName("cardAccount")]
    [XmlElement("CardAccount", Namespace = UblNamespaces.Cac)]
    public List<CardAccount>? CardAccount { get; set; }

    /// <summary>
    /// Payee financial account.
    /// </summary>
    [JsonPropertyName("payeeFinancialAccount")]
    [XmlElement("PayeeFinancialAccount", Namespace = UblNamespaces.Cac)]
    public FinancialAccount? PayeeFinancialAccount { get; set; }

    /// <summary>
    /// Payer financial account.
    /// </summary>
    [JsonPropertyName("payerFinancialAccount")]
    [XmlElement("PayerFinancialAccount", Namespace = UblNamespaces.Cac)]
    public FinancialAccount? PayerFinancialAccount { get; set; }
}

/// <summary>
/// UBL Card Account.
/// </summary>
[XmlType(Namespace = UblNamespaces.Cac)]
public sealed class CardAccount
{
    /// <summary>
    /// Primary account number (masked).
    /// </summary>
    [JsonPropertyName("primaryAccountNumberID")]
    [XmlElement("PrimaryAccountNumberID", Namespace = UblNamespaces.Cbc)]
    public Identifier? PrimaryAccountNumberId { get; set; }

    /// <summary>
    /// Card network ID (e.g., "VISA").
    /// </summary>
    [JsonPropertyName("networkID")]
    [XmlElement("NetworkID", Namespace = UblNamespaces.Cbc)]
    public Identifier? NetworkId { get; set; }

    /// <summary>
    /// Card type code.
    /// </summary>
    [JsonPropertyName("cardTypeCode")]
    [XmlElement("CardTypeCode", Namespace = UblNamespaces.Cbc)]
    public Code? CardTypeCode { get; set; }

    /// <summary>
    /// Validity start date.
    /// </summary>
    [JsonPropertyName("validityStartDate")]
    [XmlElement("ValidityStartDate", Namespace = UblNamespaces.Cbc)]
    public DateTimeOffset? ValidityStartDate { get; set; }

    /// <summary>
    /// Expiry date.
    /// </summary>
    [JsonPropertyName("expiryDate")]
    [XmlElement("ExpiryDate", Namespace = UblNamespaces.Cbc)]
    public DateTimeOffset? ExpiryDate { get; set; }

    /// <summary>
    /// Issuer ID.
    /// </summary>
    [JsonPropertyName("issuerId")]
    [XmlElement("IssuerID", Namespace = UblNamespaces.Cbc)]
    public Identifier? IssuerId { get; set; }

    /// <summary>
    /// Issue number ID.
    /// </summary>
    [JsonPropertyName("issueNumberId")]
    [XmlElement("IssueNumberID", Namespace = UblNamespaces.Cbc)]
    public Identifier? IssueNumberId { get; set; }

    /// <summary>
    /// CV2 ID (security code).
    /// </summary>
    [JsonPropertyName("cv2Id")]
    [XmlElement("CV2ID", Namespace = UblNamespaces.Cbc)]
    public Identifier? Cv2Id { get; set; }

    /// <summary>
    /// Card chip code.
    /// </summary>
    [JsonPropertyName("cardChipCode")]
    [XmlElement("CardChipCode", Namespace = UblNamespaces.Cbc)]
    public Code? CardChipCode { get; set; }

    /// <summary>
    /// Chip application ID.
    /// </summary>
    [JsonPropertyName("chipApplicationId")]
    [XmlElement("ChipApplicationID", Namespace = UblNamespaces.Cbc)]
    public Identifier? ChipApplicationId { get; set; }

    /// <summary>
    /// Card holder name.
    /// </summary>
    [JsonPropertyName("holderName")]
    [XmlElement("HolderName", Namespace = UblNamespaces.Cbc)]
    public string? HolderName { get; set; }

    /// <summary>
    /// Holder ID / role code.
    /// </summary>
    [JsonPropertyName("holderId")]
    [XmlElement("HolderID", Namespace = UblNamespaces.Cbc)]
    public Code? HolderId { get; set; }
}

/// <summary>
/// UBL Financial Account.
/// </summary>
[XmlType(Namespace = UblNamespaces.Cac)]
public sealed class FinancialAccount
{
    /// <summary>
    /// Account ID (e.g., IBAN).
    /// </summary>
    [JsonPropertyName("id")]
    [XmlElement("ID", Namespace = UblNamespaces.Cbc)]
    public string? Id { get; set; }

    /// <summary>
    /// Account ID scheme (e.g., "IBAN").
    /// </summary>
    [JsonPropertyName("schemeId")]
    [XmlIgnore]
    public string? SchemeId { get; set; }

    /// <summary>
    /// Account name.
    /// </summary>
    [JsonPropertyName("name")]
    [XmlElement("Name", Namespace = UblNamespaces.Cbc)]
    public string? Name { get; set; }

    /// <summary>
    /// Financial institution branch.
    /// </summary>
    [JsonPropertyName("financialInstitutionBranch")]
    [XmlElement("FinancialInstitutionBranch", Namespace = UblNamespaces.Cac)]
    public FinancialInstitutionBranch? FinancialInstitutionBranch { get; set; }
}

/// <summary>
/// UBL Financial Institution Branch.
/// </summary>
[XmlType(Namespace = UblNamespaces.Cac)]
public sealed class FinancialInstitutionBranch
{
    /// <summary>
    /// Branch ID (e.g., BIC/SWIFT).
    /// </summary>
    [JsonPropertyName("id")]
    [XmlElement("ID", Namespace = UblNamespaces.Cbc)]
    public string? Id { get; set; }

    /// <summary>
    /// Branch name.
    /// </summary>
    [JsonPropertyName("name")]
    [XmlElement("Name", Namespace = UblNamespaces.Cbc)]
    public string? Name { get; set; }
}
