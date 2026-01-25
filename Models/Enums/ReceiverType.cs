using System.Text.Json.Serialization;

namespace Cheqi.Sdk.Models.Enums;

/// <summary>
/// Type of receipt receiver.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ReceiverType
{
    /// <summary>
    /// Webhook receiver - receipt delivered via HTTP webhook.
    /// </summary>
    WEBHOOK,

    /// <summary>
    /// Device receiver - receipt delivered to a registered device.
    /// </summary>
    DEVICE
}
