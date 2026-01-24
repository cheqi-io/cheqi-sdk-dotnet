using System.Text.Json.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Document Distribution - document distribution information.
/// </summary>
public sealed class DocumentDistribution
{
    /// <summary>
    /// Distribution identifier.
    /// </summary>
    [JsonPropertyName("id")]
    public Identifier? Id { get; set; }

    /// <summary>
    /// Document type code.
    /// </summary>
    [JsonPropertyName("documentTypeCode")]
    public Code? DocumentTypeCode { get; set; }

    /// <summary>
    /// Distribution type code.
    /// </summary>
    [JsonPropertyName("distributionTypeCode")]
    public Code? DistributionTypeCode { get; set; }

    /// <summary>
    /// Distribution type descriptions.
    /// </summary>
    [JsonPropertyName("distributionType")]
    public List<string>? DistributionType { get; set; }

    /// <summary>
    /// Print qualifier.
    /// </summary>
    [JsonPropertyName("printQualifier")]
    public string? PrintQualifier { get; set; }

    /// <summary>
    /// Copy indicator.
    /// </summary>
    [JsonPropertyName("copyIndicator")]
    public bool? CopyIndicator { get; set; }

    /// <summary>
    /// Maximum number of copies.
    /// </summary>
    [JsonPropertyName("maximumCopiesNumeric")]
    public decimal? MaximumCopiesNumeric { get; set; }

    /// <summary>
    /// Maximum number of originals.
    /// </summary>
    [JsonPropertyName("maximumOriginalsNumeric")]
    public decimal? MaximumOriginalsNumeric { get; set; }

    /// <summary>
    /// Communication channel.
    /// </summary>
    [JsonPropertyName("communication")]
    public Communication? Communication { get; set; }

    /// <summary>
    /// Party.
    /// </summary>
    [JsonPropertyName("party")]
    public Party? Party { get; set; }
}
