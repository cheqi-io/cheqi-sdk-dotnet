using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Item Identification - identification of an item.
/// </summary>
[XmlType(Namespace = UblNamespaces.Cac)]
public sealed class ItemIdentification
{
    /// <summary>
    /// Item identifier.
    /// </summary>
    [JsonPropertyName("id")]
    [XmlElement("ID", Namespace = UblNamespaces.Cbc)]
    public Identifier? Id { get; set; }

    /// <summary>
    /// Extended identifier.
    /// </summary>
    [JsonPropertyName("extendedId")]
    [XmlElement("ExtendedID", Namespace = UblNamespaces.Cbc)]
    public Identifier? ExtendedId { get; set; }

    /// <summary>
    /// Barcode symbology identifier.
    /// </summary>
    [JsonPropertyName("barcodeSymbologyId")]
    [XmlElement("BarcodeSymbologyID", Namespace = UblNamespaces.Cbc)]
    public Identifier? BarcodeSymbologyId { get; set; }

    /// <summary>
    /// Issuer scope identifier.
    /// </summary>
    [JsonPropertyName("issuerScopeId")]
    [XmlElement("IssuerScopeID", Namespace = UblNamespaces.Cbc)]
    public Identifier? IssuerScopeId { get; set; }

    /// <summary>
    /// Physical attributes.
    /// </summary>
    [JsonPropertyName("physicalAttribute")]
    [XmlElement("PhysicalAttribute", Namespace = UblNamespaces.Cac)]
    public List<PhysicalAttribute>? PhysicalAttribute { get; set; }

    /// <summary>
    /// Measurement dimensions.
    /// </summary>
    [JsonPropertyName("measurementDimension")]
    [XmlElement("MeasurementDimension", Namespace = UblNamespaces.Cac)]
    public List<Dimension>? MeasurementDimension { get; set; }

    /// <summary>
    /// Issuer party.
    /// </summary>
    [JsonPropertyName("issuerParty")]
    [XmlElement("IssuerParty", Namespace = UblNamespaces.Cac)]
    public Party? IssuerParty { get; set; }
}
