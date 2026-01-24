using System.Text.Json.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Item Identification - identification of an item.
/// </summary>
public sealed class ItemIdentification
{
    /// <summary>
    /// Item identifier.
    /// </summary>
    [JsonPropertyName("id")]
    public Identifier? Id { get; set; }

    /// <summary>
    /// Extended identifier.
    /// </summary>
    [JsonPropertyName("extendedId")]
    public Identifier? ExtendedId { get; set; }

    /// <summary>
    /// Barcode symbology identifier.
    /// </summary>
    [JsonPropertyName("barcodeSymbologyId")]
    public Identifier? BarcodeSymbologyId { get; set; }

    /// <summary>
    /// Issuer scope identifier.
    /// </summary>
    [JsonPropertyName("issuerScopeId")]
    public Identifier? IssuerScopeId { get; set; }

    /// <summary>
    /// Physical attributes.
    /// </summary>
    [JsonPropertyName("physicalAttribute")]
    public List<PhysicalAttribute>? PhysicalAttribute { get; set; }

    /// <summary>
    /// Measurement dimensions.
    /// </summary>
    [JsonPropertyName("measurementDimension")]
    public List<Dimension>? MeasurementDimension { get; set; }

    /// <summary>
    /// Issuer party.
    /// </summary>
    [JsonPropertyName("issuerParty")]
    public Party? IssuerParty { get; set; }
}
