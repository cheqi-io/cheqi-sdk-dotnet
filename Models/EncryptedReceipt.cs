using System.Text.Json.Serialization;
using Cheqi.Sdk.Models.Enums;

namespace Cheqi.Sdk.Models;

/// <summary>
/// Encrypted receipt DTO containing encrypted receipt data and metadata.
/// </summary>
public sealed class EncryptedReceipt
{
    // ===== MANDATORY FIELDS =====

    /// <summary>
    /// The UUID of the target device for this encrypted receipt.
    /// </summary>
    [JsonPropertyName("recipientId")]
    public Guid recipientId { get; }

    /// <summary>
    /// The receipt ID.
    /// </summary>
    [JsonPropertyName("receiptId")]
    public string receiptId { get; }

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
    /// Base64-encoded device-specific encrypted symmetric key.
    /// </summary>
    [JsonPropertyName("encryptedSymmetricKey")]
    public string encryptedSymmetricKey { get; }

    /// <summary>
    /// Base64-encoded encrypted customer details.
    /// </summary>
    [JsonPropertyName("encryptedCustomerDetails")]
    public string encryptedCustomerDetails { get; }

    /// <summary>
    /// Base64-encoded encrypted customer AES key.
    /// </summary>
    [JsonPropertyName("encryptedCustomerAesKey")]
    public string encryptedCustomerAesKey { get; }

    /// <summary>
    /// Final hash of the receipt.
    /// </summary>
    [JsonPropertyName("finalHash")]
    public string finalHash { get; }

    /// <summary>
    /// Public key used to encrypt the symmetric key (for key selection on decrypt).
    /// </summary>
    [JsonPropertyName("publicKey")]
    public string publicKey { get; }

    // ===== OPTIONAL FIELDS =====

    /// <summary>
    /// Supplier party ID for tracking purposes.
    /// </summary>
    [JsonPropertyName("supplierPartyId")]
    public Guid? supplierPartyId { get; }

    // ===== CONSTRUCTOR =====

    private EncryptedReceipt(
        Guid recipientId,
        string receiptId,
        ReceiverType receiverType,
        string encryptedReceipt,
        string encryptedSymmetricKey,
        string encryptedCustomerDetails,
        string encryptedCustomerAesKey,
        string finalHash,
        string publicKey,
        Guid? supplierPartyId)
    {
        this.recipientId = recipientId;
        this.receiptId = receiptId;
        this.receiverType = receiverType;
        this.encryptedReceipt = encryptedReceipt;
        this.encryptedSymmetricKey = encryptedSymmetricKey;
        this.encryptedCustomerDetails = encryptedCustomerDetails;
        this.encryptedCustomerAesKey = encryptedCustomerAesKey;
        this.finalHash = finalHash;
        this.publicKey = publicKey;
        this.supplierPartyId = supplierPartyId;
    }

    // ===== BUILDER =====

    public static EncryptedReceiptBuilder Builder() => new EncryptedReceiptBuilder();

    public sealed class EncryptedReceiptBuilder
    {
        private Guid? _recipientId;
        private string? _receiptId;
        private ReceiverType? _receiverType;
        private string? _encryptedReceipt;
        private string? _encryptedSymmetricKey;
        private string? _encryptedCustomerDetails;
        private string? _encryptedCustomerAesKey;
        private string? _finalHash;
        private string? _publicKey;
        private Guid? _supplierPartyId;

        internal EncryptedReceiptBuilder() { }

        public EncryptedReceiptBuilder From(EncryptedReceipt other)
        {
            _recipientId = other.recipientId;
            _receiptId = other.receiptId;
            _receiverType = other.receiverType;
            _encryptedReceipt = other.encryptedReceipt;
            _encryptedSymmetricKey = other.encryptedSymmetricKey;
            _encryptedCustomerDetails = other.encryptedCustomerDetails;
            _encryptedCustomerAesKey = other.encryptedCustomerAesKey;
            _finalHash = other.finalHash;
            _publicKey = other.publicKey;
            _supplierPartyId = other.supplierPartyId;
            return this;
        }

        public EncryptedReceiptBuilder RecipientId(Guid recipientId)
        {
            _recipientId = recipientId;
            return this;
        }

        public EncryptedReceiptBuilder ReceiptId(string receiptId)
        {
            _receiptId = receiptId;
            return this;
        }

        public EncryptedReceiptBuilder ReceiverType(ReceiverType receiverType)
        {
            _receiverType = receiverType;
            return this;
        }

        public EncryptedReceiptBuilder EncryptedReceipt(string encryptedReceipt)
        {
            _encryptedReceipt = encryptedReceipt;
            return this;
        }

        public EncryptedReceiptBuilder EncryptedSymmetricKey(string encryptedSymmetricKey)
        {
            _encryptedSymmetricKey = encryptedSymmetricKey;
            return this;
        }

        public EncryptedReceiptBuilder EncryptedCustomerDetails(string encryptedCustomerDetails)
        {
            _encryptedCustomerDetails = encryptedCustomerDetails;
            return this;
        }

        public EncryptedReceiptBuilder EncryptedCustomerAesKey(string encryptedCustomerAesKey)
        {
            _encryptedCustomerAesKey = encryptedCustomerAesKey;
            return this;
        }

        public EncryptedReceiptBuilder FinalHash(string finalHash)
        {
            _finalHash = finalHash;
            return this;
        }

        public EncryptedReceiptBuilder PublicKey(string publicKey)
        {
            _publicKey = publicKey;
            return this;
        }

        public EncryptedReceiptBuilder SupplierPartyId(Guid? supplierPartyId)
        {
            _supplierPartyId = supplierPartyId;
            return this;
        }

        public EncryptedReceipt Build()
        {
            if (_recipientId == null)
                throw new InvalidOperationException("RecipientId is required");
            if (_receiptId == null)
                throw new InvalidOperationException("ReceiptId is required");
            if (_receiverType == null)
                throw new InvalidOperationException("ReceiverType is required");
            if (_encryptedReceipt == null)
                throw new InvalidOperationException("EncryptedReceipt is required");
            if (_encryptedSymmetricKey == null)
                throw new InvalidOperationException("EncryptedSymmetricKey is required");
            if (_encryptedCustomerDetails == null)
                throw new InvalidOperationException("EncryptedCustomerDetails is required");
            if (_encryptedCustomerAesKey == null)
                throw new InvalidOperationException("EncryptedCustomerAesKey is required");
            if (_finalHash == null)
                throw new InvalidOperationException("FinalHash is required");
            if (_publicKey == null)
                throw new InvalidOperationException("PublicKey is required");

            return new EncryptedReceipt(
                _recipientId.Value,
                _receiptId,
                _receiverType.Value,
                _encryptedReceipt,
                _encryptedSymmetricKey,
                _encryptedCustomerDetails,
                _encryptedCustomerAesKey,
                _finalHash,
                _publicKey,
                _supplierPartyId
            );
        }
    }

    // ===== VALIDATION METHODS =====

    /// <summary>
    /// Validates this encrypted receipt DTO.
    /// </summary>
    /// <returns>true if all mandatory fields are present and valid</returns>
    public bool IsValid() => GetValidationErrors().Count == 0;

    /// <summary>
    /// Gets detailed validation errors for this encrypted receipt DTO.
    /// </summary>
    /// <returns>List of validation error messages, empty if valid</returns>
    public List<string> GetValidationErrors()
    {
        var errors = new List<string>();

        if (recipientId == Guid.Empty)
            errors.Add("Device ID is required");

        if (receiverType == default)
            errors.Add("Receiver type is required");

        if (string.IsNullOrWhiteSpace(encryptedReceipt))
            errors.Add("Encrypted receipt data is required");

        if (string.IsNullOrWhiteSpace(encryptedSymmetricKey))
            errors.Add("Encrypted symmetric key is required");

        return errors;
    }

    // ===== EQUALS, HASHCODE, TOSTRING =====

    public override bool Equals(object? obj)
    {
        if (obj is not EncryptedReceipt other) return false;
        return recipientId == other.recipientId
            && receiptId == other.receiptId
            && receiverType == other.receiverType
            && encryptedReceipt == other.encryptedReceipt
            && encryptedCustomerDetails == other.encryptedCustomerDetails
            && encryptedSymmetricKey == other.encryptedSymmetricKey
            && encryptedCustomerAesKey == other.encryptedCustomerAesKey
            && finalHash == other.finalHash
            && supplierPartyId == other.supplierPartyId;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(
            recipientId,
            encryptedReceipt,
            encryptedCustomerDetails,
            encryptedSymmetricKey,
            encryptedCustomerAesKey,
            supplierPartyId);
    }

    public override string ToString()
    {
        return $"EncryptedReceipt{{" +
               $"recipientId={recipientId}, " +
               $"receiptId={receiptId}, " +
               $"receiverType={receiverType}, " +
               $"encryptedReceipt='{(encryptedReceipt != null ? "[ENCRYPTED]" : null)}', " +
               $"encryptedCustomerDetails='{(encryptedCustomerDetails != null ? "[ENCRYPTED]" : null)}', " +
               $"encryptedSymmetricKey='{(encryptedSymmetricKey != null ? "[ENCRYPTED]" : null)}', " +
               $"encryptedCustomerAesKey='{(encryptedCustomerAesKey != null ? "[ENCRYPTED]" : null)}', " +
               $"finalHash='{(finalHash != null ? "[ENCRYPTED]" : null)}', " +
               $"supplierPartyId={supplierPartyId}}}";
    }
}
