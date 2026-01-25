using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Security.Cryptography;
using System.Text;

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
            _logger.LogInformation("🔐 Starting RSA-OAEP AES key decryption");
            _logger.LogInformation("🔑 Encrypted key Base64 length: {Length} characters", encryptedKeyBase64.Length);
            _logger.LogInformation("🔑 Encrypted key Base64 (first 100 chars): {Key}...",
                encryptedKeyBase64.Substring(0, Math.Min(100, encryptedKeyBase64.Length)));

            byte[] encryptedKeyBytes = Convert.FromBase64String(encryptedKeyBase64);
            _logger.LogInformation("✅ Decoded encrypted key: {Length} bytes", encryptedKeyBytes.Length);
            _logger.LogInformation("🔍 Encrypted key (hex, first 32 bytes): {Hex}", BytesToHex(encryptedKeyBytes, 32));

            using var rsa = ParsePrivateKey(privateKeyBase64);
            _logger.LogInformation("✅ Parsed private key successfully");

            _logger.LogInformation("🔧 Using RSA-OAEP with SHA-256 and MGF1");

            byte[] decryptedKeyBytes = rsa.Decrypt(encryptedKeyBytes, RSAEncryptionPadding.OaepSHA256);
            _logger.LogInformation("✅ RSA decryption successful: {Length} bytes", decryptedKeyBytes.Length);
            _logger.LogInformation("🔍 Decrypted AES key (hex, first 16 bytes): {Hex}", BytesToHex(decryptedKeyBytes, 16));

            if (decryptedKeyBytes.Length != 32)
            {
                _logger.LogError("❌ Decrypted AES key size mismatch: expected 32 bytes, got {Length}", decryptedKeyBytes.Length);
                throw new DecryptionException($"Decrypted AES key is not 32 bytes: {decryptedKeyBytes.Length}");
            }

            _logger.LogInformation("✅ AES key decryption completed successfully");
            return decryptedKeyBytes;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "❌ RSA key decryption failed: {Message}", e.Message);
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
            _logger.LogDebug("🔑 Parsing private key from Base64 (PKCS#8 format)");
            byte[] keyBytes = Convert.FromBase64String(privateKeyBase64);
            _logger.LogDebug("✅ Decoded private key: {Length} bytes", keyBytes.Length);

            var rsa = RSA.Create();
            rsa.ImportPkcs8PrivateKey(keyBytes, out _);
            _logger.LogDebug("✅ Generated RSA private key");
            return rsa;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "❌ Failed to parse RSA private key: {Message}", e.Message);
            throw new DecryptionException("Failed to parse RSA private key", e);
        }
    }

    /// <summary>
    /// Converts byte array to hex string for logging.
    /// </summary>
    private string BytesToHex(byte[] bytes, int maxBytes)
    {
        int length = Math.Min(bytes.Length, maxBytes);
        var sb = new StringBuilder();
        for (int i = 0; i < length; i++)
        {
            sb.Append($"{bytes[i]:x2}");
        }
        return sb.ToString();
    }
}
