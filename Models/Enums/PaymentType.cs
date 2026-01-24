using System.Text.Json.Serialization;

namespace Cheqi.Sdk.Models.Enums;

/// <summary>
/// Payment method types supported by the Cheqi SDK.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PaymentType
{
    /// <summary>
    /// Card payment (credit/debit).
    /// </summary>
    CARD_PAYMENT,

    /// <summary>
    /// Bank transfer payment.
    /// </summary>
    BANK_TRANSFER,

    /// <summary>
    /// Cash payment.
    /// </summary>
    CASH,

    /// <summary>
    /// Direct debit payment.
    /// </summary>
    DIRECT_DEBIT,

    /// <summary>
    /// Gift card payment.
    /// </summary>
    GIFT_CARD,

    /// <summary>
    /// Loyalty points payment.
    /// </summary>
    LOYALTY_POINTS,

    /// <summary>
    /// Mobile payment (Apple Pay, Google Pay, etc.).
    /// </summary>
    MOBILE_PAYMENT,

    /// <summary>
    /// Other payment method.
    /// </summary>
    OTHER
}
