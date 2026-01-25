using Cheqi.Sdk.CreditNote;
using Cheqi.Sdk.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

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
        _logger.LogInformation("📦 Starting receipt decryption for recipient: {RecipientId}", encryptedReceipt.recipientId);
        _logger.LogInformation("🔍 Encrypted receipt length: {Length} characters", encryptedReceipt.encryptedReceipt?.Length ?? 0);
        _logger.LogInformation("🔍 Encrypted symmetric key length: {Length} characters", encryptedReceipt.encryptedSymmetricKey?.Length ?? 0);

        try
        {
            // Step 1: Decrypt the receipt AES key using RSA private key
            _logger.LogInformation("🔐 Step 1: Decrypting receipt AES key with RSA-OAEP...");
            byte[] receiptAesKey = _rsaKeyDecryptor.DecryptKey(
                encryptedReceipt.encryptedSymmetricKey ?? throw new DecryptionException("Encrypted symmetric key is null"),
                privateKeyBase64
            );
            _logger.LogInformation("✅ Step 1 complete: Receipt AES key decrypted");

            // Step 2: Decrypt the receipt content using the receipt AES key
            _logger.LogInformation("🔐 Step 2: Decrypting receipt content with AES-GCM...");
            string decryptedReceiptContent = _aesDecryptor.Decrypt(
                encryptedReceipt.encryptedReceipt ?? throw new DecryptionException("Encrypted receipt is null"),
                receiptAesKey
            );
            _logger.LogInformation("✅ Step 2 complete: Receipt content decrypted ({Length} characters)", decryptedReceiptContent.Length);

            // Step 3: Decrypt customer details using separate customer AES key (if present)
            string? decryptedCustomerDetails = null;
            if (HasCustomerDetails(encryptedReceipt))
            {
                _logger.LogInformation("🔐 Step 3: Decrypting customer details...");

                byte[] customerAesKey = _rsaKeyDecryptor.DecryptKey(
                    encryptedReceipt.encryptedCustomerAesKey ?? throw new DecryptionException("Encrypted customer AES key is null"),
                    privateKeyBase64
                );

                decryptedCustomerDetails = _aesDecryptor.Decrypt(
                    encryptedReceipt.encryptedCustomerDetails ?? throw new DecryptionException("Encrypted customer details is null"),
                    customerAesKey
                );
                _logger.LogInformation("✅ Step 3 complete: Customer details decrypted ({Length} characters)", decryptedCustomerDetails.Length);
            }
            else
            {
                _logger.LogInformation("ℹ️ Step 3 skipped: No customer details present");
            }

            _logger.LogInformation("✅ Successfully decrypted receipt for recipient: {RecipientId}", encryptedReceipt.recipientId);
            _logger.LogInformation("📊 Decryption summary: receipt={ReceiptLength} chars, customer={CustomerLength} chars",
                decryptedReceiptContent.Length,
                decryptedCustomerDetails?.Length ?? 0);
            return new DecryptedReceipt(decryptedReceiptContent, decryptedCustomerDetails);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "❌ Failed to decrypt receipt for recipient {RecipientId}: {Message}",
                encryptedReceipt.recipientId, e.Message);
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
        EncryptedCreditNoteInitiationRequest encryptedCreditNote,
        string privateKeyBase64)
    {
        _logger.LogInformation("📦 Starting credit note decryption for receipt: {ReceiptId}", encryptedCreditNote.CheqiReceiptId);
        _logger.LogInformation("🔍 Public key (first 100 chars): {Key}...",
            encryptedCreditNote.PublicKey?.Substring(0, Math.Min(100, encryptedCreditNote.PublicKey?.Length ?? 0)));
        _logger.LogInformation("🔍 Encrypted credit note length: {Length} characters",
            encryptedCreditNote.EncryptedCreditNoteInitiationRequestData?.Length ?? 0);
        _logger.LogInformation("🔍 Encrypted symmetric key length: {Length} characters",
            encryptedCreditNote.EncryptedSymmetricKey?.Length ?? 0);

        try
        {
            // Step 1: Decrypt the AES key using RSA private key
            _logger.LogInformation("🔐 Step 1: Decrypting AES key with RSA-OAEP...");
            byte[] aesKey = _rsaKeyDecryptor.DecryptKey(
                encryptedCreditNote.EncryptedSymmetricKey ?? throw new DecryptionException("Encrypted symmetric key is null"),
                privateKeyBase64
            );
            _logger.LogInformation("✅ Step 1 complete: AES key decrypted");

            // Step 2: Decrypt the credit note content using the AES key
            _logger.LogInformation("🔐 Step 2: Decrypting credit note content with AES-GCM...");
            string decryptedCreditNoteContent = _aesDecryptor.Decrypt(
                encryptedCreditNote.EncryptedCreditNoteInitiationRequestData ?? throw new DecryptionException("Encrypted credit note data is null"),
                aesKey
            );
            _logger.LogInformation("✅ Step 2 complete: Credit note content decrypted ({Length} characters)",
                decryptedCreditNoteContent.Length);

            _logger.LogInformation("✅ Successfully decrypted credit note for receipt: {ReceiptId}",
                encryptedCreditNote.CheqiReceiptId);
            _logger.LogInformation("📊 Decryption summary: content={Length} chars", decryptedCreditNoteContent.Length);

            return new DecryptedCreditNote(decryptedCreditNoteContent);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "❌ Failed to decrypt credit note for receipt {ReceiptId}: {Message}",
                encryptedCreditNote.CheqiReceiptId, e.Message);
            throw new DecryptionException("Failed to decrypt credit note", e);
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
