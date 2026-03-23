using Cheqi.Sdk.CreditNote;
using Cheqi.Sdk.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Security.Cryptography;
using EncryptedReceiptRequestDto = Cheqi.Sdk.Models.Generated.EncryptedReceiptRequest;
using Recipient = Cheqi.Sdk.Models.Generated.MatchedRecipient;

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
        if (recipients == null || recipients.Count == 0)
        {
            throw new EncryptionException("Recipients list cannot be null or empty");
        }

        try
        {
            var encryptedReceipts = new HashSet<EncryptedReceiptRequestDto>();
            for (int i = 0; i < recipients.Count; i++)
            {
                var recipient = recipients[i];
                var encrypted = EncryptReceiptForRecipient(recipient, purchaseReceipt);
                encryptedReceipts.Add(encrypted);
            }

            return encryptedReceipts;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to encrypt receipt for recipients");
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
            if (recipient == null)
            {
                throw new ArgumentNullException(nameof(recipient), "Recipient cannot be null");
            }

            if (string.IsNullOrEmpty(recipient.PublicKey))
            {
                throw new EncryptionException($"Recipient {recipient.Id} has no public key");
            }

            var aesKey = _keyGenerator.GenerateKey();

            var encryptedData = _aesEncryptor.Encrypt(purchaseReceipt, aesKey);
            var encryptedReceiptBase64 = encryptedData.ToBase64String();

            try
            {
                var encryptedSymmetricKey = _rsaKeyEncryptor.EncryptKey(aesKey, recipient.PublicKey);

                var dto = new EncryptedReceiptRequestDto();
                dto.RecipientId = recipient.Id;
                dto.PublicKey = recipient.PublicKey;
                dto.EncryptedReceipt = encryptedReceiptBase64;
                dto.EncryptedSymmetricKey = encryptedSymmetricKey;
                return dto;
            }
            catch (Exception rsaException)
            {
                _logger.LogError(rsaException, "RSA encryption failed for recipient {RecipientId}", recipient.Id);
                throw;
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to encrypt receipt for recipient {RecipientId}", recipient?.Id);
            throw new EncryptionException($"Failed to encrypt receipt for recipient: {recipient?.Id}", e);
        }
    }

    /// <summary>
    /// Encrypts a credit note for a single recipient using hybrid encryption.
    /// </summary>
    private EncryptedCreditNote EncryptCreditNoteForRecipient(Recipient recipient, string creditNote)
    {
        try
        {
            _logger.LogDebug("Encrypting credit note for recipient: {RecipientId}", recipient.Id);

            // Step 1: Generate unique AES-256 key for this credit note
            var aesKey = _keyGenerator.GenerateKey();

            // Step 2: Encrypt credit note data with AES-GCM
            var encryptedData = _aesEncryptor.Encrypt(creditNote, aesKey);
            var encryptedCreditNoteBase64 = encryptedData.ToBase64String();

            // Step 3: Encrypt AES key with recipient's RSA public key
            var encryptedSymmetricKey = _rsaKeyEncryptor.EncryptKey(aesKey, recipient.PublicKey);

            // Step 4: Build encrypted credit note DTO
            return EncryptedCreditNote.CreateBuilder()
                .RecipientId(recipient.Id)
                .PublicKey(recipient.PublicKey)
                .EncryptedCreditNote(encryptedCreditNoteBase64)
                .EncryptedSymmetricKey(encryptedSymmetricKey)
                .Build();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to encrypt credit note for recipient {RecipientId}: {Message}", recipient.Id, e.Message);
            throw new EncryptionException($"Failed to encrypt credit note for recipient: {recipient.Id}", e);
        }
    }
}
