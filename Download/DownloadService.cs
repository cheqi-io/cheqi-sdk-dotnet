using System.Security.Cryptography;
using System.Text.Json;
using System.Text.RegularExpressions;
using Cheqi.Sdk.Config;
using Cheqi.Sdk.Decryption;
using Cheqi.Sdk.Encryption;
using Cheqi.Sdk.Models.Generated;
using Cheqi.Sdk.Serialization;

namespace Cheqi.Sdk.Download;

/// <summary>Stateless helpers for encrypted browser downloads; keys never enter upload requests.</summary>
public sealed class DownloadService
{
    public const int DownloadIdLength = 16;
    public const int ContentKeyLength = 32;
    public const string ProductionBaseUrl = "https://receipt.cheqi.io";
    public const string SandboxBaseUrl = "https://sandbox.receipt.cheqi.io";
    private readonly JsonSerializerOptions _json = CheqiJson.CreateOptions();

    public DownloadLink GenerateDownloadLink(CheqiApiEnvironment environment)
        => GenerateDownloadLink(environment == CheqiApiEnvironment.Production ? ProductionBaseUrl : SandboxBaseUrl);

    public DownloadLink GenerateDownloadLink(string baseUrl)
    {
        ValidateBaseUrl(baseUrl);
        var id = Encode(RandomNumberGenerator.GetBytes(DownloadIdLength));
        var key = Encode(RandomNumberGenerator.GetBytes(ContentKeyLength));
        return new DownloadLink(id, key, BuildDownloadUrl(baseUrl, id, key));
    }

    public string BuildDownloadUrl(string baseUrl, string downloadId, string contentKey)
    {
        ValidateBaseUrl(baseUrl);
        ValidateId(downloadId);
        DecodeKey(contentKey);
        return $"{baseUrl.TrimEnd('/')}/{downloadId}#{contentKey}";
    }

    public DownloadLink ParseDownloadUrl(string url)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri) || (uri.Scheme != "http" && uri.Scheme != "https"))
            throw new ArgumentException("Invalid receipt download URL", nameof(url));
        var id = uri.AbsolutePath.Split('/').Last();
        var key = uri.Fragment.TrimStart('#');
        ValidateId(id);
        DecodeKey(key);
        return new DownloadLink(id, key, url);
    }

    /// <summary>Returns Base64(12-byte IV || ciphertext || 16-byte GCM tag).</summary>
    public string EncryptDownloadEnvelope(ReceiptEnvelope envelope, string contentKey)
    {
        ArgumentNullException.ThrowIfNull(envelope);
        var key = DecodeKey(contentKey);
        using var random = RandomNumberGenerator.Create();
        try { return new AesEncryptor(random).Encrypt(JsonSerializer.Serialize(envelope, _json), key).ToBase64String(); }
        finally { CryptographicOperations.ZeroMemory(key); }
    }

    public ReceiptEnvelope DecryptDownloadEnvelope(string ciphertext, string contentKey)
    {
        var key = DecodeKey(contentKey);
        try
        {
            return JsonSerializer.Deserialize<ReceiptEnvelope>(new AesDecryptor().Decrypt(ciphertext, key), _json)
                ?? throw new DecryptionException("Download envelope is null");
        }
        finally { CryptographicOperations.ZeroMemory(key); }
    }

    internal static void ValidateBaseUrl(string baseUrl)
    {
        if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out var uri) || (uri.Scheme != "https" && uri.Scheme != "http")
            || uri.Query.Length != 0 || uri.Fragment.Length != 0 || uri.UserInfo.Length != 0)
            throw new ArgumentException("Download base URL must be an absolute HTTP(S) URL without query, fragment or credentials", nameof(baseUrl));
    }
    private static string Encode(byte[] data) => Convert.ToBase64String(data).TrimEnd('=').Replace('+', '-').Replace('/', '_');
    private static void ValidateId(string id)
    {
        if (id == null || !Regex.IsMatch(id, "\\A[A-Za-z0-9_-]{22,64}\\z")) throw new ArgumentException("downloadId must contain 22-64 base64url characters", nameof(id));
    }
    private static byte[] DecodeKey(string key)
    {
        if (key == null || !Regex.IsMatch(key, "\\A[A-Za-z0-9_-]{43}\\z")) throw new ArgumentException("contentKey must be an unpadded base64url AES-256 key", nameof(key));
        return Convert.FromBase64String(key.Replace('-', '+').Replace('_', '/') + "=");
    }
}
