namespace Cheqi.Sdk.Config;

/// <summary>
/// Configuration for encryption algorithms used in the SDK.
/// </summary>
public sealed class EncryptionConfig
{
    /// <summary>
    /// AES key size in bits (256 for AES-256).
    /// </summary>
    public int AesKeySize { get; init; } = 256;

    /// <summary>
    /// AES-GCM initialization vector (nonce) size in bytes.
    /// </summary>
    public int AesIvSize { get; init; } = 12;

    /// <summary>
    /// AES-GCM authentication tag size in bits.
    /// </summary>
    public int AesTagSize { get; init; } = 128;

    /// <summary>
    /// RSA key encryption algorithm (RSA-OAEP with SHA-256).
    /// </summary>
    public string RsaAlgorithm { get; init; } = "RSA-OAEP-256";

    /// <summary>
    /// Default encryption configuration.
    /// </summary>
    public static EncryptionConfig Default => new();
}
