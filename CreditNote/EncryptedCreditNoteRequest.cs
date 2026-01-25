using System.Text.Json.Serialization;

namespace Cheqi.Sdk.CreditNote;

public class EncryptedCreditNoteRequest
{
    [JsonPropertyName("cheqiReceiptId")]
    public string? CheqiReceiptId { get; set; }
    
    [JsonPropertyName("parentCheqiReceiptId")]
    public string? ParentCheqiReceiptId { get; set; }
    
    [JsonPropertyName("publicKey")]
    public string? PublicKey { get; set; }
    
    [JsonPropertyName("encryptedCreditRequest")]
    public string? EncryptedCreditRequest { get; set; }
    
    [JsonPropertyName("encryptedSymmetricKey")]
    public string? EncryptedSymmetricKey { get; set; }

    public EncryptedCreditNoteRequest()
    {
    }

    public EncryptedCreditNoteRequest(string cheqiReceiptId, string parentCheqiReceiptId, 
        string publicKey, string encryptedCreditRequest, string encryptedSymmetricKey)
    {
        CheqiReceiptId = cheqiReceiptId;
        ParentCheqiReceiptId = parentCheqiReceiptId;
        PublicKey = publicKey;
        EncryptedCreditRequest = encryptedCreditRequest;
        EncryptedSymmetricKey = encryptedSymmetricKey;
    }
}
