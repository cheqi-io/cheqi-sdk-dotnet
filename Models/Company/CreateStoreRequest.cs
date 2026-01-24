using System.Text.Json.Serialization;

namespace Cheqi.Sdk.Models.Company;

/// <summary>
/// Request for creating a new store.
/// </summary>
public sealed class CreateStoreRequest
{
    /// <summary>
    /// Store name.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Store code or identifier in the merchant's system.
    /// </summary>
    [JsonPropertyName("storeCode")]
    public string? StoreCode { get; set; }

    /// <summary>
    /// Store email address.
    /// </summary>
    [JsonPropertyName("email")]
    public string? Email { get; set; }

    /// <summary>
    /// Store phone number.
    /// </summary>
    [JsonPropertyName("phone")]
    public string? Phone { get; set; }

    /// <summary>
    /// Store address.
    /// </summary>
    [JsonPropertyName("address")]
    public Address? Address { get; set; }

    /// <summary>
    /// Store timezone.
    /// </summary>
    [JsonPropertyName("timezone")]
    public string? Timezone { get; set; }
}
