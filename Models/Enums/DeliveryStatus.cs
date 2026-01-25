using System.Text.Json.Serialization;

namespace Cheqi.Sdk.Models.Enums;

/// <summary>
/// Status of receipt delivery.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum DeliveryStatus
{
    /// <summary>
    /// Receipt successfully delivered to customer's Cheqi app/devices.
    /// </summary>
    DELIVERED_DIGITAL,

    /// <summary>
    /// Receipt sent via email (fallback).
    /// </summary>
    DELIVERED_EMAIL,

    /// <summary>
    /// Customer not found - email required.
    /// </summary>
    CUSTOMER_NOT_FOUND,

    /// <summary>
    /// Processing failed.
    /// </summary>
    FAILED
}
