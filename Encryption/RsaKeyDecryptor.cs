using System.Security.Cryptography;

namespace Cheqi.Sdk.Encryption;

/// <summary>
/// RSA-OAEP decryption for unwrapping AES keys.
/// </summary>
public sealed class RsaKeyDecryptor
{
    private readonly RSA? _privateKey;

    public RsaKeyDecryptor(RSA? privateKey = null)
    {
        _privateKey = privateKey;
    }

    /// <summary>
    /// Decrypts an AES key using RSA-OAEP with SHA-256.
    /// </summary>
    /// <param name="encryptedKey">The encrypted AES key.</param>
    /// <param name="privateKey">The RSA private key (optional, uses instance key if not provided).</param>
    /// <returns>The decrypted AES key.</returns>
    public byte[] DecryptKey(byte[] encryptedKey, RSA? privateKey = null)
    {
        var key = privateKey ?? _privateKey;
        if (key == null)
            throw new InvalidOperationException("No private key provided");
        if (encryptedKey == null || encryptedKey.Length == 0)
            throw new ArgumentException("Encrypted key cannot be null or empty", nameof(encryptedKey));

        try
        {
            return key.Decrypt(encryptedKey, RSAEncryptionPadding.OaepSHA256);
        }
        catch (CryptographicException ex)
        {
            throw new DecryptionException("Failed to decrypt key with RSA-OAEP: key may be corrupted or wrong private key", ex);
        }
        catch (Exception ex)
        {
            throw new DecryptionException("Failed to decrypt key with RSA-OAEP", ex);
        }
    }

    /// <summary>
    /// Decrypts an AES key from Base64-encoded encrypted key.
    /// </summary>
    /// <param name="encryptedKeyBase64">The Base64-encoded encrypted key.</param>
    /// <param name="privateKey">The RSA private key (optional).</param>
    /// <returns>The decrypted AES key.</returns>
    public byte[] DecryptKey(string encryptedKeyBase64, RSA? privateKey = null)
    {
        var encryptedKey = Convert.FromBase64String(encryptedKeyBase64);
        return DecryptKey(encryptedKey, privateKey);
    }

    /// <summary>
    /// Parses an RSA private key from PEM format.
    /// </summary>
    /// <param name="privateKeyPem">The private key in PEM format.</param>
    /// <returns>The RSA instance with the imported private key.</returns>
    public static RSA ParsePrivateKey(string privateKeyPem)
    {
        if (string.IsNullOrEmpty(privateKeyPem))
            throw new ArgumentException("Private key PEM cannot be null or empty", nameof(privateKeyPem));

        try
        {
            var rsa = RSA.Create();
            rsa.ImportFromPem(privateKeyPem);
            return rsa;
        }
        catch (Exception ex)
        {
            throw new DecryptionException("Failed to parse RSA private key from PEM", ex);
        }
    }
}
