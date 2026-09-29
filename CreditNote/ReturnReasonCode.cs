using System.Text.Json.Serialization;
namespace Cheqi.Sdk.CreditNote;
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ReturnReasonCode { DEFECTIVE, DAMAGED, WRONG_ITEM, NOT_AS_DESCRIBED, UNWANTED, EXPIRED, INCOMPLETE }
