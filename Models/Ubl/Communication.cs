using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Communication - communication channel information.
/// </summary>
[XmlType(Namespace = UblNamespaces.Cac)]
public sealed class Communication
{
    /// <summary>
    /// Channel code.
    /// </summary>
    [JsonPropertyName("channelCode")]
    [XmlElement("ChannelCode", Namespace = UblNamespaces.Cbc)]
    public Code? ChannelCode { get; set; }

    /// <summary>
    /// Channel description.
    /// </summary>
    [JsonPropertyName("channel")]
    [XmlElement("Channel", Namespace = UblNamespaces.Cbc)]
    public string? Channel { get; set; }

    /// <summary>
    /// Communication value (e.g., email address, phone number).
    /// </summary>
    [JsonPropertyName("value")]
    [XmlElement("Value", Namespace = UblNamespaces.Cbc)]
    public string? Value { get; set; }
}
