using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Trade Financing - trade financing information.
/// </summary>
[XmlType(Namespace = UblNamespaces.Cac)]
public sealed class TradeFinancing
{
    /// <summary>
    /// Trade financing identifier.
    /// </summary>
    [JsonPropertyName("id")]
    [XmlElement("ID", Namespace = UblNamespaces.Cbc)]
    public Identifier? Id { get; set; }

    /// <summary>
    /// Financing instrument code.
    /// </summary>
    [JsonPropertyName("financingInstrumentCode")]
    [XmlElement("FinancingInstrumentCode", Namespace = UblNamespaces.Cbc)]
    public Code? FinancingInstrumentCode { get; set; }

    /// <summary>
    /// Contract document reference.
    /// </summary>
    [JsonPropertyName("contractDocumentReference")]
    [XmlElement("ContractDocumentReference", Namespace = UblNamespaces.Cac)]
    public DocumentReference? ContractDocumentReference { get; set; }

    /// <summary>
    /// Document references.
    /// </summary>
    [JsonPropertyName("documentReference")]
    [XmlElement("DocumentReference", Namespace = UblNamespaces.Cac)]
    public List<DocumentReference>? DocumentReference { get; set; }

    /// <summary>
    /// Financing party.
    /// </summary>
    [JsonPropertyName("financingParty")]
    [XmlElement("FinancingParty", Namespace = UblNamespaces.Cac)]
    public Party? FinancingParty { get; set; }

    /// <summary>
    /// Financing financial account.
    /// </summary>
    [JsonPropertyName("financingFinancialAccount")]
    [XmlElement("FinancingFinancialAccount", Namespace = UblNamespaces.Cac)]
    public FinancialAccount? FinancingFinancialAccount { get; set; }

    /// <summary>
    /// Clause.
    /// </summary>
    [JsonPropertyName("clause")]
    [XmlElement("Clause", Namespace = UblNamespaces.Cac)]
    public Clause? Clause { get; set; }
}
