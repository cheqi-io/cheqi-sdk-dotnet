using System.Text.Json.Serialization;

namespace Cheqi.Sdk.Models;

/// <summary>
/// Represents a merchant-defined identifier such as:
/// - order number
/// - webshop ID
/// - POS ticket number
/// - customer reference
/// - ERP reference
/// - shipping reference
/// </summary>
public sealed class Identifier
{
    /// <summary>
    /// The type of identifier (e.g., "ORDER_NUMBER", "POS_TICKET", "CUSTOMER_REF").
    /// </summary>
    [JsonPropertyName("type")]
    public string type { get; }

    /// <summary>
    /// The identifier value.
    /// </summary>
    [JsonPropertyName("value")]
    public string value { get; }

    // ===== CONSTRUCTOR =====

    public Identifier(string type, string value)
    {
        this.type = type;
        this.value = value;
    }

    // ===== FACTORY METHOD =====

    /// <summary>
    /// Convenience factory method to create an identifier.
    /// </summary>
    /// <param name="type">The identifier type</param>
    /// <param name="value">The identifier value</param>
    /// <returns>A new Identifier instance</returns>
    public static Identifier Of(string type, string value)
    {
        return new Identifier(type, value);
    }

    // ===== EQUALS, HASHCODE, TOSTRING =====

    public override bool Equals(object? obj)
    {
        if (obj is not Identifier other) return false;
        return type == other.type && value == other.value;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(type, value);
    }

    public override string ToString()
    {
        return $"Identifier{{type='{type}', value='{value}'}}";
    }
}
