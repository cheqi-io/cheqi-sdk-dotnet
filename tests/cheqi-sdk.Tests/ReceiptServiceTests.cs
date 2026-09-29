using System.Net;
using System.Security.Cryptography;
using System.Text.Json;
using Cheqi.Sdk.Http.Exceptions;
using Cheqi.Sdk.Models.Generated;
using Cheqi.Sdk.Receipt;
using Cheqi.Sdk.Serialization;
using Cheqi.Sdk.Tests.TestSupport;
using Xunit;

namespace Cheqi.Sdk.Tests;

public sealed class ReceiptServiceTests
{
    [Fact]
    public async Task DigitalEncryptsIdenticalDefinitivePayloadIndependentlyForEveryDevice()
    {
        using var rsa = RSA.Create(2048);
        using var harness = new SdkHarness();
        harness.Enqueue(200, SdkHarness.Match(rsa: rsa));
        harness.Enqueue(202, SdkHarness.Submitted);
        var payload = SdkHarness.Payload();
        var store = Guid.NewGuid();
        var result = await harness.Sdk.ReceiptService.IssueReceiptAsync(SdkHarness.Identification(), payload, store, "explicit-token");
        Assert.True(result.IsAccepted);
        Assert.Equal(ReceiptIssueState.DigitalSubmitted, result.State);
        Assert.Equal(new[] { "/recipient/resolve", "/receipt/encrypted" }, harness.Requests.Select(r => r.Path));
        Assert.All(harness.Requests, request => Assert.Equal("Bearer explicit-token", request.Authorization));
        var envelope = SdkHarness.Read<EncryptedReceiptEnvelope>(harness.Requests[1].Body);
        Assert.Equal(store, envelope.StoreId);
        Assert.Equal(2, envelope.DeviceDeliveries.Count);
        var expected = JsonSerializer.Serialize(payload, CheqiJson.CreateOptions());
        Assert.All(envelope.DeviceDeliveries, delivery => Assert.Equal(expected, SdkHarness.Decrypt(delivery, rsa)));
        Assert.NotEqual(envelope.DeviceDeliveries[0].EncryptedContent, envelope.DeviceDeliveries[1].EncryptedContent);
        Assert.DoesNotContain("POS-001", harness.Requests[1].Body);
        Assert.DoesNotContain("private-locator", harness.Requests[1].Body);
    }

    [Theory]
    [InlineData("DOWNLOAD_FALLBACK", ReceiptIssueState.DownloadFallback)]
    [InlineData("EMAIL_FALLBACK", ReceiptIssueState.EmailFallback)]
    public async Task ExplicitFallbackReturnsIntegrationAction(string route, ReceiptIssueState expected)
    {
        using var harness = new SdkHarness();
        harness.Enqueue(200, SdkHarness.Match(route));
        var result = await harness.Sdk.ReceiptService.IssueReceiptAsync(new IdentificationDetails { PairingCode = "pair" }, SdkHarness.Payload());
        Assert.Equal(expected, result.State);
        Assert.False(result.IsAccepted);
        Assert.Equal(route == "DOWNLOAD_FALLBACK", result.DownloadEnvelopeRequired);
        Assert.Equal(route == "EMAIL_FALLBACK", result.EmailReceiptRequired);
        Assert.Single(harness.Requests);
    }

    [Fact]
    public async Task DownloadFallbackPreservesPaymentContextAndUploadsOnlyCiphertext()
    {
        using var harness = new SdkHarness();
        harness.Enqueue(200, SdkHarness.Match("DOWNLOAD_FALLBACK"));
        harness.Enqueue(201, SdkHarness.DownloadAccepted);
        var result = await harness.Sdk.ReceiptService.IssueReceiptAsync(SdkHarness.Identification(), SdkHarness.Payload());
        Assert.True(result.IsAccepted);
        Assert.Equal("/receipt/download", harness.Requests[1].Path);
        var request = SdkHarness.Read<ClientReceiptDownloadRequest>(harness.Requests[1].Body);
        var link = harness.Sdk.DownloadService.ParseDownloadUrl(result.DownloadUrl!);
        Assert.DoesNotContain(link.ContentKey, harness.Requests[1].Body);
        Assert.DoesNotContain("private-locator", harness.Requests[1].Body);
        var envelope = harness.Sdk.DownloadService.DecryptDownloadEnvelope(request.Ciphertext, link.ContentKey);
        Assert.Equal(2, envelope.EnvelopeVersion);
        using var document = JsonDocument.Parse(envelope.Documents["RECEIPT_PAYLOAD"].Content);
        Assert.False(document.RootElement.TryGetProperty("identificationDetails", out _));
        using var identification = JsonDocument.Parse(envelope.Documents["IDENTIFICATION_DETAILS"].Content);
        Assert.Equal("private-locator", identification.RootElement.GetProperty("pairingCode").GetString());
        Assert.Equal("CARD_PAYMENT", identification.RootElement.GetProperty("paymentType").GetString());
    }

    [Theory]
    [InlineData(503, "{}", null)]
    [InlineData(202, "{\"state\":\"IN_PROGRESS\",\"retryAfterSeconds\":2}", 2)]
    [InlineData(202, "invalid-json", null)]
    [InlineData(202, "{}", null)]
    [InlineData(202, "[]", null)]
    [InlineData(202, "null", null)]
    public async Task AmbiguousDigitalSubmissionPreparesFallbackWithoutRetryOrRematch(int status, string body, int? retryAfter)
    {
        using var rsa = RSA.Create(2048);
        using var harness = new SdkHarness(retries: true);
        harness.Enqueue(200, SdkHarness.Match(rsa: rsa));
        harness.Enqueue(status, body);
        var result = await harness.Sdk.ReceiptService.IssueReceiptAsync(SdkHarness.Identification(), SdkHarness.Payload());
        Assert.Equal(ReceiptIssueState.DigitalPending, result.State);
        Assert.False(result.IsAccepted);
        Assert.True(result.Retryable);
        Assert.Equal("match-original", result.MatchId);
        Assert.Equal(retryAfter, result.RetryAfterSeconds);
        Assert.Equal(2, harness.Requests.Count);
        Assert.NotNull(result.PreparedDownload);
        var link = harness.Sdk.DownloadService.ParseDownloadUrl(result.DownloadUrl!);
        var envelope = harness.Sdk.DownloadService.DecryptDownloadEnvelope(result.PreparedDownload!.Ciphertext, link.ContentKey);
        Assert.DoesNotContain("private-locator", envelope.Documents["RECEIPT_PAYLOAD"].Content);
        Assert.DoesNotContain("identificationDetails", envelope.Documents["RECEIPT_PAYLOAD"].Content);
    }

    [Fact]
    public async Task MatchingNetworkFailureCreatesPersistableFallbackAndCanUploadLater()
    {
        using var harness = new SdkHarness();
        harness.Fail(new HttpRequestException("offline"));
        var result = await harness.Sdk.ReceiptService.IssueReceiptAsync(SdkHarness.Identification(), SdkHarness.Payload());
        Assert.Equal(ReceiptIssueState.DownloadFallback, result.State);
        var serialized = JsonSerializer.Serialize(result.PreparedDownload, CheqiJson.CreateOptions());
        var restored = SdkHarness.Read<PreparedReceiptDownload>(serialized);
        Assert.Equal(result.DownloadUrl, restored.DownloadUrl);
        var originalCiphertext = restored.Ciphertext;
        restored.UploadRequest.Ciphertext = "changed";
        Assert.Equal(originalCiphertext, restored.Ciphertext);
        harness.Enqueue(201, SdkHarness.DownloadAccepted);
        await harness.Sdk.ReceiptService.UploadPreparedDownloadAsync(restored);
        Assert.Equal(originalCiphertext, SdkHarness.Read<ClientReceiptDownloadRequest>(harness.Requests[1].Body).Ciphertext);
        Assert.DoesNotContain("private-locator", serialized);
        Assert.DoesNotContain(restored.DownloadUrl, restored.ToString());
    }

    [Theory]
    [InlineData("IN_PROGRESS", ReceiptIssueState.DigitalPending)]
    [InlineData("EXPIRED", ReceiptIssueState.DigitalExpired)]
    [InlineData("SUBMITTED", ReceiptIssueState.DigitalSubmitted)]
    public async Task ResumeUsesOriginalMatchAndNeverRematches(string state, ReceiptIssueState expected)
    {
        using var harness = new SdkHarness();
        harness.Enqueue(200, $"{{\"matchId\":\"match-original\",\"state\":\"{state}\",\"retryAfterSeconds\":2,\"submission\":{SdkHarness.Submitted}}}");
        var result = await harness.Sdk.ReceiptService.ResumeReceiptAsync("match-original", SdkHarness.Payload());
        Assert.Equal(expected, result.State);
        Assert.Null(result.PreparedDownload);
        Assert.Equal("/recipient/matches/match-original", harness.Requests.Single().Path);
        Assert.Equal(state == "SUBMITTED", result.IsAccepted);
    }

    [Fact]
    public async Task ResumeMatchedEncryptsForOriginalDeviceSnapshot()
    {
        using var rsa = RSA.Create(2048);
        using var harness = new SdkHarness();
        using var match = JsonDocument.Parse(SdkHarness.Match(rsa: rsa));
        harness.Enqueue(200, $"{{\"matchId\":\"match-original\",\"state\":\"MATCHED\",\"route\":\"DIGITAL\",\"recipients\":{match.RootElement.GetProperty("recipients").GetRawText()}}}");
        harness.Enqueue(202, SdkHarness.Submitted);
        var result = await harness.Sdk.ReceiptService.ResumeReceiptAsync("match-original", SdkHarness.Payload());
        Assert.True(result.IsAccepted);
        Assert.Equal(new[] { "/recipient/matches/match-original", "/receipt/encrypted" }, harness.Requests.Select(r => r.Path));
    }

    [Fact]
    public async Task ExpiredSubmissionDoesNotPrepareNewFallback()
    {
        using var rsa = RSA.Create(2048);
        using var harness = new SdkHarness();
        harness.Enqueue(200, SdkHarness.Match(rsa: rsa));
        harness.Enqueue(410, "{}");
        var result = await harness.Sdk.ReceiptService.IssueReceiptAsync(SdkHarness.Identification(), SdkHarness.Payload());
        Assert.Equal(ReceiptIssueState.DigitalExpired, result.State);
        Assert.Null(result.PreparedDownload);
        Assert.False(result.Retryable);
    }

    [Fact]
    public async Task ExplicitDownloadDoesNotMatchAndCanBeDeferredAfterNetworkFailure()
    {
        using var harness = new SdkHarness();
        harness.Fail(new HttpRequestException("offline"));
        var result = await harness.Sdk.ReceiptService.IssueDownloadReceiptAsync(SdkHarness.Identification(), SdkHarness.Payload());
        Assert.False(result.IsAccepted);
        Assert.NotNull(result.PreparedDownload);
        Assert.Equal("/receipt/download", harness.Requests.Single().Path);
    }

    [Fact]
    public async Task CancellationPropagatesWithoutFallback()
    {
        using var harness = new SdkHarness();
        using var cts = new CancellationTokenSource();
        harness.Enqueue((_, ct) => { cts.Cancel(); return Task.FromCanceled<HttpResponseMessage>(ct); });
        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => harness.Sdk.ReceiptService.IssueReceiptAsync(SdkHarness.Identification(), SdkHarness.Payload(), cancellationToken: cts.Token));
        Assert.Single(harness.Requests);
    }

    [Theory]
    [InlineData(400)]
    [InlineData(401)]
    [InlineData(403)]
    public async Task PermanentFailurePreservesApiMetadataAndDoesNotBecomeFallback(int status)
    {
        using var harness = new SdkHarness();
        harness.Enqueue(status, "{\"message\":\"denied\"}");
        var exception = await Assert.ThrowsAsync<CheqiApiException>(() => harness.Sdk.ReceiptService.IssueReceiptAsync(SdkHarness.Identification(), SdkHarness.Payload()));
        Assert.Equal(status, exception.HttpStatusCode);
        Assert.Equal("correlation-test", exception.CorrelationId);
        Assert.Single(harness.Requests);
    }
    [Fact]
    public async Task CallerSuppliedEnvelopeCanCompleteDownloadFallback()
    {
        using var harness = new SdkHarness();
        harness.Enqueue(200, SdkHarness.Match("DOWNLOAD_FALLBACK"));
        var result = await harness.Sdk.ReceiptService.IssueReceiptAsync(new IdentificationDetails { PairingCode = "pair" }, SdkHarness.Payload());
        var final = new ReceiptEnvelope("local-receipt", new() { ["CHEQI"] = new("{}", ReceiptEnvelopeDocument.MediaTypeEnum.ApplicationJson) }, 1, "local-generator", Guid.NewGuid());
        harness.Enqueue(201, SdkHarness.DownloadAccepted);
        var completed = await harness.Sdk.ReceiptService.CompleteDownloadFallbackAsync(result, final, templateHash: "caller-hash");
        Assert.True(completed.IsAccepted);
        var request = SdkHarness.Read<ClientReceiptDownloadRequest>(harness.Requests[1].Body);
        Assert.Equal("caller-hash", request.TemplateHash);
        var key = harness.Sdk.DownloadService.ParseDownloadUrl(completed.DownloadUrl!).ContentKey;
        Assert.Equal(final.ReceiptUuid, harness.Sdk.DownloadService.DecryptDownloadEnvelope(request.Ciphertext, key).ReceiptUuid);
    }

    [Theory]
    [InlineData(503, ReceiptIssueState.DigitalPending)]
    [InlineData(410, ReceiptIssueState.DigitalExpired)]
    public async Task RecoveryHandlesTransientFailureOrExpiryWithoutMakingNewCredentials(int status, ReceiptIssueState expected)
    {
        using var harness = new SdkHarness();
        harness.Enqueue(status, "{}");
        var result = await harness.Sdk.ReceiptService.ResumeReceiptAsync("match-original", SdkHarness.Payload());
        Assert.Equal(expected, result.State);
        Assert.Null(result.PreparedDownload);
        Assert.Null(result.DownloadUrl);
        Assert.Single(harness.Requests);
    }

    [Fact]
    public async Task InvalidPayloadAndTokenAreRejectedBeforeAnyHttp()
    {
        using var harness = new SdkHarness();
        var payload = SdkHarness.Payload();
        payload.Products.Clear();
        await Assert.ThrowsAsync<ArgumentException>(() => harness.Sdk.ReceiptService.IssueReceiptAsync(SdkHarness.Identification(), payload));
        await Assert.ThrowsAsync<ArgumentException>(() => harness.Sdk.ReceiptService.ResumeReceiptAsync(" ", SdkHarness.Payload()));
        await Assert.ThrowsAsync<ArgumentException>(() => harness.Sdk.ReceiptService.IssueReceiptAsync(SdkHarness.Identification(), SdkHarness.Payload(), accessToken: " "));
        Assert.Empty(harness.Requests);
    }
}
