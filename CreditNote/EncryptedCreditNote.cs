using System.Text.Json.Serialization;
using Cheqi.Sdk.Models.Enums;

namespace Cheqi.Sdk.CreditNote;

public class EncryptedCreditNote
{
    [JsonPropertyName("recipientId")]
    public string recipientId { get; set; }

    [JsonPropertyName("creditNoteId")]
    public string? creditNoteId { get; set; }

    [JsonPropertyName("originalReceiptId")]
    public string? originalReceiptId { get; set; }

    [JsonPropertyName("receiverType")]
    public ReceiverType receiverType { get; set; }

    [JsonPropertyName("encryptedCreditNote")]
    public string? encryptedCreditNote { get; set; }

    /// <summary>
    /// Base64-encoded device-specific encrypted symmetric key.
    /// </summary>
    [JsonPropertyName("encryptedSymmetricKey")]
    public string? encryptedSymmetricKey { get; set; }

    /// <summary>
    /// Base64-encoded encrypted customer details.
    /// </summary>
    [JsonPropertyName("encryptedCustomerDetails")]
    public string? encryptedCustomerDetails { get; set; }

    [JsonPropertyName("encryptedCustomerAesKey")]
    public string? encryptedCustomerAesKey { get; set; }

    [JsonPropertyName("finalHash")]
    public string? finalHash { get; set; }

    [JsonPropertyName("publicKey")]
    public string? publicKey { get; set; }

    // ===== OPTIONAL FIELDS =====

    /// <summary>
    /// Supplier party ID for tracking purposes.
    /// </summary>
    [JsonPropertyName("supplierPartyId")]
    public Guid? supplierPartyId { get; set; }

    [JsonPropertyName("additionalProperties")]
    public Dictionary<string, object>? additionalProperties { get; set; }

    public EncryptedCreditNote()
    {
    }

    public EncryptedCreditNote(
        string recipientId,
        string? creditNoteId,
        string? originalReceiptId,
        ReceiverType receiverType,
        string? encryptedCreditNote,
        string? encryptedSymmetricKey,
        string? encryptedCustomerDetails,
        string? encryptedCustomerAesKey,
        string? finalHash,
        string? publicKey,
        Guid? supplierPartyId,
        Dictionary<string, object>? additionalProperties)
    {
        this.recipientId = recipientId;
        this.creditNoteId = creditNoteId;
        this.originalReceiptId = originalReceiptId;
        this.receiverType = receiverType;
        this.encryptedCreditNote = encryptedCreditNote;
        this.encryptedSymmetricKey = encryptedSymmetricKey;
        this.encryptedCustomerDetails = encryptedCustomerDetails;
        this.encryptedCustomerAesKey = encryptedCustomerAesKey;
        this.finalHash = finalHash;
        this.publicKey = publicKey;
        this.supplierPartyId = supplierPartyId;
        this.additionalProperties = additionalProperties;
    }

    public static Builder CreateBuilder() => new Builder();

    public class Builder
    {
        private string? _recipientId;
        private string? _creditNoteId;
        private string? _originalReceiptId;
        private ReceiverType _receiverType;
        private string? _encryptedCreditNote;
        private string? _encryptedSymmetricKey;
        private string? _encryptedCustomerDetails;
        private string? _encryptedCustomerAesKey;
        private string? _finalHash;
        private string? _publicKey;
        private Guid? _supplierPartyId;
        private Dictionary<string, object>? _additionalProperties;

        public Builder RecipientId(string recipientId)
        {
            _recipientId = recipientId;
            return this;
        }

        public Builder CreditNoteId(string creditNoteId)
        {
            _creditNoteId = creditNoteId;
            return this;
        }

        public Builder OriginalReceiptId(string originalReceiptId)
        {
            _originalReceiptId = originalReceiptId;
            return this;
        }

        public Builder ReceiverType(ReceiverType receiverType)
        {
            _receiverType = receiverType;
            return this;
        }

        public Builder EncryptedCreditNote(string encryptedCreditNote)
        {
            _encryptedCreditNote = encryptedCreditNote;
            return this;
        }

        public Builder EncryptedSymmetricKey(string encryptedSymmetricKey)
        {
            _encryptedSymmetricKey = encryptedSymmetricKey;
            return this;
        }

        public Builder EncryptedCustomerDetails(string encryptedCustomerDetails)
        {
            _encryptedCustomerDetails = encryptedCustomerDetails;
            return this;
        }

        public Builder EncryptedCustomerAesKey(string encryptedCustomerAesKey)
        {
            _encryptedCustomerAesKey = encryptedCustomerAesKey;
            return this;
        }

        public Builder FinalHash(string finalHash)
        {
            _finalHash = finalHash;
            return this;
        }

        public Builder PublicKey(string publicKey)
        {
            _publicKey = publicKey;
            return this;
        }

        public Builder SupplierPartyId(Guid? supplierPartyId)
        {
            _supplierPartyId = supplierPartyId;
            return this;
        }

        public Builder AdditionalProperties(Dictionary<string, object> additionalProperties)
        {
            _additionalProperties = additionalProperties;
            return this;
        }

        public EncryptedCreditNote Build()
        {
            return new EncryptedCreditNote(
                _recipientId,
                _creditNoteId,
                _originalReceiptId,
                _receiverType,
                _encryptedCreditNote,
                _encryptedSymmetricKey,
                _encryptedCustomerDetails,
                _encryptedCustomerAesKey,
                _finalHash,
                _publicKey,
                _supplierPartyId,
                _additionalProperties
            );
        }
    }
}
