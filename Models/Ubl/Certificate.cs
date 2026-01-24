using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Certificate - a certificate document.
/// </summary>
[XmlType(Namespace = UblNamespaces.Cac)]
public sealed class Certificate
{
    /// <summary>
    /// Certificate identifier.
    /// </summary>
    [JsonPropertyName("id")]
    [XmlElement("ID", Namespace = UblNamespaces.Cbc)]
    public Identifier? Id { get; set; }

    /// <summary>
    /// Certificate type code.
    /// </summary>
    [JsonPropertyName("certificateTypeCode")]
    [XmlElement("CertificateTypeCode", Namespace = UblNamespaces.Cbc)]
    public string? CertificateTypeCode { get; set; }

    /// <summary>
    /// Certificate types.
    /// </summary>
    [JsonPropertyName("certificateType")]
    [XmlElement("CertificateType", Namespace = UblNamespaces.Cbc)]
    public List<string>? CertificateType { get; set; }

    /// <summary>
    /// Remarks.
    /// </summary>
    [JsonPropertyName("remarks")]
    [XmlElement("Remarks", Namespace = UblNamespaces.Cbc)]
    public List<string>? Remarks { get; set; }

    /// <summary>
    /// Issuer party.
    /// </summary>
    [JsonPropertyName("issuerParty")]
    [XmlElement("IssuerParty", Namespace = UblNamespaces.Cac)]
    public Party? IssuerParty { get; set; }

    /// <summary>
    /// Document references.
    /// </summary>
    [JsonPropertyName("documentReference")]
    [XmlElement("DocumentReference", Namespace = UblNamespaces.Cac)]
    public List<DocumentReference>? DocumentReference { get; set; }

    /// <summary>
    /// Signatures.
    /// </summary>
    [JsonPropertyName("signature")]
    [XmlElement("Signature", Namespace = UblNamespaces.Cac)]
    public List<Signature>? Signature { get; set; }
}
