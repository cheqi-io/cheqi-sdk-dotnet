using System.Text.Json.Serialization;

namespace Cheqi.Sdk.Models.Enums;

/// <summary>
/// Card network providers.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CardProvider
{
    /// <summary>
    /// Maestro debit card network.
    /// </summary>
    MAESTRO,

    /// <summary>
    /// Mastercard credit/debit card network.
    /// </summary>
    MASTERCARD,

    /// <summary>
    /// Visa credit/debit card network.
    /// </summary>
    VISA,

    /// <summary>
    /// American Express card network.
    /// </summary>
    AMERICAN_EXPRESS
}

/// <summary>
/// Extension methods for CardProvider enum.
/// </summary>
public static class CardProviderExtensions
{
    /// <summary>
    /// Parses a string value to CardProvider enum, handling common variations.
    /// </summary>
    /// <param name="value">The string value to parse</param>
    /// <returns>The matching CardProvider, or null if the value is null</returns>
    /// <exception cref="ArgumentException">if the value is not recognized</exception>
    public static CardProvider? FromString(string? value)
    {
        if (value == null)
            return null;

        // Normalize the input: convert to uppercase and remove spaces/hyphens
        var normalized = value.ToUpperInvariant()
            .Replace(" ", "_")
            .Replace("-", "_");

        // Handle common variations
        return normalized switch
        {
            "MASTERCARD" or "MASTER_CARD" or "MASTER" => CardProvider.MASTERCARD,
            "VISA" => CardProvider.VISA,
            "MAESTRO" => CardProvider.MAESTRO,
            "AMERICAN_EXPRESS" or "AMERICANEXPRESS" or "AMERICAN EXPRESS" or "AMEX" => CardProvider.AMERICAN_EXPRESS,
            _ => Enum.TryParse<CardProvider>(normalized, true, out var result)
                ? result
                : throw new ArgumentException($"Unknown CardProvider: {value}. Valid values are: VISA, MASTERCARD, MAESTRO, AMERICAN_EXPRESS")
        };
    }

    /// <summary>
    /// Gets the string value for JSON serialization.
    /// </summary>
    public static string ToValue(this CardProvider provider)
    {
        return provider.ToString();
    }
}
