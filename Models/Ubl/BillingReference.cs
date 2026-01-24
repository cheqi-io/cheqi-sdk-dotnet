using System.Text.Json.Serialization;

namespace Cheqi.Sdk.Models.Ubl;

/// <summary>
/// UBL Billing Reference - a reference to a billing document.
/// </summary>
public sealed class BillingReference
{
    /// <summary>
    /// Reference to an invoice document.
    /// </summary>
    [JsonPropertyName("invoiceDocumentReference")]
    public InvoiceDocumentReference? InvoiceDocumentReference { get; set; }
}
