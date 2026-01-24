using System.Text.Json.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Party Name - a party name.
/// </summary>
public sealed class PartyName
{
    /// <summary>
    /// The party name.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }
}
