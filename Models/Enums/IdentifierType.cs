using System.Text.Json.Serialization;

namespace Cheqi.Sdk.Models.Enums;

/// <summary>
/// Type of customer identifier.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum IdentifierType
{
    /// <summary>
    /// Payment card identifier.
    /// </summary>
    CARD,

    /// <summary>
    /// IBAN bank account identifier.
    /// </summary>
    IBAN,

    /// <summary>
    /// Email address identifier.
    /// </summary>
    EMAIL
}
