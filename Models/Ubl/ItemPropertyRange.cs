using System.Text.Json.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Item Property Range - a range for an item property value.
/// </summary>
public sealed class ItemPropertyRange
{
    /// <summary>
    /// Minimum value.
    /// </summary>
    [JsonPropertyName("minimumValue")]
    public string? MinimumValue { get; set; }

    /// <summary>
    /// Maximum value.
    /// </summary>
    [JsonPropertyName("maximumValue")]
    public string? MaximumValue { get; set; }
}
