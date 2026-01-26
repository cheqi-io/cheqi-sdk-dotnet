using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Item Property - represents a property or characteristic of an item.
/// </summary>
[XmlType(Namespace = UblNamespaces.Cac)]
public sealed class ItemProperty
{
    /// <summary>
    /// Property ID.
    /// </summary>
    [JsonPropertyName("id")]
    [XmlElement("ID", Namespace = UblNamespaces.Cbc)]
    public Identifier? Id { get; set; }

    /// <summary>
    /// Property name (required).
    /// </summary>
    [JsonPropertyName("name")]
    [XmlElement("Name", Namespace = UblNamespaces.Cbc)]
    public Name? Name { get; set; }

    /// <summary>
    /// Name code.
    /// </summary>
    [JsonPropertyName("nameCode")]
    [XmlElement("NameCode", Namespace = UblNamespaces.Cbc)]
    public Code? NameCode { get; set; }

    /// <summary>
    /// Test method.
    /// </summary>
    [JsonPropertyName("testMethod")]
    [XmlElement("TestMethod", Namespace = UblNamespaces.Cbc)]
    public string? TestMethod { get; set; }

    /// <summary>
    /// Property value.
    /// </summary>
    [JsonPropertyName("value")]
    [XmlElement("Value", Namespace = UblNamespaces.Cbc)]
    public string? Value { get; set; }

    /// <summary>
    /// Value quantity.
    /// </summary>
    [JsonPropertyName("valueQuantity")]
    [XmlElement("ValueQuantity", Namespace = UblNamespaces.Cbc)]
    public Quantity? ValueQuantity { get; set; }

    /// <summary>
    /// Value qualifiers.
    /// </summary>
    [JsonPropertyName("valueQualifier")]
    [XmlElement("ValueQualifier", Namespace = UblNamespaces.Cbc)]
    public List<string>? ValueQualifier { get; set; }

    /// <summary>
    /// Importance code.
    /// </summary>
    [JsonPropertyName("importanceCode")]
    [XmlElement("ImportanceCode", Namespace = UblNamespaces.Cbc)]
    public Code? ImportanceCode { get; set; }

    /// <summary>
    /// List values.
    /// </summary>
    [JsonPropertyName("listValue")]
    [XmlElement("ListValue", Namespace = UblNamespaces.Cbc)]
    public List<string>? ListValue { get; set; }

    /// <summary>
    /// Usability period.
    /// </summary>
    [JsonPropertyName("usabilityPeriod")]
    [XmlElement("UsabilityPeriod", Namespace = UblNamespaces.Cac)]
    public Period? UsabilityPeriod { get; set; }

    /// <summary>
    /// Item property groups.
    /// </summary>
    [JsonPropertyName("itemPropertyGroup")]
    [XmlElement("ItemPropertyGroup", Namespace = UblNamespaces.Cac)]
    public List<ItemPropertyGroup>? ItemPropertyGroup { get; set; }

    /// <summary>
    /// Range dimension.
    /// </summary>
    [JsonPropertyName("rangeDimension")]
    [XmlElement("RangeDimension", Namespace = UblNamespaces.Cac)]
    public Dimension? RangeDimension { get; set; }

    /// <summary>
    /// Item property range.
    /// </summary>
    [JsonPropertyName("itemPropertyRange")]
    [XmlElement("ItemPropertyRange", Namespace = UblNamespaces.Cac)]
    public ItemPropertyRange? ItemPropertyRange { get; set; }

    /// <summary>
    /// Standard property identification.
    /// </summary>
    [JsonPropertyName("standardPropertyIdentification")]
    [XmlElement("StandardPropertyIdentification", Namespace = UblNamespaces.Cac)]
    public PropertyIdentification? StandardPropertyIdentification { get; set; }
}