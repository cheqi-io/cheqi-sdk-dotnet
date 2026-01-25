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
        _logger.LogDebug("Encrypting receipt for {RecipientCount} recipients", recipients.Count);

        try
        {
            var encryptedReceipts = recipients
                .Select(recipient => EncryptReceiptForRecipient(recipient, purchaseReceipt))
                .ToHashSet();

            _logger.LogInformation("Successfully encrypted receipt for {RecipientCount} recipients", encryptedReceipts.Count);
            return encryptedReceipts;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to encrypt receipt for recipients: {Message}", e.Message);
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
            _logger.LogDebug("Encrypting receipt for recipient: {RecipientId}", recipient.id);

            // Step 1: Generate unique AES-256 key for this receipt
            var aesKey = _keyGenerator.GenerateKey();

            // Step 2: Encrypt receipt data with AES-GCM
            var encryptedData = _aesEncryptor.Encrypt(purchaseReceipt, aesKey);
            var encryptedReceiptBase64 = encryptedData.ToBase64String();

            // Step 3: Encrypt AES key with recipient's RSA public key
            var encryptedSymmetricKey = _rsaKeyEncryptor.EncryptKey(aesKey, recipient.publicKey);

            // Step 4: Build encrypted receipt DTO
            return EncryptedReceiptRequestDto.Builder()
                .RecipientId(recipient.id)
                .PublicKey(recipient.publicKey)
                .ReceiverType(recipient.receiverType)
                .EncryptedReceipt(encryptedReceiptBase64)
                .EncryptedSymmetricKey(encryptedSymmetricKey)
                .Build();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to encrypt receipt for recipient {RecipientId}: {Message}", recipient.id, e.Message);
            throw new EncryptionException($"Failed to encrypt receipt for recipient: {recipient.id}", e);
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
