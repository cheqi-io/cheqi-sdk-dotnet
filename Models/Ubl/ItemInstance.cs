using System.Text.Json.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Item Instance - a specific instance of an item.
/// </summary>
public sealed class ItemInstance
{
    /// <summary>
    /// Product trace identifier.
    /// </summary>
    [JsonPropertyName("productTraceId")]
    public Identifier? ProductTraceId { get; set; }

    /// <summary>
    /// Manufacture date.
    /// </summary>
    [JsonPropertyName("manufactureDate")]
    public DateTimeOffset? ManufactureDate { get; set; }

    /// <summary>
    /// Manufacture time.
    /// </summary>
    [JsonPropertyName("manufactureTime")]
    public TimeSpan? ManufactureTime { get; set; }

    /// <summary>
    /// Best before date.
    /// </summary>
    [JsonPropertyName("bestBeforeDate")]
    public DateTimeOffset? BestBeforeDate { get; set; }

    /// <summary>
    /// Registration identifier.
    /// </summary>
    [JsonPropertyName("registrationId")]
    public Identifier? RegistrationId { get; set; }

    /// <summary>
    /// Serial identifier.
    /// </summary>
    [JsonPropertyName("serialId")]
    public Identifier? SerialId { get; set; }

    /// <summary>
    /// Additional item properties.
    /// </summary>
    [JsonPropertyName("additionalItemProperty")]
    public List<ItemProperty>? AdditionalItemProperty { get; set; }

    /// <summary>
    /// Lot identification.
    /// </summary>
    [JsonPropertyName("lotIdentification")]
    public LotIdentification? LotIdentification { get; set; }
}
