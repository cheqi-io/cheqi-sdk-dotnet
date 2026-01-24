using System.Text.Json;
using Cheqi.Sdk.Config;
using Cheqi.Sdk.Models;

namespace Cheqi.Sdk.Encryption;

/// <summary>
/// Service for encrypting receipts and credit notes for multiple recipients.
/// Uses hybrid encryption: AES-256-GCM for data, RSA-OAEP for key wrapping.
/// </summary>
public sealed class EncryptionService
{
    private readonly EncryptionConfig _config;
    private readonly AesEncryptor _aesEncryptor;
    private readonly RsaKeyEncryptor _rsaKeyEncryptor;
    private readonly JsonSerializerOptions _jsonOptions;

    public EncryptionService(EncryptionConfig? config = null)
    {
        _config = config ?? EncryptionConfig.Default;
        _aesEncryptor = new AesEncryptor(_config);
        _rsaKeyEncryptor = new RsaKeyEncryptor();
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
    }

    /// <summary>
    /// Encrypts a receipt for multiple recipients.
    /// </summary>
    /// <param name="receiptJson">The receipt as a JSON string.</param>
    /// <param name="recipients">The list of recipients with their public keys.</param>
    /// <returns>Encrypted receipt data for each recipient.</returns>
    public List<EncryptedReceiptData> EncryptReceiptForRecipients(string receiptJson, IEnumerable<Recipient> recipients)
    {
        if (string.IsNullOrEmpty(receiptJson))
            throw new ArgumentException("Receipt JSON cannot be null or empty", nameof(receiptJson));

        var recipientList = recipients?.ToList() ?? throw new ArgumentNullException(nameof(recipients));
        if (recipientList.Count == 0)
            throw new ArgumentException("At least one recipient is required", nameof(recipients));

        var result = new List<EncryptedReceiptData>();

        foreach (var recipient in recipientList)
        {
            var encryptedData = EncryptForRecipient(receiptJson, recipient);
            result.Add(encryptedData);
        }

        return result;
    }

    /// <summary>
    /// Encrypts a receipt object for multiple recipients.
    /// </summary>
    /// <typeparam name="T">The receipt type.</typeparam>
    /// <param name="receipt">The receipt object.</param>
    /// <param name="recipients">The list of recipients.</param>
    /// <returns>Encrypted receipt data for each recipient.</returns>
    public List<EncryptedReceiptData> EncryptReceiptForRecipients<T>(T receipt, IEnumerable<Recipient> recipients)
    {
        var receiptJson = JsonSerializer.Serialize(receipt, _jsonOptions);
        return EncryptReceiptForRecipients(receiptJson, recipients);
    }

    /// <summary>
    /// Encrypts data for a single recipient.
    /// </summary>
    private EncryptedReceiptData EncryptForRecipient(string data, Recipient recipient)
    {
        if (string.IsNullOrEmpty(recipient.PublicKey))
            throw new EncryptionException($"Recipient {recipient.Id} has no public key");

        try
        {
            // Encrypt data with AES-GCM
            var (encryptedData, aesKey) = _aesEncryptor.Encrypt(data);

            // Encrypt AES key with recipient's RSA public key
            var encryptedKey = _rsaKeyEncryptor.EncryptKeyToBase64(aesKey, recipient.PublicKey);

            // Compute hash of original data for integrity verification
            var dataHash = Utils.HashUtils.ComputeSha256Hash(data);

            return new EncryptedReceiptData
            {
                RecipientId = recipient.Id,
                EncryptedData = encryptedData.CiphertextBase64,
                Iv = encryptedData.IvBase64,
                AuthTag = encryptedData.AuthTagBase64,
                EncryptedKey = encryptedKey,
                DataHash = dataHash
            };
        }
        catch (EncryptionException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new EncryptionException($"Failed to encrypt data for recipient {recipient.Id}", ex);
        }
    }

    /// <summary>
    /// Encrypts a credit note for multiple recipients.
    /// </summary>
    public List<CreditNote.EncryptedCreditNoteData> EncryptCreditNoteForRecipients(
        string creditNoteJson,
        IEnumerable<Recipient> recipients)
    {
        if (string.IsNullOrEmpty(creditNoteJson))
            throw new ArgumentException("Credit note JSON cannot be null or empty", nameof(creditNoteJson));

        var recipientList = recipients?.ToList() ?? throw new ArgumentNullException(nameof(recipients));
        if (recipientList.Count == 0)
            throw new ArgumentException("At least one recipient is required", nameof(recipients));

        var result = new List<CreditNote.EncryptedCreditNoteData>();

        foreach (var recipient in recipientList)
        {
            var encryptedData = EncryptCreditNoteForRecipient(creditNoteJson, recipient);
            result.Add(encryptedData);
        }

        return result;
    }

    /// <summary>
    /// Encrypts a credit note for a single recipient.
    /// </summary>
    private CreditNote.EncryptedCreditNoteData EncryptCreditNoteForRecipient(string data, Recipient recipient)
    {
        if (string.IsNullOrEmpty(recipient.PublicKey))
            throw new EncryptionException($"Recipient {recipient.Id} has no public key");

        try
        {
            var (encryptedData, aesKey) = _aesEncryptor.Encrypt(data);
            var encryptedKey = _rsaKeyEncryptor.EncryptKeyToBase64(aesKey, recipient.PublicKey);
            var dataHash = Utils.HashUtils.ComputeSha256Hash(data);

            return new CreditNote.EncryptedCreditNoteData
            {
                RecipientId = recipient.Id,
                EncryptedData = encryptedData.CiphertextBase64,
                Iv = encryptedData.IvBase64,
                AuthTag = encryptedData.AuthTagBase64,
                EncryptedKey = encryptedKey,
                DataHash = dataHash
            };
        }
        catch (EncryptionException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new EncryptionException($"Failed to encrypt credit note for recipient {recipient.Id}", ex);
        }
    }
}
