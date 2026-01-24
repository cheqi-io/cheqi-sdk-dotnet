using System.Text.Json.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Invoice Document Reference - a reference to an invoice document.
/// </summary>
public sealed class InvoiceDocumentReference
{
    /// <summary>
    /// Document identifier.
    /// </summary>
    [JsonPropertyName("id")]
    public Identifier? Id { get; set; }

    /// <summary>
    /// Issue date of the referenced document.
    /// </summary>
    [JsonPropertyName("issueDate")]
    public DateTimeOffset? IssueDate { get; set; }
}
