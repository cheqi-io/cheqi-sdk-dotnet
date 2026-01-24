using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Result Of Verification - verification result information.
/// </summary>
[XmlType(Namespace = UblNamespaces.Cac)]
public sealed class ResultOfVerification
{
    /// <summary>
    /// Validator identifier.
    /// </summary>
    [JsonPropertyName("validatorId")]
    [XmlElement("ValidatorID", Namespace = UblNamespaces.Cbc)]
    public Identifier? ValidatorId { get; set; }

    /// <summary>
    /// Validation result code.
    /// </summary>
    [JsonPropertyName("validationResultCode")]
    [XmlElement("ValidationResultCode", Namespace = UblNamespaces.Cbc)]
    public string? ValidationResultCode { get; set; }

    /// <summary>
    /// Validation date.
    /// </summary>
    [JsonPropertyName("validationDate")]
    [XmlElement("ValidationDate", Namespace = UblNamespaces.Cbc)]
    public string? ValidationDate { get; set; }

    /// <summary>
    /// Validation time.
    /// </summary>
    [JsonPropertyName("validationTime")]
    [XmlElement("ValidationTime", Namespace = UblNamespaces.Cbc)]
    public string? ValidationTime { get; set; }

    /// <summary>
    /// Validate process.
    /// </summary>
    [JsonPropertyName("validateProcess")]
    [XmlElement("ValidateProcess", Namespace = UblNamespaces.Cbc)]
    public string? ValidateProcess { get; set; }

    /// <summary>
    /// Validate tool.
    /// </summary>
    [JsonPropertyName("validateTool")]
    [XmlElement("ValidateTool", Namespace = UblNamespaces.Cbc)]
    public string? ValidateTool { get; set; }

    /// <summary>
    /// Validate tool version.
    /// </summary>
    [JsonPropertyName("validateToolVersion")]
    [XmlElement("ValidateToolVersion", Namespace = UblNamespaces.Cbc)]
    public string? ValidateToolVersion { get; set; }

    /// <summary>
    /// Signatory party.
    /// </summary>
    [JsonPropertyName("signatoryParty")]
    [XmlElement("SignatoryParty", Namespace = UblNamespaces.Cac)]
    public Party? SignatoryParty { get; set; }
}
