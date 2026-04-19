using System.Text.Json.Serialization;
using Cheqi.Sdk.Models.Generated;

namespace Cheqi.Sdk.CreditNote;

public class CreditNoteEnvelope
{
    [JsonPropertyName("cheqiCreditNote")]
    public CheqiCreditNote? CheqiCreditNote { get; set; }

    [JsonPropertyName("ublCreditNote")]
    public string? UblCreditNote { get; set; }
}
