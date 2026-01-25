using System.Text.Json.Serialization;

namespace Cheqi.Sdk.CreditNote;

public class EncryptedCreditNotesRequest
{
    [JsonPropertyName("matchId")]
    public string? MatchId { get; set; }
    
    [JsonPropertyName("parentCheqiReceiptId")]
    public string? ParentCheqiReceiptId { get; set; }
    
    [JsonPropertyName("encryptedCreditNotes")]
    public HashSet<EncryptedCreditNote>? EncryptedCreditNotes { get; set; }
    
    [JsonPropertyName("templateHash")]
    public string? TemplateHash { get; set; }

    public EncryptedCreditNotesRequest()
    {
    }

    public EncryptedCreditNotesRequest(string matchId, string parentCheqiReceiptId, 
        HashSet<EncryptedCreditNote> encryptedCreditNotes, string templateHash)
    {
        MatchId = matchId;
        ParentCheqiReceiptId = parentCheqiReceiptId;
        EncryptedCreditNotes = encryptedCreditNotes;
        TemplateHash = templateHash;
    }
}
