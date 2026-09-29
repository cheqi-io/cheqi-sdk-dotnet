using System.Security.Cryptography;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Cheqi.Sdk.Encryption;

/// <summary>
/// Encrypts AES keys with RSA-OAEP for hybrid encryption.
/// </summary>
public class RsaKeyEncryptor
{
    private readonly RsaPublicKeyParser _keyParser;
    private readonly ILogger<RsaKeyEncryptor> _logger;

    public RsaKeyEncryptor(ILogger<RsaKeyEncryptor>? logger = null)
    {
        _keyParser = new RsaPublicKeyParser();
        _logger = logger ?? NullLogger<RsaKeyEncryptor>.Instance;
    }

    /// <summary>
    /// Encrypts an AES key with RSA-OAEP using the recipient's public key.
    /// </summary>
    public string EncryptKey(byte[] aesKey, string publicKeyBase64)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(aesKey);
            _logger.LogDebug("[RSA] Starting RSA key encryption");
            _logger.LogDebug("[RSA] AES key length: {Length} bytes", aesKey.Length);
            _logger.LogDebug("[RSA] Public key Base64 length: {Length} characters", publicKeyBase64?.Length ?? 0);
            
            if (string.IsNullOrEmpty(publicKeyBase64))
            {
                _logger.LogError("[RSA] Public key is null or empty");
                throw new ArgumentException("Public key cannot be null or empty", nameof(publicKeyBase64));
            }
            
            _logger.LogDebug("[RSA] Parsing public key...");
            using var publicKey = _keyParser.Parse(publicKeyBase64);
            _logger.LogDebug("[RSA] Public key parsed successfully, key size: {KeySize} bits", publicKey.KeySize);

            // Encrypt with RSA-OAEP (SHA-256, MGF1)
            _logger.LogDebug("[RSA] Encrypting AES key with RSA-OAEP (SHA-256)...");
            byte[] encryptedKey = publicKey.Encrypt(aesKey, RSAEncryptionPadding.OaepSHA256);
            _logger.LogDebug("[RSA] Encrypted key length: {Length} bytes", encryptedKey.Length);

            var result = Convert.ToBase64String(encryptedKey);
            _logger.LogDebug("[RSA] RSA encryption successful, Base64 result length: {Length}", result.Length);
            return result;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "[RSA] Failed to encrypt AES key with RSA: {Message}. Type: {ExceptionType}", 
                e.Message, e.GetType().Name);
            throw new EncryptionException("Failed to encrypt AES key with RSA", e);
        }
    }

    /// <summary>
    /// Encrypts an AES key using RSA-OAEP with SHA-256.
    /// </summary>
    /// <param name="key">The AES key to encrypt.</param>
    /// <param name="publicKey">The RSA public key.</param>
    /// <returns>The encrypted key.</returns>
    public byte[] EncryptKey(byte[] key, RSA publicKey)
    {
        if (key == null || key.Length == 0)
            throw new ArgumentException("Key cannot be null or empty", nameof(key));
        if (publicKey == null)
            throw new ArgumentNullException(nameof(publicKey));

        try
        {
            return publicKey.Encrypt(key, RSAEncryptionPadding.OaepSHA256);
        }
        catch (Exception ex)
        {
            throw new EncryptionException("Failed to encrypt key with RSA-OAEP", ex);
        }
    }

    /// <summary>
    /// Encrypts an AES key using RSA-OAEP with SHA-256 and returns Base64.
    /// </summary>
    /// <param name="key">The AES key to encrypt.</param>
    /// <param name="publicKey">The RSA public key.</param>
    /// <returns>The encrypted key as Base64 string.</returns>
    public string EncryptKeyToBase64(byte[] key, RSA publicKey)
    {
        var encryptedKey = EncryptKey(key, publicKey);
        return Convert.ToBase64String(encryptedKey);
    }

    /// <summary>
    /// Encrypts an AES key using RSA-OAEP with SHA-256 from PEM-encoded public key.
    /// </summary>
    /// <param name="key">The AES key to encrypt.</param>
    /// <param name="publicKeyPem">The RSA public key in PEM format.</param>
    /// <returns>The encrypted key as Base64 string.</returns>
    public string EncryptKeyToBase64(byte[] key, string publicKeyPem)
    {
        using var rsa = RSA.Create();
        rsa.ImportFromPem(publicKeyPem);
        return EncryptKeyToBase64(key, rsa);
    }

    /// <summary>
    /// Parses an RSA public key from PEM format.
    /// </summary>
    /// <param name="publicKeyPem">The public key in PEM format.</param>
    /// <returns>The RSA instance with the imported public key.</returns>
    public static RSA ParsePublicKey(string publicKeyPem)
    {
        if (string.IsNullOrEmpty(publicKeyPem))
            throw new ArgumentException("Public key PEM cannot be null or empty", nameof(publicKeyPem));

        try
        {
            var rsa = RSA.Create();
            rsa.ImportFromPem(publicKeyPem);
            return rsa;
        }
        catch (Exception ex)
        {
            throw new EncryptionException("Failed to parse RSA public key from PEM", ex);
        }
    }
}
