using System.Text.Json.Serialization;

namespace Cheqi.Sdk.Models;

/// <summary>
/// Response from the recipient resolution (customer matching) API.
/// </summary>
public sealed class RecipientResolutionResponse
{
    /// <summary>
    /// Whether matching recipients were found.
    /// </summary>
    [JsonPropertyName("found")]
    public bool Found { get; set; }

    /// <summary>
    /// List of matched recipients with their public keys.
    /// </summary>
    [JsonPropertyName("recipients")]
    public List<Recipient> Recipients { get; set; } = new();

    /// <summary>
    /// Message providing additional context.
    /// </summary>
    [JsonPropertyName("message")]
    public string? Message { get; set; }

    /// <summary>
    /// Returns true if recipients were found.
    /// </summary>
    [JsonIgnore]
    public bool HasRecipients => Found && Recipients.Count > 0;
}
