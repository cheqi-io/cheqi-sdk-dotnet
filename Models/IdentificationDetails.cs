using System.Text.Json.Serialization;
using Cheqi.Sdk.Models.Enums;

namespace Cheqi.Sdk.Models;

/// <summary>
/// Payment details DTO containing complete payment information for UBL PaymentMeans generation.
///
/// <para><b>Mandatory Fields:</b></para>
/// <list type="bullet">
///   <item><b>paymentType</b>: The type of payment being processed</item>
/// </list>
///
/// <para><b>Optional Fields:</b></para>
/// <list type="bullet">
///   <item><b>cardDetails</b>: Card details for card-based payments</item>
///   <item><b>paymentAccountDetails</b>: Payment account details for direct debit payments</item>
///   <item><b>paymentIds</b>: List of payment identifiers</item>
///   <item><b>recipientEmail</b>: Customer email for identification</item>
/// </list>
/// </summary>
public sealed class IdentificationDetails
{
    // ===== MANDATORY FIELDS =====

    /// <summary>
    /// The type of payment being processed.
    /// </summary>
    [JsonPropertyName("paymentType")]
    public PaymentType paymentType { get; }

    // ===== OPTIONAL FIELDS =====

    /// <summary>
    /// Card details for card-based payments.
    /// </summary>
    [JsonPropertyName("cardDetails")]
    public CardDetails? cardDetails { get; }

    /// <summary>
    /// Payment account details for direct debit payments.
    /// </summary>
    [JsonPropertyName("paymentAccountDetails")]
    public PaymentAccountDetails? paymentAccountDetails { get; }

    /// <summary>
    /// List of payment identifiers.
    /// </summary>
    [JsonPropertyName("paymentIds")]
    public IReadOnlyList<string>? paymentIds { get; }

    /// <summary>
    /// Customer email for identification.
    /// </summary>
    [JsonPropertyName("recipientEmail")]
    public string? recipientEmail { get; }

    /// <summary>
    /// Cheqi Receipt ID for direct delivery.
    /// </summary>
    [JsonPropertyName("cheqiReceiptId")]
    public string? cheqiReceiptId { get; }

    // ===== CONSTRUCTOR =====

    private IdentificationDetails(
        PaymentType paymentType,
        CardDetails? cardDetails,
        PaymentAccountDetails? paymentAccountDetails,
        List<string>? paymentIds,
        string? recipientEmail,
        string? cheqiReceiptId)
    {
        this.paymentType = paymentType;
        this.cardDetails = cardDetails;
        this.paymentAccountDetails = paymentAccountDetails;
        this.paymentIds = paymentIds?.AsReadOnly();
        this.recipientEmail = recipientEmail;
        this.cheqiReceiptId = cheqiReceiptId;
    }

    // ===== BUILDER =====

    public static IdentificationDetailsBuilder Builder() => new IdentificationDetailsBuilder();

    public sealed class IdentificationDetailsBuilder
    {
        private PaymentType? _paymentType;
        private CardDetails? _cardDetails;
        private PaymentAccountDetails? _paymentAccountDetails;
        private List<string>? _paymentIds;
        private string? _recipientEmail;
        private string? _cheqiReceiptId;

        internal IdentificationDetailsBuilder() { }

        public IdentificationDetailsBuilder From(IdentificationDetails other)
        {
            _paymentType = other.paymentType;
            _cardDetails = other.cardDetails;
            _paymentAccountDetails = other.paymentAccountDetails;
            _paymentIds = other.paymentIds?.ToList();
            _recipientEmail = other.recipientEmail;
            _cheqiReceiptId = other.cheqiReceiptId;
            return this;
        }

        public IdentificationDetailsBuilder PaymentType(PaymentType paymentType)
        {
            _paymentType = paymentType;
            return this;
        }

        public IdentificationDetailsBuilder CardDetails(CardDetails? cardDetails)
        {
            _cardDetails = cardDetails;
            return this;
        }

        public IdentificationDetailsBuilder PaymentAccountDetails(PaymentAccountDetails? paymentAccountDetails)
        {
            _paymentAccountDetails = paymentAccountDetails;
            return this;
        }

        public IdentificationDetailsBuilder PaymentIds(List<string>? paymentIds)
        {
            _paymentIds = paymentIds;
            return this;
        }

        public IdentificationDetailsBuilder AddPaymentId(string paymentId)
        {
            _paymentIds ??= new List<string>();
            _paymentIds.Add(paymentId);
            return this;
        }

        public IdentificationDetailsBuilder RecipientEmail(string? recipientEmail)
        {
            _recipientEmail = recipientEmail;
            return this;
        }

        public IdentificationDetailsBuilder CheqiReceiptId(string? cheqiReceiptId)
        {
            _cheqiReceiptId = cheqiReceiptId;
            return this;
        }

        public IdentificationDetails Build()
        {
            if (_paymentType == null)
                throw new InvalidOperationException("PaymentType is required");

            return new IdentificationDetails(
                _paymentType.Value,
                _cardDetails,
                _paymentAccountDetails,
                _paymentIds,
                _recipientEmail,
                _cheqiReceiptId
            );
        }
    }

    // ===== EQUALS, HASHCODE, TOSTRING =====

    public override bool Equals(object? obj)
    {
        if (obj is not IdentificationDetails other) return false;
        return paymentType == other.paymentType
            && Equals(cardDetails, other.cardDetails)
            && Equals(paymentAccountDetails, other.paymentAccountDetails)
            && (paymentIds == null && other.paymentIds == null || 
                paymentIds != null && other.paymentIds != null && paymentIds.SequenceEqual(other.paymentIds))
            && recipientEmail == other.recipientEmail
            && cheqiReceiptId == other.cheqiReceiptId;
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(paymentType);
        hash.Add(cardDetails);
        hash.Add(paymentAccountDetails);
        if (paymentIds != null)
        {
            foreach (var id in paymentIds)
                hash.Add(id);
        }
        hash.Add(recipientEmail);
        hash.Add(cheqiReceiptId);
        return hash.ToHashCode();
    }

    public override string ToString()
    {
        return $"IdentificationDetails{{" +
               $"paymentType={paymentType}, " +
               $"cardDetails={cardDetails}, " +
               $"paymentAccountDetails={paymentAccountDetails}, " +
               $"paymentIds={paymentIds?.Count ?? 0} items, " +
               $"recipientEmail={recipientEmail}, " +
               $"cheqiReceiptId={cheqiReceiptId}}}";
    }
}
