using System.Security.Cryptography;
using System.Text.Json;
using Cheqi.Sdk.Config;
using Cheqi.Sdk.Models;

namespace Cheqi.Sdk.Encryption;

/// <summary>
/// Service for decrypting receipts and credit notes.
/// </summary>
public sealed class DecryptionService
{
    private readonly EncryptionConfig _config;
    private readonly AesDecryptor _aesDecryptor;
    private readonly RsaKeyDecryptor _rsaKeyDecryptor;
    private readonly JsonSerializerOptions _jsonOptions;

    public DecryptionService(RSA? privateKey = null, EncryptionConfig? config = null)
    {
        _config = config ?? EncryptionConfig.Default;
        _aesDecryptor = new AesDecryptor(_config);
        _rsaKeyDecryptor = new RsaKeyDecryptor(privateKey);
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    /// <summary>
    /// Decrypts encrypted receipt data.
    /// </summary>
    /// <param name="encryptedData">The encrypted receipt data.</param>
    /// <param name="privateKey">The RSA private key (optional, uses instance key if not provided).</param>
    /// <returns>The decrypted receipt as JSON string.</returns>
    public string DecryptReceipt(EncryptedReceiptData encryptedData, RSA? privateKey = null)
    {
        if (encryptedData == null)
            throw new ArgumentNullException(nameof(encryptedData));

        try
        {
            // Decrypt the AES key using RSA
            var aesKey = _rsaKeyDecryptor.DecryptKey(encryptedData.EncryptedKey, privateKey);

            // Decrypt the data using AES-GCM
            var decrypted = _aesDecryptor.Decrypt(
                encryptedData.EncryptedData,
                encryptedData.Iv,
                encryptedData.AuthTag,
                aesKey);

            var decryptedJson = System.Text.Encoding.UTF8.GetString(decrypted);

            // Verify hash if present
            if (!string.IsNullOrEmpty(encryptedData.DataHash))
            {
                var computedHash = Utils.HashUtils.ComputeSha256Hash(decryptedJson);
                if (!string.Equals(computedHash, encryptedData.DataHash, StringComparison.OrdinalIgnoreCase))
                {
                    throw new DecryptionException("Data integrity check failed: hash mismatch");
                }
            }

            return decryptedJson;
        }
        catch (DecryptionException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new DecryptionException("Failed to decrypt receipt", ex);
        }
    }

    /// <summary>
    /// Decrypts encrypted receipt data to a typed object.
    /// </summary>
    /// <typeparam name="T">The receipt type to deserialize to.</typeparam>
    /// <param name="encryptedData">The encrypted receipt data.</param>
    /// <param name="privateKey">The RSA private key (optional).</param>
    /// <returns>The decrypted and deserialized receipt.</returns>
    public T DecryptReceipt<T>(EncryptedReceiptData encryptedData, RSA? privateKey = null)
    {
        var json = DecryptReceipt(encryptedData, privateKey);
        return JsonSerializer.Deserialize<T>(json, _jsonOptions)
            ?? throw new DecryptionException("Failed to deserialize decrypted receipt");
    }

    /// <summary>
    /// Decrypts encrypted credit note data.
    /// </summary>
    /// <param name="encryptedData">The encrypted credit note data.</param>
    /// <param name="privateKey">The RSA private key (optional).</param>
    /// <returns>The decrypted credit note as JSON string.</returns>
    public string DecryptCreditNote(CreditNote.EncryptedCreditNoteData encryptedData, RSA? privateKey = null)
    {
        if (encryptedData == null)
            throw new ArgumentNullException(nameof(encryptedData));

        try
        {
            var aesKey = _rsaKeyDecryptor.DecryptKey(encryptedData.EncryptedKey, privateKey);
            var decrypted = _aesDecryptor.Decrypt(
                encryptedData.EncryptedData,
                encryptedData.Iv,
                encryptedData.AuthTag,
                aesKey);

            var decryptedJson = System.Text.Encoding.UTF8.GetString(decrypted);

            if (!string.IsNullOrEmpty(encryptedData.DataHash))
            {
                var computedHash = Utils.HashUtils.ComputeSha256Hash(decryptedJson);
                if (!string.Equals(computedHash, encryptedData.DataHash, StringComparison.OrdinalIgnoreCase))
                {
                    throw new DecryptionException("Data integrity check failed: hash mismatch");
                }
            }

            return decryptedJson;
        }
        catch (DecryptionException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new DecryptionException("Failed to decrypt credit note", ex);
        }
    }

    /// <summary>
    /// Decrypts encrypted credit note data to a typed object.
    /// </summary>
    /// <typeparam name="T">The credit note type to deserialize to.</typeparam>
    /// <param name="encryptedData">The encrypted credit note data.</param>
    /// <param name="privateKey">The RSA private key (optional).</param>
    /// <returns>The decrypted and deserialized credit note.</returns>
    public T DecryptCreditNote<T>(CreditNote.EncryptedCreditNoteData encryptedData, RSA? privateKey = null)
    {
        var json = DecryptCreditNote(encryptedData, privateKey);
        return JsonSerializer.Deserialize<T>(json, _jsonOptions)
            ?? throw new DecryptionException("Failed to deserialize decrypted credit note");
    }
}
