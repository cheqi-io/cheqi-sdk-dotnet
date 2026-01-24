using System.Text.Json.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Signature - digital signature information.
/// </summary>
public sealed class Signature
{
    /// <summary>
    /// Signature identifier.
    /// </summary>
    [JsonPropertyName("id")]
    public Identifier? Id { get; set; }

    /// <summary>
    /// Reason code for the signature.
    /// </summary>
    [JsonPropertyName("reasonCode")]
    public Code? ReasonCode { get; set; }

    /// <summary>
    /// Notes about the signature.
    /// </summary>
    [JsonPropertyName("note")]
    public List<string>? Note { get; set; }

    /// <summary>
    /// Validation date.
    /// </summary>
    [JsonPropertyName("validationDate")]
    public DateTimeOffset? ValidationDate { get; set; }

    /// <summary>
    /// Validation time.
    /// </summary>
    [JsonPropertyName("validationTime")]
    public TimeSpan? ValidationTime { get; set; }

    /// <summary>
    /// Validator identifier.
    /// </summary>
    [JsonPropertyName("validatorId")]
    public Identifier? ValidatorId { get; set; }

    /// <summary>
    /// Canonicalization method.
    /// </summary>
    [JsonPropertyName("canonicalizationMethod")]
    public string? CanonicalizationMethod { get; set; }

    /// <summary>
    /// Signature method.
    /// </summary>
    [JsonPropertyName("signatureMethod")]
    public string? SignatureMethod { get; set; }

    /// <summary>
    /// Signatory party.
    /// </summary>
    [JsonPropertyName("signatoryParty")]
    public Party? SignatoryParty { get; set; }

    /// <summary>
    /// Digital signature attachment.
    /// </summary>
    [JsonPropertyName("digitalSignatureAttachment")]
    public Attachment? DigitalSignatureAttachment { get; set; }

    /// <summary>
    /// Original document reference.
    /// </summary>
    [JsonPropertyName("documentReference")]
    public DocumentReference? DocumentReference { get; set; }
}
