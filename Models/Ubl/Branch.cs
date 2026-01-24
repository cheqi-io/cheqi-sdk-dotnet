using System.Text.Json.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Branch - a branch of a financial institution.
/// </summary>
public sealed class Branch
{
    /// <summary>
    /// Branch identifier.
    /// </summary>
    [JsonPropertyName("id")]
    public Identifier? Id { get; set; }

    /// <summary>
    /// Branch name.
    /// </summary>
    [JsonPropertyName("name")]
    public Name? Name { get; set; }

    /// <summary>
    /// Financial institution.
    /// </summary>
    [JsonPropertyName("financialInstitution")]
    public FinancialInstitution? FinancialInstitution { get; set; }

    /// <summary>
    /// Branch address.
    /// </summary>
    [JsonPropertyName("address")]
    public Address? Address { get; set; }
}
