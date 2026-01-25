using System.Text.Json.Serialization;

namespace Cheqi.Sdk.CreditNote;

public class EncryptedCreditNoteInitiationRequest
{
    [JsonPropertyName("cheqiReceiptId")]
    public string? CheqiReceiptId { get; set; }
    
    [JsonPropertyName("publicKey")]
    public string? PublicKey { get; set; }
    
    [JsonPropertyName("encryptedCreditNoteInitiationRequest")]
    public string? EncryptedCreditNoteInitiationRequestData { get; set; }
    
    [JsonPropertyName("encryptedSymmetricKey")]
    public string? EncryptedSymmetricKey { get; set; }

    public EncryptedCreditNoteInitiationRequest()
    {
    }

    private EncryptedCreditNoteInitiationRequest(Builder builder)
    {
        CheqiReceiptId = builder._cheqiReceiptId;
        PublicKey = builder._publicKey;
        EncryptedCreditNoteInitiationRequestData = builder._encryptedCreditNoteInitiationRequest;
        EncryptedSymmetricKey = builder._encryptedSymmetricKey;
    }

    public static Builder BuilderInstance() => new Builder();

    public class Builder
    {
        internal string? _cheqiReceiptId;
        internal string? _publicKey;
        internal string? _encryptedCreditNoteInitiationRequest;
        internal string? _encryptedSymmetricKey;

        public Builder CheqiReceiptId(string cheqiReceiptId)
        {
            _cheqiReceiptId = cheqiReceiptId;
            return this;
        }

        public Builder PublicKey(string publicKey)
        {
            _publicKey = publicKey;
            return this;
        }

        public Builder EncryptedCreditNoteInitiationRequest(string encryptedCreditNoteInitiationRequest)
        {
            _encryptedCreditNoteInitiationRequest = encryptedCreditNoteInitiationRequest;
            return this;
        }

        public Builder EncryptedSymmetricKey(string encryptedSymmetricKey)
        {
            _encryptedSymmetricKey = encryptedSymmetricKey;
            return this;
        }

        public EncryptedCreditNoteInitiationRequest Build()
        {
            return new EncryptedCreditNoteInitiationRequest(this);
        }
    }
}
