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
    [JsonPropertyName("storeName")]
    public string StoreName { get; set; } = string.Empty;

    /// <summary>
    /// Store code or identifier in the merchant's system.
    /// </summary>
    [JsonPropertyName("storeCode")]
    public string? StoreCode { get; set; }

    /// <summary>
    /// Store address.
    /// </summary>
    [JsonPropertyName("address")]
    public Address? Address { get; set; }

    /// <summary>
    /// Store phone number.
    /// </summary>
    [JsonPropertyName("phoneNumber")]
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Store email address.
    /// </summary>
    [JsonPropertyName("email")]
    public string? Email { get; set; }

    /// <summary>
    /// Store opening hours.
    /// </summary>
    [JsonPropertyName("openingHours")]
    public string? OpeningHours { get; set; }

    /// <summary>
    /// Validates the store request.
    /// </summary>
    public bool IsValid() => GetValidationErrors().Count == 0;

    /// <summary>
    /// Gets validation errors for the store request.
    /// </summary>
    public List<string> GetValidationErrors()
    {
        var errors = new List<string>();
        
        if (string.IsNullOrWhiteSpace(StoreName))
            errors.Add("storeName is required");
        
        if (Address == null)
            errors.Add("address is required");
        else if (!Address.IsValid())
        {
            foreach (var error in Address.GetValidationErrors())
                errors.Add($"address.{error}");
        }
        
        return errors;
    }
}
