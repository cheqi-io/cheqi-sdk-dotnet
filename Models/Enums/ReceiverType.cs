using System.Text.Json.Serialization;

namespace Cheqi.Sdk.Models.Enums;

/// <summary>
/// Type of receipt receiver.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum ReceiverType
{
    /// <summary>
    /// Individual customer.
    /// </summary>
    CUSTOMER,

    /// <summary>
    /// Business entity.
    /// </summary>
    BUSINESS
}
