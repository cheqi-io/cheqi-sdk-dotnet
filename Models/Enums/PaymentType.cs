using System.Text.Json.Serialization;

namespace Cheqi.Sdk.Models.Enums;

/// <summary>
/// Payment method types supported by the Cheqi SDK.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PaymentType
{
    /// <summary>
    /// Card payment (credit/debit) - UBL code 48.
    /// </summary>
    CARD_PAYMENT,

    /// <summary>
    /// Direct debit payment - UBL code 49.
    /// </summary>
    DIRECT_DEBIT,

    /// <summary>
    /// Cash payment - UBL code 10.
    /// </summary>
    CASH
}

/// <summary>
/// Extension methods for PaymentType enum.
/// </summary>
public static class PaymentTypeExtensions
{
    /// <summary>
    /// Gets the UBL payment means code for the payment type.
    /// </summary>
    public static string GetValue(this PaymentType paymentType)
    {
        return paymentType switch
        {
            PaymentType.CARD_PAYMENT => "48",
            PaymentType.DIRECT_DEBIT => "49",
            PaymentType.CASH => "10",
            _ => throw new ArgumentOutOfRangeException(nameof(paymentType), paymentType, null)
        };
    }

    /// <summary>
    /// Parses a UBL payment means code to PaymentType enum.
    /// </summary>
    public static PaymentType FromValue(string value)
    {
        return value switch
        {
            "48" => PaymentType.CARD_PAYMENT,
            "49" => PaymentType.DIRECT_DEBIT,
            "10" => PaymentType.CASH,
            _ => throw new ArgumentException($"Unknown payment type value: {value}", nameof(value))
        };
    }
}
