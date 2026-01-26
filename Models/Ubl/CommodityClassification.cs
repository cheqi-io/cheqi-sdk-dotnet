using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Commodity Classification - represents classification information for a commodity.
/// </summary>
[XmlType(Namespace = UblNamespaces.Cac)]
public sealed class CommodityClassification
{
    /// <summary>
    /// Nature code.
    /// </summary>
    [JsonPropertyName("natureCode")]
    [XmlElement("NatureCode", Namespace = UblNamespaces.Cbc)]
    public string? NatureCode { get; set; }

    /// <summary>
    /// Cargo type code.
    /// </summary>
    [JsonPropertyName("cargoTypeCode")]
    [XmlElement("CargoTypeCode", Namespace = UblNamespaces.Cbc)]
    public string? CargoTypeCode { get; set; }

    /// <summary>
    /// Commodity code.
    /// </summary>
    [JsonPropertyName("commodityCode")]
    [XmlElement("CommodityCode", Namespace = UblNamespaces.Cbc)]
    public string? CommodityCode { get; set; }

    /// <summary>
    /// Item classification code.
    /// </summary>
    [JsonPropertyName("itemClassificationCode")]
    [XmlElement("ItemClassificationCode", Namespace = UblNamespaces.Cbc)]
    public Code? ItemClassificationCode { get; set; }
}