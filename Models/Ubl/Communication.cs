using System.Text.Json.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Communication - communication channel information.
/// </summary>
public sealed class Communication
{
    /// <summary>
    /// Channel code.
    /// </summary>
    [JsonPropertyName("channelCode")]
    public Code? ChannelCode { get; set; }

    /// <summary>
    /// Channel description.
    /// </summary>
    [JsonPropertyName("channel")]
    public string? Channel { get; set; }

    /// <summary>
    /// Communication value (e.g., email address, phone number).
    /// </summary>
    [JsonPropertyName("value")]
    public string? Value { get; set; }
}
