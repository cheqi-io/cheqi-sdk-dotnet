using System.Security.Cryptography;
using Cheqi.Sdk.Models.Generated;
using Microsoft.Extensions.Logging;

namespace Cheqi.Sdk.Encryption;

/// <summary>Independent AES-256-GCM and RSA-OAEP-SHA256 encryption for every owner device.</summary>
public sealed class EncryptionService : IDisposable
{
    private readonly RandomNumberGenerator _random = RandomNumberGenerator.Create();
    public EncryptionService(ILogger<EncryptionService>? logger = null) { }

    public EncryptedReceiptPayload EncryptForDevice(string plaintext, MatchedRecipient recipient)
    {
        if (string.IsNullOrWhiteSpace(plaintext)) throw new ArgumentException("plaintext is required", nameof(plaintext));
        ArgumentNullException.ThrowIfNull(recipient);
        if (string.IsNullOrWhiteSpace(recipient.Id) || string.IsNullOrWhiteSpace(recipient.PublicKey))
            throw new EncryptionException("Recipient must have an id and RSA public key");
        var key = RandomNumberGenerator.GetBytes(32);
        try
        {
            return new EncryptedReceiptPayload(recipient.Id,
                new AesEncryptor(_random).Encrypt(plaintext, key).ToBase64String(),
                new RsaKeyEncryptor().EncryptKey(key, recipient.PublicKey));
        }
        finally { CryptographicOperations.ZeroMemory(key); }
    }

    public EncryptedReceiptPayload EncryptReceiptForRecipient(string plaintext, MatchedRecipient recipient)
        => EncryptForDevice(plaintext, recipient);

    public List<EncryptedReceiptPayload> EncryptReceiptForRecipients(string plaintext, IEnumerable<MatchedRecipient> recipients)
    {
        ArgumentNullException.ThrowIfNull(recipients);
        var devices = recipients.ToList();
        if (devices.Count == 0) throw new EncryptionException("Recipients cannot be empty");
        return devices.Select(device => EncryptReceiptForRecipient(plaintext, device)).ToList();
    }

    public EncryptedReceiptPayload EncryptCreditNoteForRecipient(string plaintext, MatchedRecipient recipient)
        => EncryptReceiptForRecipient(plaintext, recipient);
    public void Dispose() => _random.Dispose();
}
