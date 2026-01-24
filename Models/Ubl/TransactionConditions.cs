using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Transaction Conditions - conditions for a transaction.
/// </summary>
[XmlType(Namespace = UblNamespaces.Cac)]
public sealed class TransactionConditions
{
    /// <summary>
    /// Condition identifier.
    /// </summary>
    [JsonPropertyName("id")]
    [XmlElement("ID", Namespace = UblNamespaces.Cbc)]
    public Identifier? Id { get; set; }

    /// <summary>
    /// Action code.
    /// </summary>
    [JsonPropertyName("actionCode")]
    [XmlElement("ActionCode", Namespace = UblNamespaces.Cbc)]
    public Code? ActionCode { get; set; }

    /// <summary>
    /// Descriptions.
    /// </summary>
    [JsonPropertyName("description")]
    [XmlElement("Description", Namespace = UblNamespaces.Cbc)]
    public List<string>? Description { get; set; }

    /// <summary>
    /// Document references.
    /// </summary>
    [JsonPropertyName("documentReference")]
    [XmlElement("DocumentReference", Namespace = UblNamespaces.Cac)]
    public List<DocumentReference>? DocumentReference { get; set; }
}
