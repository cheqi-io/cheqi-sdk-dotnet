using System.Security.Cryptography;
using System.Text;

namespace Cheqi.Sdk.Encryption;

public class AesEncryptor
{
    private const int GCM_IV_LENGTH = 12;
    private const int GCM_TAG_LENGTH = 16;

    private readonly RandomNumberGenerator _secureRandom;

    public AesEncryptor(RandomNumberGenerator secureRandom)
    {
        _secureRandom = secureRandom;
    }

    /// <summary>
    /// Encrypts plaintext with AES-256-GCM.
    /// </summary>
    public EncryptedData Encrypt(string plaintext, byte[] key)
    {
        try
        {
            // Generate random IV
            byte[] iv = new byte[GCM_IV_LENGTH];
            _secureRandom.GetBytes(iv);

            byte[] plaintextBytes = Encoding.UTF8.GetBytes(plaintext);
            byte[] ciphertext = new byte[plaintextBytes.Length];
            byte[] tag = new byte[GCM_TAG_LENGTH];

            // Encrypt with AES-GCM
            using var aesGcm = new AesGcm(key, GCM_TAG_LENGTH);
            aesGcm.Encrypt(iv, plaintextBytes, ciphertext, tag);

            return new EncryptedData(ciphertext, iv, tag);
        }
        catch (Exception e)
        {
            throw new EncryptionException("AES encryption failed", e);
        }
    }
}
