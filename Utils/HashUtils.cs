using System.Security.Cryptography;
using System.Text;

namespace Cheqi.Sdk.Utils;

/// <summary>
/// Utility methods for cryptographic hashing.
/// </summary>
public static class HashUtils
{
    /// <summary>
    /// Computes the SHA-256 hash of a string and returns it as a hex string.
    /// </summary>
    /// <param name="input">The input string to hash.</param>
    /// <returns>The SHA-256 hash as a lowercase hex string.</returns>
    public static string ComputeSha256Hash(string input)
    {
        if (input == null)
            throw new ArgumentNullException(nameof(input));

        var inputBytes = Encoding.UTF8.GetBytes(input);
        return ComputeSha256Hash(inputBytes);
    }

    /// <summary>
    /// Computes the SHA-256 hash of a byte array and returns it as a hex string.
    /// </summary>
    /// <param name="input">The input bytes to hash.</param>
    /// <returns>The SHA-256 hash as a lowercase hex string.</returns>
    public static string ComputeSha256Hash(byte[] input)
    {
        if (input == null)
            throw new ArgumentNullException(nameof(input));

        var hashBytes = SHA256.HashData(input);
        return Convert.ToHexString(hashBytes).ToLowerInvariant();
    }

    /// <summary>
    /// Computes the SHA-256 hash of a string and returns it as Base64.
    /// </summary>
    /// <param name="input">The input string to hash.</param>
    /// <returns>The SHA-256 hash as a Base64 string.</returns>
    public static string ComputeSha256HashBase64(string input)
    {
        if (input == null)
            throw new ArgumentNullException(nameof(input));

        var inputBytes = Encoding.UTF8.GetBytes(input);
        var hashBytes = SHA256.HashData(inputBytes);
        return Convert.ToBase64String(hashBytes);
    }

    /// <summary>
    /// Computes the HMAC-SHA256 of a message with a key.
    /// </summary>
    /// <param name="message">The message to authenticate.</param>
    /// <param name="key">The secret key.</param>
    /// <returns>The HMAC-SHA256 as a lowercase hex string.</returns>
    public static string ComputeHmacSha256(string message, byte[] key)
    {
        if (message == null)
            throw new ArgumentNullException(nameof(message));
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        var messageBytes = Encoding.UTF8.GetBytes(message);
        var hashBytes = HMACSHA256.HashData(key, messageBytes);
        return Convert.ToHexString(hashBytes).ToLowerInvariant();
    }
}
