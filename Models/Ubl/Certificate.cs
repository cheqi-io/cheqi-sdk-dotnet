using System.Text.Json.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Certificate - a certificate document.
/// </summary>
public sealed class Certificate
{
    /// <summary>
    /// Certificate identifier.
    /// </summary>
    [JsonPropertyName("id")]
    public Identifier? Id { get; set; }

    /// <summary>
    /// Certificate type code.
    /// </summary>
    [JsonPropertyName("certificateTypeCode")]
    public string? CertificateTypeCode { get; set; }

    /// <summary>
    /// Certificate types.
    /// </summary>
    [JsonPropertyName("certificateType")]
    public List<string>? CertificateType { get; set; }

    /// <summary>
    /// Remarks.
    /// </summary>
    [JsonPropertyName("remarks")]
    public List<string>? Remarks { get; set; }

    /// <summary>
    /// Issuer party.
    /// </summary>
    [JsonPropertyName("issuerParty")]
    public Party? IssuerParty { get; set; }

    /// <summary>
    /// Document references.
    /// </summary>
    [JsonPropertyName("documentReference")]
    public List<DocumentReference>? DocumentReference { get; set; }

    /// <summary>
    /// Signatures.
    /// </summary>
    [JsonPropertyName("signature")]
    public List<Signature>? Signature { get; set; }
}
