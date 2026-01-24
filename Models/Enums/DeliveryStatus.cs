using System.Text.Json.Serialization;

namespace Cheqi.Sdk.Models.Enums;

/// <summary>
/// Status of receipt delivery.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum DeliveryStatus
{
    /// <summary>
    /// Receipt was delivered successfully.
    /// </summary>
    DELIVERED,

    /// <summary>
    /// Receipt is pending delivery.
    /// </summary>
    PENDING,

    /// <summary>
    /// Receipt delivery failed.
    /// </summary>
    FAILED,

    /// <summary>
    /// No recipient found for the identifier.
    /// </summary>
    NO_RECIPIENT
}
