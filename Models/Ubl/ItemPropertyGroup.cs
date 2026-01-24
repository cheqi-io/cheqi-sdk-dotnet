using System.Text.Json.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Item Property Group - a group of item properties.
/// </summary>
public sealed class ItemPropertyGroup
{
    /// <summary>
    /// Group identifier.
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    /// <summary>
    /// Group name.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// Importance code.
    /// </summary>
    [JsonPropertyName("importanceCode")]
    public string? ImportanceCode { get; set; }
}
