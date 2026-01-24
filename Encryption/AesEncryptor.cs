using System.Security.Cryptography;
using Cheqi.Sdk.Config;

namespace Cheqi.Sdk.Encryption;

/// <summary>
/// AES-GCM encryption implementation.
/// </summary>
public sealed class AesEncryptor
{
    private readonly EncryptionConfig _config;
    private readonly AesKeyGenerator _keyGenerator;

    public AesEncryptor(EncryptionConfig? config = null)
    {
        _config = config ?? EncryptionConfig.Default;
        _keyGenerator = new AesKeyGenerator(_config);
    }

    /// <summary>
    /// Encrypts data using AES-GCM with a randomly generated key.
    /// </summary>
    /// <param name="plaintext">The data to encrypt.</param>
    /// <returns>A tuple of (encrypted data, encryption key).</returns>
    public (EncryptedData EncryptedData, byte[] Key) Encrypt(byte[] plaintext)
    {
        var key = _keyGenerator.GenerateKey();
        var encryptedData = Encrypt(plaintext, key);
        return (encryptedData, key);
    }

    /// <summary>
    /// Encrypts data using AES-GCM with the provided key.
    /// </summary>
    /// <param name="plaintext">The data to encrypt.</param>
    /// <param name="key">The AES key to use.</param>
    /// <returns>The encrypted data including IV and authentication tag.</returns>
    public EncryptedData Encrypt(byte[] plaintext, byte[] key)
    {
        if (plaintext == null || plaintext.Length == 0)
            throw new ArgumentException("Plaintext cannot be null or empty", nameof(plaintext));
        if (key == null || key.Length != _config.AesKeySize / 8)
            throw new ArgumentException($"Key must be {_config.AesKeySize} bits", nameof(key));

        var iv = _keyGenerator.GenerateIv();
        var tagSize = _config.AesTagSize / 8;
        var ciphertext = new byte[plaintext.Length];
        var tag = new byte[tagSize];

        try
        {
#pragma warning disable SYSLIB0053 // AesGcm(byte[]) is obsolete in .NET 8+
            using var aes = new AesGcm(key);
#pragma warning restore SYSLIB0053
            aes.Encrypt(iv, plaintext, ciphertext, tag);
            return new EncryptedData(ciphertext, iv, tag);
        }
        catch (Exception ex)
        {
            throw new EncryptionException("Failed to encrypt data with AES-GCM", ex);
        }
    }

    /// <summary>
    /// Encrypts a string using AES-GCM.
    /// </summary>
    /// <param name="plaintext">The string to encrypt.</param>
    /// <returns>A tuple of (encrypted data, encryption key).</returns>
    public (EncryptedData EncryptedData, byte[] Key) Encrypt(string plaintext)
    {
        var plaintextBytes = System.Text.Encoding.UTF8.GetBytes(plaintext);
        return Encrypt(plaintextBytes);
    }

    /// <summary>
    /// Encrypts a string using AES-GCM with the provided key.
    /// </summary>
    /// <param name="plaintext">The string to encrypt.</param>
    /// <param name="key">The AES key to use.</param>
    /// <returns>The encrypted data.</returns>
    public EncryptedData Encrypt(string plaintext, byte[] key)
    {
        var plaintextBytes = System.Text.Encoding.UTF8.GetBytes(plaintext);
        return Encrypt(plaintextBytes, key);
    }
}
