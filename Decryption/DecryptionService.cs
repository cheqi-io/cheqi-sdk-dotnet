using Cheqi.Sdk.CreditNote;
using Cheqi.Sdk.Models;
using Cheqi.Sdk.Models.Generated;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Text.Json;

namespace Cheqi.Sdk.Decryption;

/// <summary>
/// Decryption service for Cheqi hybrid-encrypted content.
///
/// This service orchestrates:
/// - RSA-OAEP decryption for AES keys
/// - AES-256-GCM decryption for content
///
/// Compatible with content encrypted by the Cheqi EncryptionService.
/// </summary>
public class DecryptionService
{
    private readonly ILogger<DecryptionService> _logger;
    private readonly AesDecryptor _aesDecryptor;
    private readonly RsaKeyDecryptor _rsaKeyDecryptor;

    public DecryptionService(ILogger<DecryptionService>? logger = null)
    {
        _logger = logger ?? NullLogger<DecryptionService>.Instance;
        _aesDecryptor = new AesDecryptor();
        _rsaKeyDecryptor = new RsaKeyDecryptor();
        _logger.LogInformation("DecryptionService initialized with AES-256-GCM and RSA-OAEP");
    }

    /// <summary>
    /// Decrypts a hybrid-encrypted receipt using the client's private key.
    ///
    /// This method handles the complete hybrid decryption process:
    /// 1. Decrypts the AES symmetric key using RSA private key
    /// 2. Decrypts the receipt content using the AES key
    /// 3. Decrypts the customer details (if present)
    /// </summary>
    /// <param name="encryptedReceipt">The encrypted receipt DTO from Cheqi API or webhook</param>
    /// <param name="privateKeyBase64">The client's private key in Base64 PKCS#8 format</param>
    /// <returns>DecryptedReceipt containing both receipt content and customer details</returns>
    /// <exception cref="DecryptionException">if decryption fails</exception>
    public DecryptedReceipt DecryptReceipt(EncryptedReceipt encryptedReceipt, string privateKeyBase64)
    {
        try
        {
            byte[] receiptAesKey = _rsaKeyDecryptor.DecryptKey(
                encryptedReceipt.encryptedSymmetricKey ?? throw new DecryptionException("Encrypted symmetric key is null"),
                privateKeyBase64
            );

            string decryptedReceiptContent = _aesDecryptor.Decrypt(
                encryptedReceipt.encryptedReceipt ?? throw new DecryptionException("Encrypted receipt is null"),
                receiptAesKey
            );

            string? decryptedCustomerDetails = null;
            if (HasCustomerDetails(encryptedReceipt))
            {
                byte[] customerAesKey = _rsaKeyDecryptor.DecryptKey(
                    encryptedReceipt.encryptedCustomerAesKey ?? throw new DecryptionException("Encrypted customer AES key is null"),
                    privateKeyBase64
                );

                decryptedCustomerDetails = _aesDecryptor.Decrypt(
                    encryptedReceipt.encryptedCustomerDetails ?? throw new DecryptionException("Encrypted customer details is null"),
                    customerAesKey
                );
            }

            return new DecryptedReceipt(decryptedReceiptContent, decryptedCustomerDetails);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to decrypt receipt for recipient {RecipientId}",
                encryptedReceipt.recipientId);
            throw new DecryptionException("Failed to decrypt receipt", e);
        }
    }

    /// <summary>
    /// Decrypts a hybrid-encrypted credit note request using the issuer's private key.
    ///
    /// This method handles the complete hybrid decryption process:
    /// 1. Decrypts the AES symmetric key using RSA private key
    /// 2. Decrypts the credit note content using the AES key
    /// </summary>
    /// <param name="encryptedCreditNote">The encrypted credit note from Cheqi API or webhook</param>
    /// <param name="privateKeyBase64">The issuer's private key in Base64 PKCS#8 format</param>
    /// <returns>DecryptedCreditNote containing the credit note request details (JSON)</returns>
    /// <exception cref="DecryptionException">if decryption fails</exception>
    public DecryptedCreditNote DecryptCreditNote(
        Cheqi.Sdk.CreditNote.EncryptedCreditNoteInitiationRequest encryptedCreditNote,
        string privateKeyBase64)
    {
        try
        {
            byte[] aesKey = _rsaKeyDecryptor.DecryptKey(
                encryptedCreditNote.EncryptedSymmetricKey ?? throw new DecryptionException("Encrypted symmetric key is null"),
                privateKeyBase64
            );

            string decryptedCreditNoteContent = _aesDecryptor.Decrypt(
                encryptedCreditNote.EncryptedCreditNoteInitiationRequestData ?? throw new DecryptionException("Encrypted credit note data is null"),
                aesKey
            );

            return new DecryptedCreditNote(decryptedCreditNoteContent);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to decrypt credit note for receipt {ReceiptId}",
                encryptedCreditNote.CheqiReceiptId);
            throw new DecryptionException("Failed to decrypt credit note", e);
        }
    }

    public DecryptedDeliveredCreditNote DecryptDeliveredCreditNote(
        EncryptedCreditNoteDto encryptedCreditNote,
        string privateKeyBase64)
    {
        try
        {
            byte[] aesKey = _rsaKeyDecryptor.DecryptKey(
                encryptedCreditNote.EncryptedSymmetricKey ?? throw new DecryptionException("Encrypted symmetric key is null"),
                privateKeyBase64
            );

            string decryptedCreditNoteContent = _aesDecryptor.Decrypt(
                encryptedCreditNote.EncryptedCreditNote ?? throw new DecryptionException("Encrypted credit note data is null"),
                aesKey
            );

            string? decryptedContext = null;
            CreditNoteContextEnvelope? contextEnvelope = null;
            if (!string.IsNullOrWhiteSpace(encryptedCreditNote.EncryptedCustomerDetails) &&
                !string.IsNullOrWhiteSpace(encryptedCreditNote.EncryptedCustomerAesKey))
            {
                byte[] contextAesKey = _rsaKeyDecryptor.DecryptKey(
                    encryptedCreditNote.EncryptedCustomerAesKey,
                    privateKeyBase64
                );

                decryptedContext = _aesDecryptor.Decrypt(
                    encryptedCreditNote.EncryptedCustomerDetails,
                    contextAesKey
                );

                contextEnvelope = JsonSerializer.Deserialize<CreditNoteContextEnvelope>(decryptedContext)
                    ?? throw new DecryptionException("Failed to deserialize credit note context");
            }

            var creditNoteEnvelope = JsonSerializer.Deserialize<CreditNoteEnvelope>(decryptedCreditNoteContent)
                ?? throw new DecryptionException("Failed to deserialize credit note envelope");

            return new DecryptedDeliveredCreditNote(
                creditNoteEnvelope,
                contextEnvelope,
                decryptedCreditNoteContent,
                decryptedContext);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to decrypt delivered credit note for recipient {RecipientId}",
                encryptedCreditNote.RecipientId);
            throw new DecryptionException("Failed to decrypt delivered credit note", e);
        }
    }

    /// <summary>
    /// Checks if the encrypted receipt contains customer details.
    /// </summary>
    private bool HasCustomerDetails(EncryptedReceipt encryptedReceipt)
    {
        return !string.IsNullOrWhiteSpace(encryptedReceipt.encryptedCustomerDetails) &&
               encryptedReceipt.encryptedCustomerAesKey != null;
    }
}
