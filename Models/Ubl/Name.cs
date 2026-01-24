using System.Text.Json.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Name - a name value with optional language information.
/// </summary>
public sealed class Name
{
    /// <summary>
    /// The name value.
    /// </summary>
    [JsonPropertyName("value")]
    public string? Value { get; set; }

    /// <summary>
    /// Language ID.
    /// </summary>
    [JsonPropertyName("languageId")]
    public string? LanguageId { get; set; }

    /// <summary>
    /// Language locale ID.
    /// </summary>
    [JsonPropertyName("languageLocaleId")]
    public string? LanguageLocaleId { get; set; }
}
