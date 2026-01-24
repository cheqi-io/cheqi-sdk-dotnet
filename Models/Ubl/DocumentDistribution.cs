using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Document Distribution - document distribution information.
/// </summary>
[XmlType(Namespace = UblNamespaces.Cac)]
public sealed class DocumentDistribution
{
    /// <summary>
    /// Distribution identifier.
    /// </summary>
    [JsonPropertyName("id")]
    [XmlElement("ID", Namespace = UblNamespaces.Cbc)]
    public Identifier? Id { get; set; }

    /// <summary>
    /// Document type code.
    /// </summary>
    [JsonPropertyName("documentTypeCode")]
    [XmlElement("DocumentTypeCode", Namespace = UblNamespaces.Cbc)]
    public Code? DocumentTypeCode { get; set; }

    /// <summary>
    /// Distribution type code.
    /// </summary>
    [JsonPropertyName("distributionTypeCode")]
    [XmlElement("DistributionTypeCode", Namespace = UblNamespaces.Cbc)]
    public Code? DistributionTypeCode { get; set; }

    /// <summary>
    /// Distribution type descriptions.
    /// </summary>
    [JsonPropertyName("distributionType")]
    [XmlElement("DistributionType", Namespace = UblNamespaces.Cbc)]
    public List<string>? DistributionType { get; set; }

    /// <summary>
    /// Print qualifier.
    /// </summary>
    [JsonPropertyName("printQualifier")]
    [XmlElement("PrintQualifier", Namespace = UblNamespaces.Cbc)]
    public string? PrintQualifier { get; set; }

    /// <summary>
    /// Copy indicator.
    /// </summary>
    [JsonPropertyName("copyIndicator")]
    [XmlElement("CopyIndicator", Namespace = UblNamespaces.Cbc)]
    public bool? CopyIndicator { get; set; }

    /// <summary>
    /// Maximum number of copies.
    /// </summary>
    [JsonPropertyName("maximumCopiesNumeric")]
    [XmlElement("MaximumCopiesNumeric", Namespace = UblNamespaces.Cbc)]
    public decimal? MaximumCopiesNumeric { get; set; }

    /// <summary>
    /// Maximum number of originals.
    /// </summary>
    [JsonPropertyName("maximumOriginalsNumeric")]
    [XmlElement("MaximumOriginalsNumeric", Namespace = UblNamespaces.Cbc)]
    public decimal? MaximumOriginalsNumeric { get; set; }

    /// <summary>
    /// Communication channel.
    /// </summary>
    [JsonPropertyName("communication")]
    [XmlElement("Communication", Namespace = UblNamespaces.Cac)]
    public Communication? Communication { get; set; }

    /// <summary>
    /// Party.
    /// </summary>
    [JsonPropertyName("party")]
    [XmlElement("Party", Namespace = UblNamespaces.Cac)]
    public Party? Party { get; set; }
}
