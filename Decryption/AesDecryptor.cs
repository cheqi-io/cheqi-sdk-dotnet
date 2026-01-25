using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Security.Cryptography;
using System.Text;

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
            _logger.LogInformation("🔐 Starting AES-GCM decryption");
            _logger.LogInformation("🔍 Encrypted data Base64 length: {Length} characters", encryptedDataBase64.Length);

            // Decode the combined data (IV + ciphertext + tag)
            byte[] combinedData = Convert.FromBase64String(encryptedDataBase64);
            _logger.LogInformation("✅ Decoded combined data: {Length} bytes", combinedData.Length);
            _logger.LogInformation("🔍 Combined data (hex, first 32 bytes): {Hex}", BytesToHex(combinedData, 32));

            // Extract components
            byte[] iv = new byte[GCM_IV_LENGTH];
            byte[] ciphertext = new byte[combinedData.Length - GCM_IV_LENGTH - GCM_TAG_LENGTH];
            byte[] tag = new byte[GCM_TAG_LENGTH];

            Array.Copy(combinedData, 0, iv, 0, GCM_IV_LENGTH);
            Array.Copy(combinedData, GCM_IV_LENGTH, ciphertext, 0, ciphertext.Length);
            Array.Copy(combinedData, GCM_IV_LENGTH + ciphertext.Length, tag, 0, GCM_TAG_LENGTH);

            _logger.LogInformation("✅ Extracted components: IV={IvLength} bytes, ciphertext={CiphertextLength} bytes, tag={TagLength} bytes",
                iv.Length, ciphertext.Length, tag.Length);
            _logger.LogInformation("🔍 IV (hex): {Hex}", BytesToHex(iv, iv.Length));
            _logger.LogInformation("🔍 Tag (hex): {Hex}", BytesToHex(tag, tag.Length));

            // Reconstruct ciphertext + tag for AES-GCM
            byte[] ciphertextWithTag = new byte[ciphertext.Length + tag.Length];
            Array.Copy(ciphertext, 0, ciphertextWithTag, 0, ciphertext.Length);
            Array.Copy(tag, 0, ciphertextWithTag, ciphertext.Length, tag.Length);
            _logger.LogInformation("✅ Reconstructed ciphertext+tag: {Length} bytes", ciphertextWithTag.Length);

            // Decrypt with AES-GCM
            using var aesGcm = new AesGcm(aesKey);
            _logger.LogInformation("🔧 Using AES-GCM with tag length: {TagLength} bits", GCM_TAG_LENGTH * 8);

            byte[] decryptedBytes = new byte[ciphertext.Length];
            aesGcm.Decrypt(iv, ciphertext, tag, decryptedBytes);
            _logger.LogInformation("✅ AES-GCM decryption successful: {Length} bytes", decryptedBytes.Length);

            string decryptedString = Encoding.UTF8.GetString(decryptedBytes);
            _logger.LogInformation("✅ Converted to UTF-8 string: {Length} characters", decryptedString.Length);
            _logger.LogInformation("🔍 Decrypted content (first 200 chars): {Content}...",
                decryptedString.Substring(0, Math.Min(200, decryptedString.Length)));

            return decryptedString;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "❌ AES-GCM decryption failed: {Message}", e.Message);
            throw new DecryptionException("AES decryption failed", e);
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
