using System.Text.Json.Serialization;

namespace Cheqi.Sdk.CreditNote;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum RefundPreference
{
    ORIGINAL_PAYMENT_METHOD,
    BANK_TRANSFER,
    STORE_CREDIT
}
