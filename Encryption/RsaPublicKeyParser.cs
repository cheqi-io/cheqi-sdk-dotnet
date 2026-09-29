using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Formats.Asn1;
using System.Numerics;
using System.Security.Cryptography;

namespace Cheqi.Sdk.Encryption;

/// <summary>
/// Parses RSA public keys from various formats.
/// Supports both X.509 SubjectPublicKeyInfo format and raw iOS format.
/// </summary>
public class RsaPublicKeyParser
{
    private readonly ILogger<RsaPublicKeyParser> _logger;

    public RsaPublicKeyParser(ILogger<RsaPublicKeyParser>? logger = null)
    {
        _logger = logger ?? NullLogger<RsaPublicKeyParser>.Instance;
    }

    /// <summary>
    /// Parses a base64-encoded RSA public key string.
    /// Handles both X.509 SubjectPublicKeyInfo format and raw RSA format (from iOS SecKeyCopyExternalRepresentation).
    /// </summary>
    public RSA Parse(string publicKeyBase64)
    {
        try
        {
            if (publicKeyBase64.Contains("-----BEGIN"))
            {
                var pemRsa = RSA.Create();
                try { pemRsa.ImportFromPem(publicKeyBase64); return pemRsa; }
                catch { pemRsa.Dispose(); throw; }
            }
            byte[] keyBytes = Convert.FromBase64String(publicKeyBase64);
            var rsa = RSA.Create();

            // First, try X.509 SubjectPublicKeyInfo format (standard format)
            try
            {
                rsa.ImportSubjectPublicKeyInfo(keyBytes, out _);
                _logger.LogDebug("Successfully parsed X.509 format RSA public key");
                return rsa;
            }
            catch (CryptographicException)
            {
                // If X.509 fails, try raw RSA format (from iOS SecKeyCopyExternalRepresentation)
                _logger.LogDebug("X.509 parsing failed, trying raw RSA format");
                rsa.Dispose();
                return ParseRawRsaPublicKey(keyBytes);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to parse RSA public key: {Message}", e.Message);
            throw new EncryptionException($"Failed to parse RSA public key: {e.Message}", e);
        }
    }

    /// <summary>
    /// Parses a raw RSA public key using ASN.1 parsing.
    /// This handles the format from iOS SecKeyCopyExternalRepresentation.
    /// </summary>
    private RSA ParseRawRsaPublicKey(byte[] keyBytes)
    {
        try
        {
            // Parse ASN.1 SEQUENCE manually
            // RSA public key format: SEQUENCE { modulus INTEGER, exponent INTEGER }
            var reader = new AsnReader(keyBytes, AsnEncodingRules.DER);
            var sequenceReader = reader.ReadSequence();

            // Read modulus and exponent as byte arrays directly
            byte[] modulusBytes = sequenceReader.ReadIntegerBytes().ToArray();
            byte[] exponentBytes = sequenceReader.ReadIntegerBytes().ToArray();

            // Ensure modulus and exponent are in the correct format (unsigned, big-endian)
            // Remove leading zero byte if present (ASN.1 adds it for positive numbers)
            if (modulusBytes.Length > 0 && modulusBytes[0] == 0)
            {
                byte[] temp = new byte[modulusBytes.Length - 1];
                Array.Copy(modulusBytes, 1, temp, 0, temp.Length);
                modulusBytes = temp;
            }

            if (exponentBytes.Length > 0 && exponentBytes[0] == 0)
            {
                byte[] temp = new byte[exponentBytes.Length - 1];
                Array.Copy(exponentBytes, 1, temp, 0, temp.Length);
                exponentBytes = temp;
            }

            // Create RSA public key parameters
            var rsaParameters = new RSAParameters
            {
                Modulus = modulusBytes,
                Exponent = exponentBytes
            };

            var rsa = RSA.Create();
            rsa.ImportParameters(rsaParameters);

            _logger.LogDebug("Successfully parsed raw RSA public key (iOS format)");
            return rsa;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to parse raw RSA public key: {Message}", e.Message);
            throw new EncryptionException($"Failed to parse raw RSA public key: {e.Message}", e);
        }
    }
}
