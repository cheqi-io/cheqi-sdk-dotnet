using System.Security.Cryptography;
using Cheqi.Sdk.Config;

namespace Cheqi.Sdk.Encryption;

/// <summary>
/// Generates cryptographically secure AES keys.
/// </summary>
public sealed class AesKeyGenerator
{
    private readonly EncryptionConfig _config;

    public AesKeyGenerator(EncryptionConfig? config = null)
    {
        _config = config ?? EncryptionConfig.Default;
    }

    /// <summary>
    /// Generates a new random AES key.
    /// </summary>
    /// <returns>A cryptographically secure random key.</returns>
    public byte[] GenerateKey()
    {
        var keySize = _config.AesKeySize / 8; // Convert bits to bytes
        var key = new byte[keySize];
        RandomNumberGenerator.Fill(key);
        return key;
    }

    /// <summary>
    /// Generates a new random initialization vector (nonce) for AES-GCM.
    /// </summary>
    /// <returns>A cryptographically secure random IV.</returns>
    public byte[] GenerateIv()
    {
        var iv = new byte[_config.AesIvSize];
        RandomNumberGenerator.Fill(iv);
        return iv;
    }
}
