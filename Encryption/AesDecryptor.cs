using System.Security.Cryptography;
using Cheqi.Sdk.Config;

namespace Cheqi.Sdk.Encryption;

/// <summary>
/// AES-GCM decryption implementation.
/// </summary>
public sealed class AesDecryptor
{
    private readonly EncryptionConfig _config;

    public AesDecryptor(EncryptionConfig? config = null)
    {
        _config = config ?? EncryptionConfig.Default;
    }

    /// <summary>
    /// Decrypts AES-GCM encrypted data.
    /// </summary>
    /// <param name="encryptedData">The encrypted data including IV and tag.</param>
    /// <param name="key">The AES key.</param>
    /// <returns>The decrypted plaintext.</returns>
    public byte[] Decrypt(EncryptedData encryptedData, byte[] key)
    {
        if (encryptedData == null)
            throw new ArgumentNullException(nameof(encryptedData));
        if (key == null || key.Length != _config.AesKeySize / 8)
            throw new ArgumentException($"Key must be {_config.AesKeySize} bits", nameof(key));

        var plaintext = new byte[encryptedData.Ciphertext.Length];

        try
        {
#pragma warning disable SYSLIB0053 // AesGcm(byte[]) is obsolete in .NET 8+
            using var aes = new AesGcm(key);
#pragma warning restore SYSLIB0053
            aes.Decrypt(encryptedData.Iv, encryptedData.Ciphertext, encryptedData.AuthTag, plaintext);
            return plaintext;
        }
        catch (CryptographicException ex)
        {
            throw new DecryptionException("Failed to decrypt data: authentication tag mismatch or corrupted data", ex);
        }
        catch (Exception ex)
        {
            throw new DecryptionException("Failed to decrypt data with AES-GCM", ex);
        }
    }

    /// <summary>
    /// Decrypts AES-GCM encrypted data from Base64 strings.
    /// </summary>
    /// <param name="ciphertextBase64">Base64-encoded ciphertext.</param>
    /// <param name="ivBase64">Base64-encoded IV.</param>
    /// <param name="authTagBase64">Base64-encoded authentication tag.</param>
    /// <param name="key">The AES key.</param>
    /// <returns>The decrypted plaintext.</returns>
    public byte[] Decrypt(string ciphertextBase64, string ivBase64, string authTagBase64, byte[] key)
    {
        var ciphertext = Convert.FromBase64String(ciphertextBase64);
        var iv = Convert.FromBase64String(ivBase64);
        var authTag = Convert.FromBase64String(authTagBase64);
        var encryptedData = new EncryptedData(ciphertext, iv, authTag);
        return Decrypt(encryptedData, key);
    }

    /// <summary>
    /// Decrypts AES-GCM encrypted data and returns as string.
    /// </summary>
    /// <param name="encryptedData">The encrypted data.</param>
    /// <param name="key">The AES key.</param>
    /// <returns>The decrypted string.</returns>
    public string DecryptToString(EncryptedData encryptedData, byte[] key)
    {
        var plaintext = Decrypt(encryptedData, key);
        return System.Text.Encoding.UTF8.GetString(plaintext);
    }
}
