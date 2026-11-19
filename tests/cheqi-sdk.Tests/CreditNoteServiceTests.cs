using System.Security.Cryptography;
using Cheqi.Sdk.CreditNote;
using Cheqi.Sdk.Models.Generated;
using Cheqi.Sdk.Tests.TestSupport;
using Xunit;

namespace Cheqi.Sdk.Tests;

public sealed class CreditNoteServiceTests
{
    [Fact]
    public async Task AlreadyEncryptedCreditNoteCanBeSubmittedWithoutRematching()
    {
        using var harness = new SdkHarness();
        harness.Enqueue(202, SdkHarness.Submitted);
        var envelope = new EncryptedCreditNoteEnvelope("match-original", "parent-1", new());
        var result = await harness.Sdk.CreditNoteService.SubmitEncryptedCreditNoteAsync(envelope,
            Http.RequestOptions.WithAccessToken("explicit-token"));
        Assert.Equal("receipt-1", result.CheqiReceiptId);
        var request = Assert.Single(harness.Requests);
        Assert.Equal("/credit-note/encrypted", request.Path);
        Assert.Equal("Bearer explicit-token", request.Authorization);
        Assert.Equal("parent-1", SdkHarness.Read<EncryptedCreditNoteEnvelope>(request.Body).ParentCheqiReceiptId);
    }
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task DefinitiveCreditNoteUsesSeparateEndpointAndParentReference(bool asString)
    {
        using var rsa = RSA.Create(2048);
        using var harness = new SdkHarness();
        harness.Enqueue(200, SdkHarness.Match(rsa: rsa));
        harness.Enqueue(202, SdkHarness.Submitted);
        object payload = asString ? "{\"totalAmount\":-12.1}" : new { totalAmount = -12.1m };
        var result = await harness.Sdk.CreditNoteService.IssueCreditNoteAsync(SdkHarness.Identification(), "parent-1", payload);
        Assert.True(result.IsAccepted);
        Assert.Equal("parent-1", result.ParentCheqiReceiptId);
        Assert.Equal("/credit-note/encrypted", harness.Requests[1].Path);
        var envelope = SdkHarness.Read<EncryptedCreditNoteEnvelope>(harness.Requests[1].Body);
        Assert.Equal("parent-1", envelope.ParentCheqiReceiptId);
        Assert.All(envelope.DeviceDeliveries, device => Assert.Equal("{\"totalAmount\":-12.1}", SdkHarness.Decrypt(device, rsa)));
        Assert.DoesNotContain("totalAmount", harness.Requests[1].Body);
    }

    [Fact]
    public async Task CreditNoteWithoutDevicesIsRejected()
    {
        using var harness = new SdkHarness();
        harness.Enqueue(200, SdkHarness.Match("DOWNLOAD_FALLBACK"));
        await Assert.ThrowsAsync<ArgumentException>(() => harness.Sdk.CreditNoteService.IssueCreditNoteAsync(SdkHarness.Identification(), "parent", new { totalAmount = -1m }));
        Assert.Single(harness.Requests);
    }
}
