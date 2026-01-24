using System.Text.Json.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Financial Institution - a financial institution (bank).
/// </summary>
public sealed class FinancialInstitution
{
    /// <summary>
    /// Institution identifier.
    /// </summary>
    [JsonPropertyName("id")]
    public Identifier? Id { get; set; }

    /// <summary>
    /// Institution name.
    /// </summary>
    [JsonPropertyName("name")]
    public Name? Name { get; set; }

    /// <summary>
    /// Institution address.
    /// </summary>
    [JsonPropertyName("address")]
    public Address? Address { get; set; }
}
