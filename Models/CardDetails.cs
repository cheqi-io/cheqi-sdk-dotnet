using System.Text.Json.Serialization;
using Cheqi.Sdk.Models.Enums;

namespace Cheqi.Sdk.Models;

/// <summary>
/// Card details DTO representing payment card information.
///
/// <para><b>Mandatory Fields:</b></para>
/// <list type="bullet">
///   <item><b>cardProvider</b>: The card provider (VISA, MASTERCARD, etc.)</item>
/// </list>
///
/// <para><b>Optional Fields:</b></para>
/// <list type="bullet">
///   <item><b>paymentAccountNumber</b>: Primary Account Number (PAN)</item>
///   <item><b>paymentAccountReference</b>: Payment Account Reference (PAR)</item>
/// </list>
/// </summary>
public sealed class CardDetails
{
    // ===== MANDATORY FIELDS =====

    /// <summary>
    /// The card provider (VISA, MASTERCARD, etc.).
    /// </summary>
    [JsonPropertyName("cardProvider")]
    public CardProvider cardProvider { get; }

    // ===== OPTIONAL FIELDS =====

    /// <summary>
    /// Primary Account Number (PAN) of the card.
    /// </summary>
    [JsonPropertyName("paymentAccountNumber")]
    public string? paymentAccountNumber { get; }

    /// <summary>
    /// Payment Account Reference (PAR) - a unique identifier for the card.
    /// </summary>
    [JsonPropertyName("paymentAccountReference")]
    public string? paymentAccountReference { get; }

    // ===== CONSTRUCTOR =====

    public CardDetails(
        CardProvider cardProvider,
        string? paymentAccountNumber = null,
        string? paymentAccountReference = null)
    {
        this.cardProvider = cardProvider;
        this.paymentAccountNumber = paymentAccountNumber;
        this.paymentAccountReference = paymentAccountReference;
    }

    public CardProvider CardProvider => cardProvider;

    public string? PaymentAccountNumber => paymentAccountNumber;

    public string? PaymentAccountReference => paymentAccountReference;

    // ===== BUILDER =====

    public static CardDetailsBuilder Builder() => new CardDetailsBuilder();

    public sealed class CardDetailsBuilder
    {
        private CardProvider? _cardProvider;
        private string? _paymentAccountNumber;
        private string? _paymentAccountReference;

        internal CardDetailsBuilder() { }

        public CardDetailsBuilder From(CardDetails other)
        {
            _cardProvider = other.cardProvider;
            _paymentAccountNumber = other.paymentAccountNumber;
            _paymentAccountReference = other.paymentAccountReference;
            return this;
        }

        public CardDetailsBuilder CardProvider(CardProvider cardProvider)
        {
            _cardProvider = cardProvider;
            return this;
        }

        public CardDetailsBuilder PaymentAccountNumber(string? paymentAccountNumber)
        {
            _paymentAccountNumber = paymentAccountNumber;
            return this;
        }

        public CardDetailsBuilder PaymentAccountReference(string? paymentAccountReference)
        {
            _paymentAccountReference = paymentAccountReference;
            return this;
        }

        public CardDetails Build()
        {
            if (_cardProvider == null)
                throw new InvalidOperationException("CardProvider is required");

            return new CardDetails(
                _cardProvider.Value,
                _paymentAccountNumber,
                _paymentAccountReference
            );
        }
    }

    // ===== EQUALS, HASHCODE, TOSTRING =====

    public override bool Equals(object? obj)
    {
        if (obj is not CardDetails other) return false;
        return cardProvider == other.cardProvider
            && paymentAccountNumber == other.paymentAccountNumber
            && paymentAccountReference == other.paymentAccountReference;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(cardProvider, paymentAccountNumber, paymentAccountReference);
    }

    public override string ToString()
    {
        return $"CardDetails{{cardProvider={cardProvider}, " +
               $"paymentAccountNumber={paymentAccountNumber}, " +
               $"paymentAccountReference={paymentAccountReference}}}";
    }
}
