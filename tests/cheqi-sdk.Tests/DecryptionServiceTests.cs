using System.Security.Cryptography;
using System.Text.Json;
using Cheqi.Sdk.Decryption;
using Cheqi.Sdk.Models.Generated;
using Cheqi.Sdk.Serialization;
using Cheqi.Sdk.Tests.TestSupport;
using Xunit;
using ReturnRequest = Cheqi.Sdk.CreditNote.CreditNoteInitiationRequest;
using ReturnItem = Cheqi.Sdk.CreditNote.ReturnLineItem;
using Refund = Cheqi.Sdk.CreditNote.RefundPreference;
using Reason = Cheqi.Sdk.CreditNote.ReturnReasonCode;

namespace Cheqi.Sdk.Tests;

public sealed class DecryptionServiceTests
{
    [Fact]
    public void ApiAndWebhookReceiptEnvelopesDecryptWithPemAndBase64Keys()
    {
        using var rsa = RSA.Create(2048);
        using var harness = new SdkHarness();
        var envelope = new ReceiptEnvelope("receipt", new() { ["CHEQI"] = new("{}", ReceiptEnvelopeDocument.MediaTypeEnum.ApplicationJson) }, 1, "v1", Guid.NewGuid());
        var delivery = harness.Sdk.EncryptionService.EncryptReceiptForRecipient(JsonSerializer.Serialize(envelope, CheqiJson.CreateOptions()), new MatchedRecipient { Id = "device", PublicKey = Convert.ToBase64String(rsa.ExportSubjectPublicKeyInfo()) });
        var api = new EncryptedReceiptDeliveryResponse { EncryptedEnvelope = delivery.EncryptedContent, EncryptedEnvelopeKey = delivery.EncryptedAesKey };
        var webhook = new WebhookReceiptEnvelope { EncryptedEnvelope = delivery.EncryptedContent, EncryptedEnvelopeKey = delivery.EncryptedAesKey };
        Assert.Equal(envelope.ReceiptUuid, harness.Sdk.DecryptionService.DecryptReceipt(api, SdkHarness.Pem("PRIVATE KEY", rsa.ExportPkcs8PrivateKey())).ReceiptUuid);
        Assert.Equal(envelope.ReceiptUuid, harness.Sdk.DecryptionService.DecryptReceipt(webhook, Convert.ToBase64String(rsa.ExportPkcs8PrivateKey())).ReceiptUuid);
    }

    [Fact]
    public void ReturnRequestDecryptsAndValidatesCurrentContract()
    {
        using var rsa = RSA.Create(2048);
        using var harness = new SdkHarness();
        var request = ValidReturn();
        var delivery = harness.Sdk.EncryptionService.EncryptCreditNoteForRecipient(JsonSerializer.Serialize(request, CheqiJson.CreateOptions()), new MatchedRecipient { Id = "device", PublicKey = SdkHarness.Pem("RSA PUBLIC KEY", rsa.ExportRSAPublicKey()) });
        var api = new EncryptedCreditNoteInitiationRequest("receipt", SdkHarness.Pem("PUBLIC KEY", rsa.ExportSubjectPublicKeyInfo()), delivery.EncryptedContent, delivery.EncryptedAesKey);
        var decrypted = harness.Sdk.DecryptionService.DecryptCreditNoteInitiationRequest(api, SdkHarness.Pem("PRIVATE KEY", rsa.ExportPkcs8PrivateKey()));
        Assert.Equal("Coffee", decrypted.LineItems![0].ProductId);
        Assert.Equal(Reason.DEFECTIVE, decrypted.LineItems[0].ReasonCode);
        request.LineItems![0].Quantity = 0;
        Assert.Throws<ArgumentException>(request.Validate);
    }

    [Fact]
    public void BankRefundRequiresAccountAndOtherMethodsRejectAccount()
    {
        var request = ValidReturn();
        request.RefundPreference = Refund.BANK_TRANSFER;
        Assert.Throws<ArgumentException>(request.Validate);
        request.RefundBankAccount = new() { Iban = "NL91ABNA0417164300", AccountHolder = "Customer" };
        request.Validate();
        request.RefundPreference = Refund.STORE_CREDIT;
        Assert.Throws<ArgumentException>(request.Validate);
    }

    private static ReturnRequest ValidReturn() => new()
    {
        CheqiReceiptId = "receipt", ReceiptId = "POS-001", CustomerNote = "Return", RefundPreference = Refund.ORIGINAL_PAYMENT_METHOD,
        LineItems = new() { new ReturnItem { ProductId = "Coffee", Quantity = 1, ReasonCode = Reason.DEFECTIVE } }
    };
}
