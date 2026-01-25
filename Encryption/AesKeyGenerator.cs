using System.Security.Cryptography;

namespace Cheqi.Sdk.Encryption;

/// <summary>
/// Generates AES-256 keys for receipt encryption.
/// </summary>
public class AesKeyGenerator
{
    private const int AES_KEY_LENGTH = 256;

    private readonly RandomNumberGenerator _secureRandom;

    public AesKeyGenerator(RandomNumberGenerator secureRandom)
    {
        _secureRandom = secureRandom;
    }

    /// <summary>
    /// Generates a new random AES-256 key.
    /// </summary>
    public byte[] GenerateKey()
    {
        try
        {
            byte[] key = new byte[AES_KEY_LENGTH / 8]; // 32 bytes for AES-256
            _secureRandom.GetBytes(key);
            return key;
        }
        catch (Exception e)
        {
            throw new EncryptionException("Failed to generate AES key", e);
        }
    }
}
