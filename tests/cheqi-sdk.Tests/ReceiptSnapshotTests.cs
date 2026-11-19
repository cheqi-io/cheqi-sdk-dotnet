using System.Net;
using System.Security.Cryptography;
using System.Text.Json;
using Cheqi.Sdk.Models.Generated;
using Cheqi.Sdk.Tests.TestSupport;
using Xunit;

namespace Cheqi.Sdk.Tests;

public sealed class ReceiptSnapshotTests
{
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task MatchingOrRecoveryMutationDoesNotChangeValidatedSnapshot(bool resume)
    {
        using var rsa = RSA.Create(2048);
        using var harness = new SdkHarness();
        var payload = SdkHarness.Payload();
        harness.Enqueue((_, _) =>
        {
            payload.DocumentNumber = "CHANGED-AFTER-VALIDATION";
            var match = System.Text.Json.Nodes.JsonNode.Parse(SdkHarness.Match(rsa: rsa))!;
            match["state"] = "MATCHED";
            match["route"] = "DIGITAL";
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
                { Content = new StringContent(match.ToJsonString()) });
        });
        harness.Enqueue(202, SdkHarness.Submitted);
        if (resume)
            await harness.Sdk.ReceiptService.ResumeReceiptAsync("match-original", payload);
        else
            await harness.Sdk.ReceiptService.IssueReceiptAsync(SdkHarness.Identification(), payload);
        var envelope = SdkHarness.Read<EncryptedReceiptEnvelope>(harness.Requests[1].Body);
        using var json = JsonDocument.Parse(SdkHarness.Decrypt(envelope.DeviceDeliveries[0], rsa));
        Assert.Equal("POS-001", json.RootElement.GetProperty("documentNumber").GetString());
    }

    [Fact]
    public async Task FallbackShouldContainSameSnapshotAsSubmittedCiphertext()
    {
        using var rsa = RSA.Create(2048);
        using var harness = new SdkHarness();
        var payload = SdkHarness.Payload();
        harness.Enqueue(200, SdkHarness.Match(rsa: rsa));
        harness.Enqueue((_, _) =>
        {
            payload.DocumentNumber = "CHANGED-DURING-SUBMISSION";
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
                { Content = new StringContent("{}") });
        });
        var result = await harness.Sdk.ReceiptService.IssueReceiptAsync(SdkHarness.Identification(), payload);
        var encrypted = SdkHarness.Read<EncryptedReceiptEnvelope>(harness.Requests[1].Body);
        var submitted = SdkHarness.Decrypt(encrypted.DeviceDeliveries[0], rsa);
        var prepared = result.PreparedDownload!;
        var link = harness.Sdk.DownloadService.ParseDownloadUrl(prepared.DownloadUrl);
        var fallback = harness.Sdk.DownloadService.DecryptDownloadEnvelope(prepared.UploadRequest.Ciphertext, link.ContentKey);
        Assert.Equal(submitted, fallback.Documents["RECEIPT_PAYLOAD"].Content);
    }

    [Fact]
    public async Task ReceiptAmountUsesJavaDecimalStringRepresentation()
    {
        using var rsa = RSA.Create(2048);
        using var harness = new SdkHarness();
        harness.Enqueue(200, SdkHarness.Match(rsa: rsa));
        harness.Enqueue(202, SdkHarness.Submitted);
        await harness.Sdk.ReceiptService.IssueReceiptAsync(SdkHarness.Identification(), SdkHarness.Payload());
        var envelope = SdkHarness.Read<EncryptedReceiptEnvelope>(harness.Requests[1].Body);
        using var json = JsonDocument.Parse(SdkHarness.Decrypt(envelope.DeviceDeliveries[0], rsa));
        Assert.Equal(JsonValueKind.String, json.RootElement.GetProperty("totalAmount").ValueKind);
    }
}
