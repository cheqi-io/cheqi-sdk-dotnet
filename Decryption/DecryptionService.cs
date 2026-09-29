using System.Security.Cryptography;
using System.Text.Json;
using Cheqi.Sdk.Models.Generated;
using Cheqi.Sdk.Serialization;
using Microsoft.Extensions.Logging;
using Initiation = Cheqi.Sdk.CreditNote.CreditNoteInitiationRequest;

namespace Cheqi.Sdk.Decryption;

/// <summary>Decrypts complete API and webhook envelopes using the recipient's private key.</summary>
public sealed class DecryptionService
{
    public DecryptionService(ILogger<DecryptionService>? logger = null) { }
    public ReceiptEnvelope DecryptReceipt(EncryptedReceiptDeliveryResponse delivery, string privateKey)
    {
        ArgumentNullException.ThrowIfNull(delivery);
        return DecryptEnvelope(delivery.EncryptedEnvelope, delivery.EncryptedEnvelopeKey, privateKey);
    }
    public ReceiptEnvelope DecryptReceipt(WebhookReceiptEnvelope delivery, string privateKey)
    {
        ArgumentNullException.ThrowIfNull(delivery);
        return DecryptEnvelope(delivery.EncryptedEnvelope, delivery.EncryptedEnvelopeKey, privateKey);
    }
    private ReceiptEnvelope DecryptEnvelope(string? encryptedContent, string? encryptedKey, string privateKey)
    {
        try
        {
            return JsonSerializer.Deserialize<ReceiptEnvelope>(Decrypt(encryptedContent, encryptedKey, privateKey), CheqiJson.CreateOptions())
                ?? throw new DecryptionException("Receipt envelope is null");
        }
        catch (DecryptionException) { throw; }
        catch (Exception exception) { throw new DecryptionException("Failed to decrypt receipt envelope", exception); }
    }

    public string DecryptCreditNoteInitiation(EncryptedCreditNoteInitiationRequest request, string privateKey)
    {
        ArgumentNullException.ThrowIfNull(request);
        return Decrypt(request.VarEncryptedCreditNoteInitiationRequest, request.EncryptedSymmetricKey, privateKey);
    }
    public string DecryptCreditNoteInitiation(EncryptedCreditNoteInitiationRequestResponse request, string privateKey)
    {
        ArgumentNullException.ThrowIfNull(request);
        return Decrypt(request.EncryptedCreditNoteInitiationRequest, request.EncryptedSymmetricKey, privateKey);
    }
    public string DecryptCreditNoteInitiation(WebhookCreditNoteInitiationRequest request, string privateKey)
    {
        ArgumentNullException.ThrowIfNull(request);
        return Decrypt(request.EncryptedCreditNoteInitiationRequest, request.EncryptedSymmetricKey, privateKey);
    }
    public Initiation DecryptCreditNoteInitiationRequest(EncryptedCreditNoteInitiationRequest request, string privateKey)
        => ParseInitiation(DecryptCreditNoteInitiation(request, privateKey));
    public Initiation DecryptCreditNoteInitiationRequest(EncryptedCreditNoteInitiationRequestResponse request, string privateKey)
        => ParseInitiation(DecryptCreditNoteInitiation(request, privateKey));
    public Initiation DecryptCreditNoteInitiationRequest(WebhookCreditNoteInitiationRequest request, string privateKey)
        => ParseInitiation(DecryptCreditNoteInitiation(request, privateKey));
    private static Initiation ParseInitiation(string plaintext)
    {
        try
        {
            var request = JsonSerializer.Deserialize<Initiation>(plaintext, CheqiJson.CreateOptions()) ?? throw new DecryptionException("Return request is null");
            request.Validate();
            return request;
        }
        catch (Exception exception) { throw new DecryptionException("Invalid credit-note initiation request", exception); }
    }
    private static string Decrypt(string? ciphertext, string? encryptedKey, string privateKey)
    {
        if (string.IsNullOrWhiteSpace(ciphertext) || string.IsNullOrWhiteSpace(encryptedKey))
            throw new ArgumentException("Encrypted content and encrypted AES key are required");
        var key = new RsaKeyDecryptor().DecryptKey(encryptedKey, privateKey);
        try { return new AesDecryptor().Decrypt(ciphertext, key); }
        finally { CryptographicOperations.ZeroMemory(key); }
    }
}
