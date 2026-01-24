using System.Text.Json.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL External Reference - a reference to an external resource.
/// </summary>
public sealed class ExternalReference
{
    /// <summary>
    /// URI of the external reference.
    /// </summary>
    [JsonPropertyName("uri")]
    public string? Uri { get; set; }
}
