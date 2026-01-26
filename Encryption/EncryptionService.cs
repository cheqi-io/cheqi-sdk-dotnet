using Cheqi.Sdk.CreditNote;
using Cheqi.Sdk.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Security.Cryptography;

namespace Cheqi.Sdk.Encryption;

/// <summary>
/// Core encryption service providing hybrid encryption for end-to-end security.
///
/// This service orchestrates:
/// - AES-256-GCM encryption for receipt data
/// - RSA-OAEP encryption for AES keys
/// - Secure key generation and management
///
/// Thread-safe and designed for high-throughput receipt encryption.
/// </summary>
public sealed class EncryptionService
{
    private readonly ILogger<EncryptionService> _logger;
    private readonly RandomNumberGenerator _secureRandom;
    private readonly AesEncryptor _aesEncryptor;
    private readonly RsaKeyEncryptor _rsaKeyEncryptor;
    private readonly AesKeyGenerator _keyGenerator;

    public EncryptionService(ILogger<EncryptionService>? logger = null)
    {
        _logger = logger ?? NullLogger<EncryptionService>.Instance;
        _secureRandom = RandomNumberGenerator.Create();
        _aesEncryptor = new AesEncryptor(_secureRandom);
        _rsaKeyEncryptor = new RsaKeyEncryptor();
        _keyGenerator = new AesKeyGenerator(_secureRandom);
        _logger.LogInformation("EncryptionService initialized with AES-256-GCM and RSA-OAEP");
    }

    /// <summary>
    /// Encrypts a receipt template for multiple recipients using hybrid encryption.
    /// Each recipient gets a unique AES key encrypted with their RSA public key.
    /// </summary>
    /// <param name="purchaseReceipt">The JSON receipt template to encrypt</param>
    /// <param name="recipients">List of recipients with their public keys</param>
    /// <returns>Set of encrypted receipt DTOs, one per recipient</returns>
    /// <exception cref="EncryptionException">if encryption fails for any recipient</exception>
    public HashSet<EncryptedReceiptRequestDto> EncryptReceiptForRecipients(string purchaseReceipt, List<Recipient> recipients)
    {
        _logger.LogInformation("[ENCRYPTION] Starting encryption for {RecipientCount} recipients", recipients.Count);
        _logger.LogDebug("[ENCRYPTION] Receipt data length: {Length} characters", purchaseReceipt?.Length ?? 0);
        
        if (recipients == null || recipients.Count == 0)
        {
            _logger.LogError("[ENCRYPTION] Recipients list is null or empty");
            throw new EncryptionException("Recipients list cannot be null or empty");
        }

        try
        {
            _logger.LogDebug("[ENCRYPTION] Processing {Count} recipients", recipients.Count);
            
            var encryptedReceipts = new HashSet<EncryptedReceiptRequestDto>();
            for (int i = 0; i < recipients.Count; i++)
            {
                var recipient = recipients[i];
                _logger.LogDebug("[ENCRYPTION] Processing recipient {Index}/{Total}: ID={RecipientId}", 
                    i + 1, recipients.Count, recipient?.id);
                
                var encrypted = EncryptReceiptForRecipient(recipient, purchaseReceipt);
                encryptedReceipts.Add(encrypted);
                
                _logger.LogDebug("[ENCRYPTION] Successfully processed recipient {Index}/{Total}", i + 1, recipients.Count);
            }

            _logger.LogInformation("[ENCRYPTION] Successfully encrypted receipt for {RecipientCount} recipients", encryptedReceipts.Count);
            return encryptedReceipts;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "[ENCRYPTION] Failed to encrypt receipt for recipients: {Message}. InnerException: {Inner}", 
                e.Message, e.InnerException?.Message ?? "none");
            throw new EncryptionException("Failed to encrypt receipt for recipients", e);
        }
    }

    /// <summary>
    /// Encrypts a credit note for multiple recipients using hybrid encryption.
    /// Each recipient gets a unique AES key encrypted with their RSA public key.
    /// </summary>
    /// <param name="creditNote">The JSON credit note to encrypt</param>
    /// <param name="recipients">List of recipients with their public keys</param>
    /// <returns>Set of encrypted credit note DTOs, one per recipient</returns>
    /// <exception cref="EncryptionException">if encryption fails for any recipient</exception>
    public HashSet<EncryptedCreditNote> EncryptCreditNoteForRecipients(string creditNote, List<Recipient> recipients)
    {
        _logger.LogDebug("Encrypting credit note for {RecipientCount} recipients", recipients.Count);

        try
        {
            var encryptedCreditNotes = recipients
                .Select(recipient => EncryptCreditNoteForRecipient(recipient, creditNote))
                .ToHashSet();

            _logger.LogInformation("Successfully encrypted credit note for {RecipientCount} recipients", encryptedCreditNotes.Count);
            return encryptedCreditNotes;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to encrypt credit note for recipients: {Message}", e.Message);
            throw new EncryptionException("Failed to encrypt credit note for recipients", e);
        }
    }

    /// <summary>
    /// Encrypts a receipt for a single recipient using hybrid encryption.
    /// </summary>
    private EncryptedReceiptRequestDto EncryptReceiptForRecipient(Recipient recipient, string purchaseReceipt)
    {
        try
        {
            _logger.LogInformation("[ENCRYPTION] Encrypting receipt for recipient: {RecipientId}", recipient?.id);
            
            if (recipient == null)
            {
                _logger.LogError("[ENCRYPTION] Recipient is null");
                throw new ArgumentNullException(nameof(recipient), "Recipient cannot be null");
            }
            
            _logger.LogDebug("[ENCRYPTION] Recipient details - ID: {Id}, ReceiverType: {Type}, KeyAlgorithm: {Algorithm}, PublicKey length: {KeyLength}",
                recipient.id, recipient.receiverType, recipient.keyAlgorithm, recipient.publicKey?.Length ?? 0);
            
            if (string.IsNullOrEmpty(recipient.publicKey))
            {
                _logger.LogError("[ENCRYPTION] Recipient {RecipientId} has null or empty public key", recipient.id);
                throw new EncryptionException($"Recipient {recipient.id} has no public key");
            }

            // Step 1: Generate unique AES-256 key for this receipt
            _logger.LogDebug("[ENCRYPTION] Step 1: Generating AES-256 key");
            var aesKey = _keyGenerator.GenerateKey();
            _logger.LogDebug("[ENCRYPTION] Generated AES key of length: {Length} bytes", aesKey.Length);

            // Step 2: Encrypt receipt data with AES-GCM
            _logger.LogDebug("[ENCRYPTION] Step 2: Encrypting receipt data with AES-GCM");
            var encryptedData = _aesEncryptor.Encrypt(purchaseReceipt, aesKey);
            var encryptedReceiptBase64 = encryptedData.ToBase64String();
            _logger.LogDebug("[ENCRYPTION] Encrypted receipt Base64 length: {Length}", encryptedReceiptBase64.Length);

            // Step 3: Encrypt AES key with recipient's RSA public key
            _logger.LogDebug("[ENCRYPTION] Step 3: Encrypting AES key with RSA public key");
            _logger.LogDebug("[ENCRYPTION] Public key (full): {PublicKey}", recipient.publicKey);
            
            try
            {
                var encryptedSymmetricKey = _rsaKeyEncryptor.EncryptKey(aesKey, recipient.publicKey);
                _logger.LogDebug("[ENCRYPTION] Encrypted symmetric key length: {Length}", encryptedSymmetricKey.Length);

                // Step 4: Build encrypted receipt DTO
                _logger.LogDebug("[ENCRYPTION] Step 4: Building encrypted receipt DTO");
                var dto = EncryptedReceiptRequestDto.Builder()
                    .RecipientId(recipient.id)
                    .PublicKey(recipient.publicKey)
                    .ReceiverType(recipient.receiverType)
                    .EncryptedReceipt(encryptedReceiptBase64)
                    .EncryptedSymmetricKey(encryptedSymmetricKey)
                    .Build();
                
                _logger.LogInformation("[ENCRYPTION] Successfully encrypted receipt for recipient {RecipientId}", recipient.id);
                return dto;
            }
            catch (Exception rsaException)
            {
                _logger.LogError(rsaException, "[ENCRYPTION] RSA encryption failed. Exception type: {Type}, Message: {Message}, InnerException: {Inner}", 
                    rsaException.GetType().Name, rsaException.Message, rsaException.InnerException?.Message ?? "none");
                throw;
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "[ENCRYPTION] Failed to encrypt receipt for recipient {RecipientId}: {Message}. Type: {ExceptionType}, InnerException: {Inner}", 
                recipient?.id, e.Message, e.GetType().Name, e.InnerException?.Message ?? "none");
            throw new EncryptionException($"Failed to encrypt receipt for recipient: {recipient?.id}", e);
        }
    }

    /// <summary>
    /// Encrypts a credit note for a single recipient using hybrid encryption.
    /// </summary>
    private EncryptedCreditNote EncryptCreditNoteForRecipient(Recipient recipient, string creditNote)
    {
        try
        {
            _logger.LogDebug("Encrypting credit note for recipient: {RecipientId}", recipient.id);

            // Step 1: Generate unique AES-256 key for this credit note
            var aesKey = _keyGenerator.GenerateKey();

            // Step 2: Encrypt credit note data with AES-GCM
            var encryptedData = _aesEncryptor.Encrypt(creditNote, aesKey);
            var encryptedCreditNoteBase64 = encryptedData.ToBase64String();

            // Step 3: Encrypt AES key with recipient's RSA public key
            var encryptedSymmetricKey = _rsaKeyEncryptor.EncryptKey(aesKey, recipient.publicKey);

            // Step 4: Build encrypted credit note DTO
            return EncryptedCreditNote.CreateBuilder()
                .RecipientId(recipient.id)
                .ReceiverType(recipient.receiverType)
                .PublicKey(recipient.publicKey)
                .EncryptedCreditNote(encryptedCreditNoteBase64)
                .EncryptedSymmetricKey(encryptedSymmetricKey)
                .Build();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to encrypt credit note for recipient {RecipientId}: {Message}", recipient.id, e.Message);
            throw new EncryptionException($"Failed to encrypt credit note for recipient: {recipient.id}", e);
        }
    }
}
