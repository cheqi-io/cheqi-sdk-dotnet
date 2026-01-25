namespace Cheqi.Sdk.Decryption;

public class DecryptedCreditNote
{
    public string CreditNoteContentJson { get; }

    public DecryptedCreditNote(string creditNoteContentJson)
    {
        CreditNoteContentJson = creditNoteContentJson;
    }
}
