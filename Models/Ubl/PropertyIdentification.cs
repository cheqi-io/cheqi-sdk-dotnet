using System.Text.Json.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Property Identification - identification of a property.
/// </summary>
public sealed class PropertyIdentification
{
    /// <summary>
    /// Property identifier.
    /// </summary>
    [JsonPropertyName("id")]
    public Identifier? Id { get; set; }

    /// <summary>
    /// Issuer scope identifier.
    /// </summary>
    [JsonPropertyName("issuerScopeId")]
    public Identifier? IssuerScopeId { get; set; }

    /// <summary>
    /// Issuer party.
    /// </summary>
    [JsonPropertyName("issuerParty")]
    public Party? IssuerParty { get; set; }
}
