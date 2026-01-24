using System.Text.Json.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Result Of Verification - verification result information.
/// </summary>
public sealed class ResultOfVerification
{
    /// <summary>
    /// Validator identifier.
    /// </summary>
    [JsonPropertyName("validatorId")]
    public Identifier? ValidatorId { get; set; }

    /// <summary>
    /// Validation result code.
    /// </summary>
    [JsonPropertyName("validationResultCode")]
    public string? ValidationResultCode { get; set; }

    /// <summary>
    /// Validation date.
    /// </summary>
    [JsonPropertyName("validationDate")]
    public string? ValidationDate { get; set; }

    /// <summary>
    /// Validation time.
    /// </summary>
    [JsonPropertyName("validationTime")]
    public string? ValidationTime { get; set; }

    /// <summary>
    /// Validate process.
    /// </summary>
    [JsonPropertyName("validateProcess")]
    public string? ValidateProcess { get; set; }

    /// <summary>
    /// Validate tool.
    /// </summary>
    [JsonPropertyName("validateTool")]
    public string? ValidateTool { get; set; }

    /// <summary>
    /// Validate tool version.
    /// </summary>
    [JsonPropertyName("validateToolVersion")]
    public string? ValidateToolVersion { get; set; }

    /// <summary>
    /// Signatory party.
    /// </summary>
    [JsonPropertyName("signatoryParty")]
    public Party? SignatoryParty { get; set; }
}
