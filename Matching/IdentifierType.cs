using System.Text.Json.Serialization;

namespace Cheqi.Sdk.Matching;

/// <summary>
/// Types of customer identifiers supported for matching.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum IdentifierType
{
    /// <summary>
    /// Payment Account Reference - tokenized card identifier.
    /// </summary>
    PAR,
    
    /// <summary>
    /// Customer email address.
    /// </summary>
    EMAIL,
    
    /// <summary>
    /// Primary Account Number - card number.
    /// </summary>
    PAN,
    
    /// <summary>
    /// International Bank Account Number.
    /// </summary>
    IBAN
}
