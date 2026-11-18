using System.Security.Cryptography;
using System.Text.Json;
using Cheqi.Sdk.Models.Generated;
using Cheqi.Sdk.Serialization;
using Cheqi.Sdk.Tests.TestSupport;
using Xunit;

namespace Cheqi.Sdk.Tests;

public sealed class CreditNoteReferenceTests
{
    private static readonly object Payload = new { documentNumber = "CN-123", originatorDocumentReference = "INV-123", taxesApplied = true, totalAmount = 12.123456789012345678m };
    [Fact]
    public async Task DownloadReferenceKeepsKeysEncryptedAndReplaysPersistedSubmission()
    {
        using var rsa = RSA.Create(2048);
        using var harness = new SdkHarness();
        harness.Enqueue(200, JsonSerializer.Serialize(new { deliveryRoute = "DOWNLOAD", parentCheqiReceiptId = "original",
            creditNotePublicKey = Convert.ToBase64String(rsa.ExportSubjectPublicKeyInfo()) }));
        var prepared = await harness.Sdk.CreditNoteService.PrepareCreditNoteAsync(new {
            taxesApplied = true, creditNoteTemplateRequest = new {
                documentNumber = "CN-123", originatorDocumentReference = "INV-123", totalAmount = 12.123456789012345678m
            }
        });
        var serialized = JsonSerializer.Serialize(prepared, CheqiJson.CreateOptions());
        Assert.DoesNotContain("INV-123", serialized);
        Assert.DoesNotContain("contentKey", serialized);
        Assert.DoesNotContain("privateKey", serialized);
        var restored = SdkHarness.Read<PreparedCreditNote>(serialized);
        var request = restored.DownloadSubmission!;
        var key = rsa.Decrypt(Convert.FromBase64String(request.EncryptedParentLinkKey!), RSAEncryptionPadding.OaepSHA256);
        using var link = JsonDocument.Parse(new Decryption.AesDecryptor().Decrypt(request.EncryptedParentLink!, key));
        Assert.Equal("original", link.RootElement.GetProperty("parentCheqiReceiptId").GetString());
        Assert.Equal(request.DownloadId, link.RootElement.GetProperty("downloadId").GetString());
        var note = new Download.DownloadService().DecryptDownloadEnvelope(request.Ciphertext,
            link.RootElement.GetProperty("contentKey").GetString()!);
        using var content = JsonDocument.Parse(note.Documents["CREDIT_NOTE_PAYLOAD"].Content);
        Assert.True(content.RootElement.GetProperty("taxesApplied").GetBoolean());
        Assert.Equal(12.123456789012345678m, content.RootElement.GetProperty("totalAmount").GetDecimal());
        harness.Enqueue(200, SdkHarness.DownloadAccepted);
        harness.Enqueue(200, SdkHarness.DownloadAccepted);
        Assert.True((await harness.Sdk.CreditNoteService.SubmitPreparedCreditNoteAsync(restored)).IsAccepted);
        await harness.Sdk.CreditNoteService.SubmitPreparedCreditNoteAsync(restored);
        Assert.Equal(new[] { "/credit-note/resolve", "/receipt/download", "/receipt/download" }, harness.Requests.Select(r => r.Path));
        Assert.Equal(harness.Requests[1].Body, harness.Requests[2].Body);
        using var reference = JsonDocument.Parse(harness.Requests[0].Body);
        Assert.Equal("7d4b8ece79c805d20d5451ad72f019580fe0dc893f7aa16eedd94992dbaebd32",
            reference.RootElement.GetProperty("documentReferenceHash").GetString());
        Assert.DoesNotContain(link.RootElement.GetProperty("contentKey").GetString()!, harness.Requests[1].Body);
        restored.ParentCheqiReceiptId = "other";
        await Assert.ThrowsAsync<ArgumentException>(() => harness.Sdk.CreditNoteService.SubmitPreparedCreditNoteAsync(restored));
    }
    [Fact]
    public async Task DigitalReferenceMatchesOriginalOwnerAndUsesOriginalStore()
    {
        using var rsa = RSA.Create(2048);
        using var harness = new SdkHarness();
        var store = Guid.NewGuid();
        harness.Enqueue(200, JsonSerializer.Serialize(new { deliveryRoute = "DIGITAL", parentCheqiReceiptId = "original", storeId = store }));
        harness.Enqueue(200, SdkHarness.Match(rsa: rsa));
        var prepared = await harness.Sdk.CreditNoteService.PrepareCreditNoteAsync(Payload);
        Assert.Equal(store, prepared.DeviceSubmission!.StoreId);
        Assert.Contains("original", harness.Requests[1].Body);
        using var content = JsonDocument.Parse(SdkHarness.Decrypt(prepared.DeviceSubmission.DeviceDeliveries[0], rsa));
        Assert.True(content.RootElement.GetProperty("taxesApplied").GetBoolean());
        Assert.Equal("INV-123", content.RootElement.GetProperty("creditNoteTemplateRequest").GetProperty("originatorDocumentReference").GetString());
        harness.Enqueue(202, SdkHarness.Submitted);
        Assert.True((await harness.Sdk.CreditNoteService.SubmitPreparedCreditNoteAsync(prepared)).IsAccepted);
    }
    [Theory]
    [InlineData(404)]
    [InlineData(409)]
    public async Task UnresolvedReferenceNeverCreatesFallback(int status)
    {
        using var harness = new SdkHarness();
        harness.Enqueue(status, "{\"message\":\"Original unavailable\"}");
        var error = await Assert.ThrowsAsync<Http.Exceptions.CheqiApiException>(() => harness.Sdk.CreditNoteService.PrepareCreditNoteAsync(Payload));
        Assert.Equal(status, error.HttpStatusCode);
        Assert.Single(harness.Requests);
    }
}
