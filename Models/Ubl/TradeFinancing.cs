using System.Text.Json.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Trade Financing - trade financing information.
/// </summary>
public sealed class TradeFinancing
{
    /// <summary>
    /// Trade financing identifier.
    /// </summary>
    [JsonPropertyName("id")]
    public Identifier? Id { get; set; }

    /// <summary>
    /// Financing instrument code.
    /// </summary>
    [JsonPropertyName("financingInstrumentCode")]
    public Code? FinancingInstrumentCode { get; set; }

    /// <summary>
    /// Contract document reference.
    /// </summary>
    [JsonPropertyName("contractDocumentReference")]
    public DocumentReference? ContractDocumentReference { get; set; }

    /// <summary>
    /// Document references.
    /// </summary>
    [JsonPropertyName("documentReference")]
    public List<DocumentReference>? DocumentReference { get; set; }

    /// <summary>
    /// Financing party.
    /// </summary>
    [JsonPropertyName("financingParty")]
    public Party? FinancingParty { get; set; }

    /// <summary>
    /// Financing financial account.
    /// </summary>
    [JsonPropertyName("financingFinancialAccount")]
    public FinancialAccount? FinancingFinancialAccount { get; set; }

    /// <summary>
    /// Clause.
    /// </summary>
    [JsonPropertyName("clause")]
    public Clause? Clause { get; set; }
}
