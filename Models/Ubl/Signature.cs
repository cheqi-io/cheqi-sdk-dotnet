using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Signature - digital signature information.
/// </summary>
[XmlType(Namespace = UblNamespaces.Cac)]
public sealed class Signature
{
    /// <summary>
    /// Signature identifier.
    /// </summary>
    [JsonPropertyName("id")]
    [XmlElement("ID", Namespace = UblNamespaces.Cbc)]
    public Identifier? Id { get; set; }

    /// <summary>
    /// Reason code for the signature.
    /// </summary>
    [JsonPropertyName("reasonCode")]
    [XmlElement("ReasonCode", Namespace = UblNamespaces.Cbc)]
    public Code? ReasonCode { get; set; }

    /// <summary>
    /// Notes about the signature.
    /// </summary>
    [JsonPropertyName("note")]
    [XmlElement("Note", Namespace = UblNamespaces.Cbc)]
    public List<string>? Note { get; set; }

    /// <summary>
    /// Validation date.
    /// </summary>
    [JsonPropertyName("validationDate")]
    [XmlElement("ValidationDate", Namespace = UblNamespaces.Cbc)]
    public DateTimeOffset? ValidationDate { get; set; }

    /// <summary>
    /// Validation time.
    /// </summary>
    [JsonPropertyName("validationTime")]
    [XmlElement("ValidationTime", Namespace = UblNamespaces.Cbc)]
    public TimeSpan? ValidationTime { get; set; }

    /// <summary>
    /// Validator identifier.
    /// </summary>
    [JsonPropertyName("validatorId")]
    [XmlElement("ValidatorID", Namespace = UblNamespaces.Cbc)]
    public Identifier? ValidatorId { get; set; }

    /// <summary>
    /// Canonicalization method.
    /// </summary>
    [JsonPropertyName("canonicalizationMethod")]
    [XmlElement("CanonicalizationMethod", Namespace = UblNamespaces.Cbc)]
    public string? CanonicalizationMethod { get; set; }

    /// <summary>
    /// Signature method.
    /// </summary>
    [JsonPropertyName("signatureMethod")]
    [XmlElement("SignatureMethod", Namespace = UblNamespaces.Cbc)]
    public string? SignatureMethod { get; set; }

    /// <summary>
    /// Signatory party.
    /// </summary>
    [JsonPropertyName("signatoryParty")]
    [XmlElement("SignatoryParty", Namespace = UblNamespaces.Cac)]
    public Party? SignatoryParty { get; set; }

    /// <summary>
    /// Digital signature attachment.
    /// </summary>
    [JsonPropertyName("digitalSignatureAttachment")]
    [XmlElement("DigitalSignatureAttachment", Namespace = UblNamespaces.Cac)]
    public Attachment? DigitalSignatureAttachment { get; set; }

    /// <summary>
    /// Original document reference.
    /// </summary>
    [JsonPropertyName("documentReference")]
    [XmlElement("OriginalDocumentReference", Namespace = UblNamespaces.Cac)]
    public DocumentReference? DocumentReference { get; set; }
}
