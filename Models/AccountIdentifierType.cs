using System.Text.Json.Serialization;

namespace Cheqi.Sdk.Models;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AccountIdentifierType
{
    IBAN
}
