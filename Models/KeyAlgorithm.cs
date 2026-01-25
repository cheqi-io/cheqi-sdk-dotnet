namespace Cheqi.Sdk.Models;

/// <summary>
/// Supported key algorithms for encryption and signing.
/// </summary>
public enum KeyAlgorithm
{
    /// <summary>
    /// RSA 2048-bit encryption with OAEP padding.
    /// </summary>
    RSA_2048,

    /// <summary>
    /// RSA 4096-bit encryption with OAEP padding.
    /// </summary>
    RSA_4096,

    /// <summary>
    /// Elliptic Curve P-256 (secp256r1).
    /// </summary>
    ECDSA_P256,

    /// <summary>
    /// Elliptic Curve P-384 (secp384r1).
    /// </summary>
    ECDSA_P384,

    /// <summary>
    /// Edwards-curve Digital Signature Algorithm.
    /// </summary>
    ED25519
}

/// <summary>
/// Extension methods for KeyAlgorithm enum.
/// </summary>
public static class KeyAlgorithmExtensions
{
    /// <summary>
    /// Gets the string value representation of the key algorithm.
    /// </summary>
    public static string GetValue(this KeyAlgorithm algorithm)
    {
        return algorithm switch
        {
            KeyAlgorithm.RSA_2048 => "RSA-2048",
            KeyAlgorithm.RSA_4096 => "RSA-4096",
            KeyAlgorithm.ECDSA_P256 => "ECDSA-P256",
            KeyAlgorithm.ECDSA_P384 => "ECDSA-P384",
            KeyAlgorithm.ED25519 => "Ed25519",
            _ => throw new ArgumentOutOfRangeException(nameof(algorithm), algorithm, null)
        };
    }

    /// <summary>
    /// Gets the description of the key algorithm.
    /// </summary>
    public static string GetDescription(this KeyAlgorithm algorithm)
    {
        return algorithm switch
        {
            KeyAlgorithm.RSA_2048 => "RSA 2048-bit encryption with OAEP padding",
            KeyAlgorithm.RSA_4096 => "RSA 4096-bit encryption with OAEP padding",
            KeyAlgorithm.ECDSA_P256 => "Elliptic Curve P-256 (secp256r1)",
            KeyAlgorithm.ECDSA_P384 => "Elliptic Curve P-384 (secp384r1)",
            KeyAlgorithm.ED25519 => "Edwards-curve Digital Signature Algorithm",
            _ => throw new ArgumentOutOfRangeException(nameof(algorithm), algorithm, null)
        };
    }

    /// <summary>
    /// Parses a string value to KeyAlgorithm enum.
    /// </summary>
    public static KeyAlgorithm FromValue(string value)
    {
        return value switch
        {
            "RSA-2048" => KeyAlgorithm.RSA_2048,
            "RSA-4096" => KeyAlgorithm.RSA_4096,
            "ECDSA-P256" => KeyAlgorithm.ECDSA_P256,
            "ECDSA-P384" => KeyAlgorithm.ECDSA_P384,
            "Ed25519" => KeyAlgorithm.ED25519,
            _ => throw new ArgumentException($"Unknown key algorithm: {value}", nameof(value))
        };
    }
}
