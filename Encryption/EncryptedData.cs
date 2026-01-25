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

    /// <summary>
    /// Converts the encrypted data to a Base64 string in the format: IV + ciphertext + tag.
    /// This matches the Java SDK format for encrypted data.
    /// </summary>
    public string ToBase64String()
    {
        // Combine IV + ciphertext + tag into a single byte array
        byte[] combined = new byte[Iv.Length + Ciphertext.Length + AuthTag.Length];
        Array.Copy(Iv, 0, combined, 0, Iv.Length);
        Array.Copy(Ciphertext, 0, combined, Iv.Length, Ciphertext.Length);
        Array.Copy(AuthTag, 0, combined, Iv.Length + Ciphertext.Length, AuthTag.Length);
        
        return Convert.ToBase64String(combined);
    }
}
