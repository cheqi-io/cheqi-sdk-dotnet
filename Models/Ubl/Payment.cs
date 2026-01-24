using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Payment - payment information.
/// </summary>
[XmlType(Namespace = UblNamespaces.Cac)]
public sealed class Payment
{
    /// <summary>
    /// Payment identifier.
    /// </summary>
    [JsonPropertyName("id")]
    [XmlElement("ID", Namespace = UblNamespaces.Cbc)]
    public Identifier? Id { get; set; }

    /// <summary>
    /// Paid amount.
    /// </summary>
    [JsonPropertyName("paidAmount")]
    [XmlElement("PaidAmount", Namespace = UblNamespaces.Cbc)]
    public Amount? PaidAmount { get; set; }

    /// <summary>
    /// Paid cash amount.
    /// </summary>
    [JsonPropertyName("paidCashAmount")]
    [XmlElement("PaidCashAmount", Namespace = UblNamespaces.Cbc)]
    public Amount? PaidCashAmount { get; set; }

    /// <summary>
    /// Cash change amount.
    /// </summary>
    [JsonPropertyName("cashChangeAmount")]
    [XmlElement("CashChangeAmount", Namespace = UblNamespaces.Cbc)]
    public Amount? CashChangeAmount { get; set; }

    /// <summary>
    /// Received date.
    /// </summary>
    [JsonPropertyName("receivedDate")]
    [XmlElement("ReceivedDate", Namespace = UblNamespaces.Cbc)]
    public DateTimeOffset? ReceivedDate { get; set; }

    /// <summary>
    /// Paid date.
    /// </summary>
    [JsonPropertyName("paidDate")]
    [XmlElement("PaidDate", Namespace = UblNamespaces.Cbc)]
    public DateTimeOffset? PaidDate { get; set; }

    /// <summary>
    /// Paid time.
    /// </summary>
    [JsonPropertyName("paidTime")]
    [XmlElement("PaidTime", Namespace = UblNamespaces.Cbc)]
    public TimeSpan? PaidTime { get; set; }

    /// <summary>
    /// Instruction identifier.
    /// </summary>
    [JsonPropertyName("instructionId")]
    [XmlElement("InstructionID", Namespace = UblNamespaces.Cbc)]
    public Identifier? InstructionId { get; set; }

    /// <summary>
    /// Merchant identifier.
    /// </summary>
    [JsonPropertyName("merchantId")]
    [XmlElement("MerchantID", Namespace = UblNamespaces.Cbc)]
    public Identifier? MerchantId { get; set; }

    /// <summary>
    /// Authorization identifier.
    /// </summary>
    [JsonPropertyName("authorizationId")]
    [XmlElement("AuthorizationID", Namespace = UblNamespaces.Cbc)]
    public Identifier? AuthorizationId { get; set; }

    /// <summary>
    /// Transaction identifier.
    /// </summary>
    [JsonPropertyName("transactionId")]
    [XmlElement("TransactionID", Namespace = UblNamespaces.Cbc)]
    public Identifier? TransactionId { get; set; }

    /// <summary>
    /// Payment terminal identifier.
    /// </summary>
    [JsonPropertyName("paymentTerminalId")]
    [XmlElement("PaymentTerminalID", Namespace = UblNamespaces.Cbc)]
    public Identifier? PaymentTerminalId { get; set; }
}
