using System.Text.Json.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Lot Identification - identification of a lot/batch.
/// </summary>
public sealed class LotIdentification
{
    /// <summary>
    /// Lot number identifier.
    /// </summary>
    [JsonPropertyName("lotNumberId")]
    public Identifier? LotNumberId { get; set; }

    /// <summary>
    /// Expiry date.
    /// </summary>
    [JsonPropertyName("expiryDate")]
    public DateTimeOffset? ExpiryDate { get; set; }

    /// <summary>
    /// Additional item properties.
    /// </summary>
    [JsonPropertyName("additionalItemProperty")]
    public List<ItemProperty>? AdditionalItemProperty { get; set; }
}
