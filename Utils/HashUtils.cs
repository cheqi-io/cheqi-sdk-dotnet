using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Security.Cryptography;
using System.Text;

namespace Cheqi.Sdk.Utils;

/// <summary>
/// Utility class for cryptographic hashing operations.
/// 
/// Provides static methods for generating SHA-256 hashes in hexadecimal format.
/// All methods are thread-safe and optimized for performance.
/// </summary>
public static class HashUtils
{
    private static ILogger? _logger;

    /// <summary>
    /// Sets the logger for HashUtils (optional).
    /// </summary>
    public static void SetLogger(ILogger logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Generates a SHA-256 hash of the input string.
    /// </summary>
    /// <param name="input">String to hash</param>
    /// <returns>Lowercase hexadecimal representation of the SHA-256 hash</returns>
    /// <exception cref="ArgumentNullException">if input is null</exception>
    public static string Sha256Hex(string input)
    {
        if (input == null)
        {
            throw new ArgumentNullException(nameof(input), "Input string cannot be null");
        }

        try
        {
            var inputBytes = Encoding.UTF8.GetBytes(input);
            var hashBytes = SHA256.HashData(inputBytes);
            string hash = BytesToHex(hashBytes);
            _logger?.LogDebug("SHA-256 hash generated: {HashPrefix}...", hash.Substring(0, Math.Min(8, hash.Length)));
            return hash;
        }
        catch (Exception e)
        {
            _logger?.LogError(e, "SHA-256 algorithm error");
            throw new InvalidOperationException("SHA-256 algorithm not available", e);
        }
    }

    /// <summary>
    /// Generates a SHA-256 hash of the input byte array.
    /// </summary>
    /// <param name="input">Byte array to hash</param>
    /// <returns>Lowercase hexadecimal representation of the SHA-256 hash</returns>
    /// <exception cref="ArgumentNullException">if input is null</exception>
    public static string Sha256Hex(byte[] input)
    {
        if (input == null)
        {
            throw new ArgumentNullException(nameof(input), "Input byte array cannot be null");
        }

        try
        {
            var hashBytes = SHA256.HashData(input);
            string hash = BytesToHex(hashBytes);
            _logger?.LogDebug("SHA-256 hash generated for {Length} bytes", input.Length);
            return hash;
        }
        catch (Exception e)
        {
            _logger?.LogError(e, "SHA-256 algorithm error");
            throw new InvalidOperationException("SHA-256 algorithm not available", e);
        }
    }

    /// <summary>
    /// Converts a byte array to a lowercase hexadecimal string.
    /// </summary>
    /// <param name="bytes">Byte array to convert</param>
    /// <returns>Lowercase hexadecimal string representation</returns>
    private static string BytesToHex(byte[] bytes)
    {
        var result = new StringBuilder(bytes.Length * 2);
        foreach (byte b in bytes)
        {
            result.Append($"{b:x2}");
        }
        return result.ToString();
    }
}
