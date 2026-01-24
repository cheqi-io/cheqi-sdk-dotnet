using System.Text.Json.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Physical Attribute - a physical attribute of an item.
/// </summary>
public sealed class PhysicalAttribute
{
    /// <summary>
    /// Attribute identifier.
    /// </summary>
    [JsonPropertyName("attributeId")]
    public Identifier? AttributeId { get; set; }

    /// <summary>
    /// Position code.
    /// </summary>
    [JsonPropertyName("positionCode")]
    public Code? PositionCode { get; set; }

    /// <summary>
    /// Description code.
    /// </summary>
    [JsonPropertyName("descriptionCode")]
    public Code? DescriptionCode { get; set; }

    /// <summary>
    /// Descriptions.
    /// </summary>
    [JsonPropertyName("description")]
    public List<string>? Description { get; set; }
}
