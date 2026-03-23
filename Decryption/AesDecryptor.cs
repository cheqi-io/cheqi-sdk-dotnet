using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Security.Cryptography;

namespace Cheqi.Sdk.Decryption;

/// <summary>
/// Handles AES-256-GCM decryption for receipt content.
/// </summary>
public class AesDecryptor
{
    private readonly ILogger<AesDecryptor> _logger;
    private const int GCM_IV_LENGTH = 12;
    private const int GCM_TAG_LENGTH = 16;

    public AesDecryptor(ILogger<AesDecryptor>? logger = null)
    {
        _logger = logger ?? NullLogger<AesDecryptor>.Instance;
    }

    /// <summary>
    /// Decrypts AES-GCM encrypted content.
    /// </summary>
    /// <param name="encryptedDataBase64">Base64-encoded data in format: IV + ciphertext + tag</param>
    /// <param name="aesKey">The AES key to use for decryption</param>
    /// <returns>Decrypted plaintext string</returns>
    /// <exception cref="DecryptionException">if decryption fails</exception>
    public string Decrypt(string encryptedDataBase64, byte[] aesKey)
    {
        try
        {
            byte[] combinedData = Convert.FromBase64String(encryptedDataBase64);

            byte[] iv = new byte[GCM_IV_LENGTH];
            byte[] ciphertext = new byte[combinedData.Length - GCM_IV_LENGTH - GCM_TAG_LENGTH];
            byte[] tag = new byte[GCM_TAG_LENGTH];

            Array.Copy(combinedData, 0, iv, 0, GCM_IV_LENGTH);
            Array.Copy(combinedData, GCM_IV_LENGTH, ciphertext, 0, ciphertext.Length);
            Array.Copy(combinedData, GCM_IV_LENGTH + ciphertext.Length, tag, 0, GCM_TAG_LENGTH);

            byte[] ciphertextWithTag = new byte[ciphertext.Length + tag.Length];
            Array.Copy(ciphertext, 0, ciphertextWithTag, 0, ciphertext.Length);
            Array.Copy(tag, 0, ciphertextWithTag, ciphertext.Length, tag.Length);

            using var aesGcm = new AesGcm(aesKey);

            byte[] decryptedBytes = new byte[ciphertext.Length];
            aesGcm.Decrypt(iv, ciphertext, tag, decryptedBytes);
            return System.Text.Encoding.UTF8.GetString(decryptedBytes);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "AES-GCM decryption failed");
            throw new DecryptionException("AES decryption failed", e);
        }
    }
}
