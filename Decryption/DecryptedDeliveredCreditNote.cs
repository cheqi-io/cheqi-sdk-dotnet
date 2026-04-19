using Cheqi.Sdk.CreditNote;
using Cheqi.Sdk.Models.Generated;

namespace Cheqi.Sdk.Decryption;

public class DecryptedDeliveredCreditNote
{
    public CreditNoteEnvelope CreditNoteEnvelope { get; }
    public CreditNoteContextEnvelope? CreditNoteContextEnvelope { get; }
    public string CreditNoteContentJson { get; }
    public string? CreditNoteContextJson { get; }

    public DecryptedDeliveredCreditNote(
        CreditNoteEnvelope creditNoteEnvelope,
        CreditNoteContextEnvelope? creditNoteContextEnvelope,
        string creditNoteContentJson,
        string? creditNoteContextJson)
    {
        CreditNoteEnvelope = creditNoteEnvelope;
        CreditNoteContextEnvelope = creditNoteContextEnvelope;
        CreditNoteContentJson = creditNoteContentJson;
        CreditNoteContextJson = creditNoteContextJson;
    }
}
