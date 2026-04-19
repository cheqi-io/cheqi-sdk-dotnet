using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Security.Cryptography;

namespace Cheqi.Sdk.Decryption;

/// <summary>
/// Decrypts AES keys using RSA-OAEP with a private key.
/// </summary>
public class RsaKeyDecryptor
{
    private readonly ILogger<RsaKeyDecryptor> _logger;

    public RsaKeyDecryptor(ILogger<RsaKeyDecryptor>? logger = null)
    {
        _logger = logger ?? NullLogger<RsaKeyDecryptor>.Instance;
    }

    /// <summary>
    /// Decrypts an AES key that was encrypted with RSA-OAEP.
    /// </summary>
    /// <param name="encryptedKeyBase64">Base64-encoded encrypted AES key</param>
    /// <param name="privateKeyBase64">Base64-encoded RSA private key in PKCS#8 format</param>
    /// <returns>Decrypted AES key bytes</returns>
    /// <exception cref="DecryptionException">if decryption fails</exception>
    public byte[] DecryptKey(string encryptedKeyBase64, string privateKeyBase64)
    {
        try
        {
            byte[] encryptedKeyBytes = Convert.FromBase64String(encryptedKeyBase64);

            using var rsa = ParsePrivateKey(privateKeyBase64);

            byte[] decryptedKeyBytes = rsa.Decrypt(encryptedKeyBytes, RSAEncryptionPadding.OaepSHA256);

            if (decryptedKeyBytes.Length != 32)
            {
                _logger.LogError("Decrypted AES key size mismatch: expected 32 bytes, got {Length}", decryptedKeyBytes.Length);
                throw new DecryptionException($"Decrypted AES key is not 32 bytes: {decryptedKeyBytes.Length}");
            }

            return decryptedKeyBytes;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "RSA key decryption failed");
            throw new DecryptionException("Failed to decrypt AES key with RSA", e);
        }
    }

    /// <summary>
    /// Parses a Base64-encoded PKCS#8 private key.
    /// </summary>
    private RSA ParsePrivateKey(string privateKeyBase64)
    {
        try
        {
            byte[] keyBytes = Convert.FromBase64String(privateKeyBase64);

            var rsa = RSA.Create();
            rsa.ImportPkcs8PrivateKey(keyBytes, out _);
            return rsa;
        }
        catch (Exception e)
        {
            const string message = "Failed to parse RSA private key. Expected PKCS#8 format " +
                "(-----BEGIN PRIVATE KEY----- / Base64 PKCS#8 DER).";
            _logger.LogError(e, message);
            throw new DecryptionException(message, e);
        }
    }
}
