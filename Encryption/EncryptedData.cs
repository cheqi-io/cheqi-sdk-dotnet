namespace Cheqi.Sdk.Encryption;

/// <summary>
/// Container for AES-GCM encrypted data.
/// </summary>
public sealed class EncryptedData
{
    /// <summary>
    /// The encrypted ciphertext.
    /// </summary>
    public byte[] Ciphertext { get; }

    /// <summary>
    /// The initialization vector (nonce).
    /// </summary>
    public byte[] Iv { get; }

    /// <summary>
    /// The authentication tag.
    /// </summary>
    public byte[] AuthTag { get; }

    public EncryptedData(byte[] ciphertext, byte[] iv, byte[] authTag)
    {
        Ciphertext = ciphertext ?? throw new ArgumentNullException(nameof(ciphertext));
        Iv = iv ?? throw new ArgumentNullException(nameof(iv));
        AuthTag = authTag ?? throw new ArgumentNullException(nameof(authTag));
    }

    /// <summary>
    /// Gets the ciphertext as a Base64 string.
    /// </summary>
    public string CiphertextBase64 => Convert.ToBase64String(Ciphertext);

    /// <summary>
    /// Gets the IV as a Base64 string.
    /// </summary>
    public string IvBase64 => Convert.ToBase64String(Iv);

    /// <summary>
    /// Gets the authentication tag as a Base64 string.
    /// </summary>
    public string AuthTagBase64 => Convert.ToBase64String(AuthTag);
}
