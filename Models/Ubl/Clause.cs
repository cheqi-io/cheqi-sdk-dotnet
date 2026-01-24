using System.Text.Json.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Clause - a contractual clause.
/// </summary>
public sealed class Clause
{
    /// <summary>
    /// Clause identifier.
    /// </summary>
    [JsonPropertyName("id")]
    public Identifier? Id { get; set; }

    /// <summary>
    /// Clause content.
    /// </summary>
    [JsonPropertyName("content")]
    public List<string>? Content { get; set; }
}
