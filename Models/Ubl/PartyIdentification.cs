using System.Text.Json.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Party Identification - party identification.
/// </summary>
public sealed class PartyIdentification
{
    /// <summary>
    /// Party identifier.
    /// </summary>
    [JsonPropertyName("id")]
    public Identifier? Id { get; set; }
}
