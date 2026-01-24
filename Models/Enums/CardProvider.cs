using System.Text.Json.Serialization;

namespace Cheqi.Sdk.Models.Enums;

/// <summary>
/// Card network providers.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CardProvider
{
    VISA,
    MASTERCARD,
    AMEX,
    DISCOVER,
    DINERS,
    JCB,
    UNIONPAY,
    MAESTRO,
    CARTES_BANCAIRES,
    OTHER
}
