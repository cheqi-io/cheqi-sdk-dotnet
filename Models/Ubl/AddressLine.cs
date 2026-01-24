using System.Text.Json.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Address Line - a free-form address line.
/// </summary>
public sealed class AddressLine
{
    /// <summary>
    /// The address line text.
    /// </summary>
    [JsonPropertyName("line")]
    public string? Line { get; set; }
}
