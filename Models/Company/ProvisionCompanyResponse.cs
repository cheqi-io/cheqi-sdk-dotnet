using System.Text.Json.Serialization;

namespace Cheqi.Sdk.Models.Company;

/// <summary>
/// Response from company provisioning.
/// </summary>
public sealed class ProvisionCompanyResponse
{
    /// <summary>
    /// Whether provisioning was successful.
    /// </summary>
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    /// <summary>
    /// The provisioned company details.
    /// </summary>
    [JsonPropertyName("company")]
    public Company? Company { get; set; }

    /// <summary>
    /// OAuth client ID for API access.
    /// </summary>
    [JsonPropertyName("clientId")]
    public string? ClientId { get; set; }

    /// <summary>
    /// OAuth client secret for API access.
    /// </summary>
    [JsonPropertyName("clientSecret")]
    public string? ClientSecret { get; set; }

    /// <summary>
    /// API key for direct access.
    /// </summary>
    [JsonPropertyName("apiKey")]
    public string? ApiKey { get; set; }

    /// <summary>
    /// Message providing additional context.
    /// </summary>
    [JsonPropertyName("message")]
    public string? Message { get; set; }
}
