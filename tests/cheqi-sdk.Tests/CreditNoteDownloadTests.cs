using System.Text.Json;
using Cheqi.Sdk.Receipt;
using Cheqi.Sdk.Serialization;
using Cheqi.Sdk.Tests.TestSupport;
using Xunit;

namespace Cheqi.Sdk.Tests;

public sealed class CreditNoteDownloadTests
{
    [Fact]
    public async Task CreditNoteUsesOrdinaryDownloadAndKeepsReferenceEncrypted()
    {
        using var harness = new SdkHarness();
        var prepared = harness.Sdk.CreditNoteService.PrepareDownload(new {
            taxesApplied = false, creditNoteTemplateRequest = new {
                documentNumber = "CN-123", originatorDocumentReference = " INV-123 ", creditNoteSubtotal = 12.123456789012345678m
            }
        });
        Assert.Empty(harness.Requests);
        var restored = SdkHarness.Read<PreparedReceiptDownload>(JsonSerializer.Serialize(prepared, CheqiJson.CreateOptions()));
        var downloads = new Download.DownloadService();
        var link = downloads.ParseDownloadUrl(restored.DownloadUrl);
        var envelope = downloads.DecryptDownloadEnvelope(restored.Ciphertext, link.ContentKey);
        using var content = JsonDocument.Parse(envelope.Documents["CREDIT_NOTE_PAYLOAD"].Content);
        Assert.Equal(" INV-123 ", content.RootElement.GetProperty("originatorDocumentReference").GetString());
        Assert.False(content.RootElement.GetProperty("taxesApplied").GetBoolean());
        Assert.Equal(12.123456789012345678m, content.RootElement.GetProperty("creditNoteSubtotal").GetDecimal());
        harness.Enqueue(200, SdkHarness.DownloadAccepted);
        harness.Enqueue(200, SdkHarness.DownloadAccepted);
        await harness.Sdk.ReceiptService.UploadPreparedDownloadAsync(restored);
        await harness.Sdk.ReceiptService.UploadPreparedDownloadAsync(restored);
        Assert.All(harness.Requests, request => Assert.Equal("/receipt/download", request.Path));
        Assert.Equal(harness.Requests[0].Body, harness.Requests[1].Body);
        Assert.DoesNotContain("INV-123", harness.Requests[0].Body);
        Assert.DoesNotContain(link.ContentKey, harness.Requests[0].Body);
        Assert.DoesNotContain("parent", harness.Requests[0].Body);
        Assert.Throws<ArgumentException>(() => harness.Sdk.CreditNoteService.PrepareDownload(new { documentNumber = "CN", taxesApplied = true }));
    }
}
