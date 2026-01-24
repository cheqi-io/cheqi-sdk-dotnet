using System.Text.Json.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Cash Register - cash register information.
/// </summary>
public sealed class CashRegister
{
    /// <summary>
    /// Cash register identifier.
    /// </summary>
    [JsonPropertyName("id")]
    public Identifier? Id { get; set; }

    /// <summary>
    /// Serial number identifier.
    /// </summary>
    [JsonPropertyName("serialNumberId")]
    public Identifier? SerialNumberId { get; set; }
}
