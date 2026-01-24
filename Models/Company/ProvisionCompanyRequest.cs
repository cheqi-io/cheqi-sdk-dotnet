using System.Text.Json.Serialization;

namespace Cheqi.Sdk.Models.Company;

/// <summary>
/// Request for provisioning a new company.
/// </summary>
public sealed class ProvisionCompanyRequest
{
    /// <summary>
    /// Company details to provision.
    /// </summary>
    [JsonPropertyName("company")]
    public Company Company { get; set; } = new();

    /// <summary>
    /// Initial admin user email.
    /// </summary>
    [JsonPropertyName("adminEmail")]
    public string? AdminEmail { get; set; }

    /// <summary>
    /// Whether to create sandbox credentials.
    /// </summary>
    [JsonPropertyName("sandbox")]
    public bool Sandbox { get; set; }

    /// <summary>
    /// Webhook URL for receiving notifications.
    /// </summary>
    [JsonPropertyName("webhookUrl")]
    public string? WebhookUrl { get; set; }
}
