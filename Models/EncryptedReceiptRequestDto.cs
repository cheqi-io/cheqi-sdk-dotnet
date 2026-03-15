using System.Text.Json.Serialization;
using Cheqi.Sdk.Models.Enums;

namespace Cheqi.Sdk.Models;

/// <summary>
/// Request DTO for submitting an encrypted receipt.
/// </summary>
public sealed class EncryptedReceiptRequestDto
{
    /// <summary>
    /// The ID of the target recipient for this encrypted receipt.
    /// </summary>
    [JsonPropertyName("recipientId")]
    public string recipientId { get; }

    /// <summary>
    /// The receipt ID (optional - set by backend after processing).
    /// </summary>
    [JsonPropertyName("receiptId")]
    public string? receiptId { get; }

    /// <summary>
    /// The receiver type (DEVICE or WEBHOOK).
    /// </summary>
    [JsonPropertyName("receiverType")]
    public ReceiverType receiverType { get; }

    /// <summary>
    /// Base64-encoded encrypted receipt data.
    /// </summary>
    [JsonPropertyName("encryptedReceipt")]
    public string encryptedReceipt { get; }

    /// <summary>
    /// Base64-encoded encrypted symmetric key.
    /// </summary>
    [JsonPropertyName("encryptedSymmetricKey")]
    public string encryptedSymmetricKey { get; }

    /// <summary>
    /// Final hash of the receipt (optional - only set when receiving party completes the receipt).
    /// </summary>
    [JsonPropertyName("finalHash")]
    public string? FinalHash { get; }

    /// <summary>
    /// Public key used to encrypt the symmetric key.
    /// </summary>
    [JsonPropertyName("publicKey")]
    public string publicKey { get; }

    /// <summary>
    /// Supplier party ID for tracking purposes.
    /// </summary>
    [JsonPropertyName("supplierPartyId")]
    public Guid? supplierPartyId { get; }

    // ===== CONSTRUCTOR =====

    private EncryptedReceiptRequestDto(
        string recipientId,
        string? receiptId,
        ReceiverType receiverType,
        string encryptedReceipt,
        string encryptedSymmetricKey,
        string? finalHash,
        string publicKey,
        Guid? supplierPartyId)
    {
        this.recipientId = recipientId;
        this.receiptId = receiptId;
        this.receiverType = receiverType;
        this.encryptedReceipt = encryptedReceipt;
        this.encryptedSymmetricKey = encryptedSymmetricKey;
        FinalHash = finalHash;
        this.publicKey = publicKey;
        this.supplierPartyId = supplierPartyId;
    }

    // ===== BUILDER =====

    public static EncryptedReceiptRequestDtoBuilder Builder() => new EncryptedReceiptRequestDtoBuilder();

    public sealed class EncryptedReceiptRequestDtoBuilder
    {
        private string? _recipientId;
        private string? _receiptId;
        private ReceiverType? _receiverType;
        private string? _encryptedReceipt;
        private string? _encryptedSymmetricKey;
        private string? _finalHash;
        private string? _publicKey;
        private Guid? _supplierPartyId;

        internal EncryptedReceiptRequestDtoBuilder() { }

        public EncryptedReceiptRequestDtoBuilder From(EncryptedReceiptRequestDto other)
        {
            _recipientId = other.recipientId;
            _receiptId = other.receiptId;
            _receiverType = other.receiverType;
            _encryptedReceipt = other.encryptedReceipt;
            _encryptedSymmetricKey = other.encryptedSymmetricKey;
            _finalHash = other.FinalHash;
            _publicKey = other.publicKey;
            _supplierPartyId = other.supplierPartyId;
            return this;
        }

        public EncryptedReceiptRequestDtoBuilder RecipientId(string recipientId)
        {
            _recipientId = recipientId;
            return this;
        }

        public EncryptedReceiptRequestDtoBuilder ReceiptId(string receiptId)
        {
            _receiptId = receiptId;
            return this;
        }

        public EncryptedReceiptRequestDtoBuilder ReceiverType(ReceiverType receiverType)
        {
            _receiverType = receiverType;
            return this;
        }

        public EncryptedReceiptRequestDtoBuilder EncryptedReceipt(string encryptedReceipt)
        {
            _encryptedReceipt = encryptedReceipt;
            return this;
        }

        public EncryptedReceiptRequestDtoBuilder EncryptedSymmetricKey(string encryptedSymmetricKey)
        {
            _encryptedSymmetricKey = encryptedSymmetricKey;
            return this;
        }

        public EncryptedReceiptRequestDtoBuilder FinalHash(string finalHash)
        {
            _finalHash = finalHash;
            return this;
        }

        public EncryptedReceiptRequestDtoBuilder PublicKey(string publicKey)
        {
            _publicKey = publicKey;
            return this;
        }

        public EncryptedReceiptRequestDtoBuilder SupplierPartyId(Guid? supplierPartyId)
        {
            _supplierPartyId = supplierPartyId;
            return this;
        }

        public EncryptedReceiptRequestDto Build()
        {
            if (_recipientId == null)
                throw new InvalidOperationException("RecipientId is required");
            if (_receiverType == null)
                throw new InvalidOperationException("ReceiverType is required");
            if (_encryptedReceipt == null)
                throw new InvalidOperationException("EncryptedReceipt is required");
            if (_encryptedSymmetricKey == null)
                throw new InvalidOperationException("EncryptedSymmetricKey is required");
            if (_publicKey == null)
                throw new InvalidOperationException("PublicKey is required");

            return new EncryptedReceiptRequestDto(
                _recipientId,
                _receiptId,
                _receiverType.Value,
                _encryptedReceipt,
                _encryptedSymmetricKey,
                _finalHash,
                _publicKey,
                _supplierPartyId
            );
        }
    }

    // ===== EQUALS, HASHCODE, TOSTRING =====

    public override bool Equals(object? obj)
    {
        if (obj is not EncryptedReceiptRequestDto other) return false;
        return recipientId == other.recipientId
            && receiptId == other.receiptId
            && receiverType == other.receiverType
            && encryptedReceipt == other.encryptedReceipt
            && encryptedSymmetricKey == other.encryptedSymmetricKey
            && FinalHash == other.FinalHash
            && publicKey == other.publicKey
            && supplierPartyId == other.supplierPartyId;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(
            recipientId,
            receiptId,
            receiverType,
            encryptedReceipt,
            encryptedSymmetricKey,
            FinalHash,
            publicKey,
            supplierPartyId);
    }

    public override string ToString()
    {
        return $"EncryptedReceiptRequestDto{{" +
               $"recipientId={recipientId}, " +
               $"receiptId='{receiptId}', " +
               $"receiverType={receiverType}, " +
               $"encryptedReceipt='{encryptedReceipt}', " +
               $"encryptedSymmetricKey='{encryptedSymmetricKey}', " +
               $"finalHash='{FinalHash}', " +
               $"publicKey='{publicKey}', " +
               $"supplierPartyId={supplierPartyId}}}";
    }
}
