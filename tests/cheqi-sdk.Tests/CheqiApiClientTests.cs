using System.Text.Json;
using Cheqi.Sdk.Http.Exceptions;
using Cheqi.Sdk.Models.Generated;
using Cheqi.Sdk.Serialization;
using Cheqi.Sdk.Tests.TestSupport;
using Xunit;

namespace Cheqi.Sdk.Tests;

public sealed class CheqiApiClientTests
{
    [Fact]
    public async Task InProgressResponseIsTypedAndHonorsRetryAfter()
    {
        using var harness = new SdkHarness();
        harness.Enqueue(202, "{\"state\":\"IN_PROGRESS\",\"retryAfterSeconds\":2}", 5);
        var exception = await Assert.ThrowsAsync<SubmissionInProgressException>(() => harness.Sdk.ApiClient.SubmitEncryptedReceiptAsync(new EncryptedReceiptEnvelope("match", new())));
        Assert.Equal(5, exception.RetryAfterSeconds);
        Assert.Equal("correlation-test", exception.CorrelationId);
    }

    [Fact]
    public async Task MatchRetrievalEscapesOpaqueId()
    {
        using var harness = new SdkHarness();
        harness.Enqueue(200, "{\"state\":\"EXPIRED\",\"matchId\":\"a/b?c\"}");
        await harness.Sdk.ApiClient.GetMatchAsync("a/b?c");
        Assert.Equal("/recipient/matches/a%2Fb%3Fc", harness.Requests.Single().Path);
    }

    [Fact]
    public async Task StoreActivationAndDeactivationAcceptNoContent()
    {
        using var harness = new SdkHarness();
        harness.Enqueue(204, "");
        harness.Enqueue(204, "");
        await harness.Sdk.StoreService.ActivateStoreAsync(Guid.Empty, Guid.Empty);
        await harness.Sdk.StoreService.DeactivateStoreAsync(Guid.Empty, Guid.Empty);
        Assert.Equal(new[] { "/company/00000000-0000-0000-0000-000000000000/stores/00000000-0000-0000-0000-000000000000/activate", "/company/00000000-0000-0000-0000-000000000000/stores/00000000-0000-0000-0000-000000000000/deactivate" }, harness.Requests.Select(request => request.Path));
    }

    [Fact]
    public async Task ActiveStoresUseServerFilter()
    {
        using var harness = new SdkHarness();
        harness.Enqueue(200, "[]");
        await harness.Sdk.StoreService.GetActiveStoresAsync(Guid.Empty);
        Assert.Equal("/company/00000000-0000-0000-0000-000000000000/stores?activeOnly=true", harness.Requests.Single().Path);
    }

    [Fact]
    public async Task TransientMatchingRequestsCanRetryWithIdenticalPayload()
    {
        using var harness = new SdkHarness(retries: true);
        harness.Enqueue(503, "{}");
        harness.Enqueue(200, SdkHarness.Match("DOWNLOAD_FALLBACK"));
        await harness.Sdk.MatchingService.MatchCustomerAsync(SdkHarness.Identification());
        Assert.Equal(2, harness.Requests.Count);
        Assert.Equal(harness.Requests[0], harness.Requests[1]);
    }

    [Fact]
    public void OneOfSubmissionResponseDiscriminatesBothShapesAndRoundTrips()
    {
        var options = CheqiJson.CreateOptions();
        var submission = SdkHarness.Read<SubmitEncryptedReceipt202Response>(SdkHarness.Submitted);
        Assert.NotNull(submission.ReceiptSubmissionResponse);
        Assert.Null(submission.MatchStatusResponse);
        var status = SdkHarness.Read<SubmitEncryptedReceipt202Response>("{\"state\":\"IN_PROGRESS\",\"matchId\":\"match\",\"retryAfterSeconds\":2}");
        Assert.Null(status.ReceiptSubmissionResponse);
        Assert.Equal(MatchState.INPROGRESS, status.MatchStatusResponse!.State);
        Assert.Contains("IN_PROGRESS", JsonSerializer.Serialize(status, options));
        Assert.Contains("receipt-1", JsonSerializer.Serialize(submission, options));
    }

    [Fact]
    public void OptionalEnumsStayAbsentAndBooleansAndZeroAmountsSurvive()
    {
        var payload = SdkHarness.Payload();
        payload.TaxesApplied = false;
        payload.TotalTaxAmount = 0;
        using var document = JsonDocument.Parse(JsonSerializer.Serialize(payload, CheqiJson.CreateOptions()));
        Assert.False(document.RootElement.GetProperty("taxesApplied").GetBoolean());
        Assert.Equal(0, document.RootElement.GetProperty("totalTaxAmount").GetDecimal());
        Assert.Equal("{}", JsonSerializer.Serialize(new RecipientResolutionResponse(), CheqiJson.CreateOptions()));
    }
    [Fact]
    public async Task StoreCrudUsesGeneratedWireModels()
    {
        using var harness = new SdkHarness();
        var companyId = Guid.NewGuid();
        var storeId = Guid.NewGuid();
        var json = $"{{\"id\":\"{storeId}\",\"storeName\":\"Store\",\"storeCode\":\"AMS\",\"addressDto\":{{\"streetName\":\"Street\",\"cityName\":\"Amsterdam\",\"postalZone\":\"1000AA\",\"countryIsoCode\":\"NL\"}}}}";
        harness.Enqueue(201, json);
        harness.Enqueue(200, json);
        harness.Enqueue(200, json);
        harness.Enqueue(204, "");
        var request = new CreateStoreRequest("Store", "AMS", new AddressDTO("Street", "Amsterdam", "1000AA", "NL"));
        var store = await harness.Sdk.StoreService.CreateStoreAsync(companyId, request);
        Assert.Equal("Store", store.StoreName);
        Assert.Equal(storeId, store.Id);
        await harness.Sdk.StoreService.UpdateStoreAsync(companyId, storeId, request);
        await harness.Sdk.StoreService.GetStoreAsync(companyId, storeId);
        await harness.Sdk.StoreService.DeleteStoreAsync(companyId, storeId);
        Assert.Equal(new[] { "POST", "PUT", "GET", "DELETE" }, harness.Requests.Select(item => item.Method));
        Assert.Contains("\"addressDto\"", harness.Requests[0].Body);
        Assert.Contains("\"storeCode\":\"AMS\"", harness.Requests[0].Body);
    }

    [Fact]
    public void GeneratedDatesPreserveOriginalOffset()
    {
        var response = SdkHarness.Read<ReceiptSubmissionResponse>("{\"cheqiReceiptId\":\"receipt\",\"matchId\":\"match\",\"createdAt\":\"2026-09-29T10:00:00+02:00\"}");
        Assert.Equal(TimeSpan.FromHours(2), response.CreatedAt!.Value.Offset);
    }

    [Fact]
    public async Task SdkDoesNotDisposeInjectedHttpClient()
    {
        using var harness = new SdkHarness();
        harness.Sdk.Dispose();
        harness.Enqueue(200, "{}");
        Assert.Equal(System.Net.HttpStatusCode.OK, (await harness.Client.GetAsync("/test")).StatusCode);
    }
    [Fact]
    public async Task OAuthUsesCurrentJsonExchangeAndCachesShortLivedToken()
    {
        using var harness = new SdkHarness();
        using var sdk = new CheqiSdk(new Config.CheqiSdkOptions
        {
            Environment = Config.CheqiApiEnvironment.Sandbox,
            SupplierClientId = "client-id", SupplierClientSecret = "client-secret", HttpClient = harness.Client
        });
        harness.Enqueue(200, "{\"access_token\":\"cached-token\",\"expires_in\":60,\"token_type\":\"bearer\"}");
        harness.Enqueue(200, SdkHarness.Match("DOWNLOAD_FALLBACK"));
        harness.Enqueue(200, SdkHarness.Match("DOWNLOAD_FALLBACK"));
        await sdk.MatchingService.MatchCustomerAsync(SdkHarness.Identification());
        await sdk.MatchingService.MatchCustomerAsync(SdkHarness.Identification());
        Assert.Equal(3, harness.Requests.Count);
        Assert.Equal("/oauth/exchange", harness.Requests[0].Path);
        Assert.Contains("\"grant_type\":\"client_credentials\"", harness.Requests[0].Body);
        Assert.Contains("\"client_id\":\"client-id\"", harness.Requests[0].Body);
        Assert.Equal("Bearer cached-token", harness.Requests[1].Authorization);
        Assert.Equal("Bearer cached-token", harness.Requests[2].Authorization);
    }
}
