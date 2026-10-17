using System.Text.Json;
using Cheqi.Sdk.Http;
using Cheqi.Sdk.Http.Exceptions;
using Cheqi.Sdk.Models.Generated;
using Cheqi.Sdk.Serialization;
using Cheqi.Sdk.Tests.TestSupport;
using Xunit;

namespace Cheqi.Sdk.Tests;

public sealed class CheqiApiClientTests
{
    [Fact]
    public async Task InviteUsersPostsEmailsAndReturnsOutcomes()
    {
        using var harness = new SdkHarness();
        var companyId = Guid.NewGuid();
        harness.Enqueue(200, "{\"invitedEmails\":[\"employee@example.com\"],\"pendingEmails\":[],\"existingMembers\":[]}");
        var result = await harness.Sdk.CompanyService.InviteUsersAsync(companyId, new List<string> { "employee@example.com" }, resendPending: true,
            options: RequestOptions.WithAccessToken("oauth-token"));
        var request = Assert.Single(harness.Requests);
        Assert.Equal("POST", request.Method);
        Assert.Equal($"/company/{companyId:D}/invite/employees", request.Path);
        Assert.Equal("Bearer oauth-token", request.Authorization);
        Assert.Equal("employee@example.com", JsonDocument.Parse(request.Body).RootElement.GetProperty("emails")[0].GetString());
        Assert.True(JsonDocument.Parse(request.Body).RootElement.GetProperty("resendPending").GetBoolean());
        Assert.Equal("employee@example.com", Assert.Single(result.InvitedEmails!));
    }

    [Fact]
    public async Task ReceiptDestinationAndWebhookMethodsUseProductionRoutes()
    {
        using var harness = new SdkHarness();
        var destinationId = Guid.NewGuid();
        var subscriptionId = Guid.NewGuid();
        harness.Enqueue(200, "[]");
        harness.Enqueue(200, "{}");
        harness.Enqueue(200, "[]");
        harness.Enqueue(204, "");
        harness.Enqueue(204, "");
        harness.Enqueue(200, "{}");
        await harness.Sdk.CompanyService.ListReceiptDestinationsAsync();
        await harness.Sdk.CompanyService.RegisterReceiptDestinationAsync(new RegisterRequest("ERP", "key", "RSA"));
        await harness.Sdk.CompanyService.GetPendingReceiptsAsync(destinationId);
        await harness.Sdk.CompanyService.AcknowledgeReceiptsAsync(destinationId, new List<string> { "receipt-1" });
        await harness.Sdk.CompanyService.DeactivateReceiptDestinationAsync(destinationId);
        await harness.Sdk.WebhookService.UpdateSubscriptionUrlAsync(subscriptionId, "https://example.com/hook");
        Assert.Equal(new[] { "GET", "POST", "GET", "POST", "DELETE", "PATCH" }, harness.Requests.Select(x => x.Method));
        Assert.Equal(new[] {
            "/company/receipt-destinations",
            "/company/receipt-destinations",
            $"/company/receipt-destinations/{destinationId:D}/receipts/queue",
            $"/company/receipt-destinations/{destinationId:D}/receipts/queue/acknowledge",
            $"/company/receipt-destinations/{destinationId:D}",
            $"/webhook/subscription/{subscriptionId:D}"
        }, harness.Requests.Select(x => x.Path));
        Assert.Equal("receipt-1", JsonDocument.Parse(harness.Requests[3].Body).RootElement.GetProperty("receiptIds")[0].GetString());
        Assert.Equal("https://example.com/hook", JsonDocument.Parse(harness.Requests[5].Body).RootElement.GetProperty("notificationUrl").GetString());
    }

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
    public async Task RequestAccessTokenPostsClientCredentialsForOneGrantWithoutBearer()
    {
        using var harness = new SdkHarness();
        var grantId = Guid.NewGuid();
        harness.Enqueue(200, "{\"access_token\":\"company-token\",\"expires_in\":900,\"token_type\":\"bearer\",\"scope\":\"receipt:issue\"}");
        var token = await harness.Sdk.OAuthService.RequestAccessTokenAsync(new ClientCredentialsTokenRequest
        {
            ClientId = "client-id", ClientSecret = "client-secret", GrantId = grantId, Scope = "receipt:issue"
        });
        Assert.Equal("company-token", token.AccessToken);
        Assert.Equal(900, token.ExpiresIn);
        var request = Assert.Single(harness.Requests);
        Assert.Equal("/oauth2/token", request.Path);
        Assert.Null(request.Authorization);
        using var body = JsonDocument.Parse(request.Body);
        Assert.Equal("client_credentials", body.RootElement.GetProperty("grant_type").GetString());
        Assert.Equal(grantId, body.RootElement.GetProperty("grant_id").GetGuid());
        Assert.Equal("client-id", body.RootElement.GetProperty("client_id").GetString());
    }

    [Fact]
    public async Task RequestAccessTokenRequiresAGrant()
    {
        using var harness = new SdkHarness();
        await Assert.ThrowsAsync<ArgumentException>(() => harness.Sdk.OAuthService.RequestAccessTokenAsync(
            new ClientCredentialsTokenRequest { ClientId = "client-id", ClientSecret = "client-secret" }));
        Assert.Empty(harness.Requests);
    }

    [Fact]
    public async Task PerCallAccessTokenOverridesApiKeyAndNothingIsCached()
    {
        using var harness = new SdkHarness();
        harness.Enqueue(200, "[]");
        harness.Enqueue(200, "[]");
        harness.Enqueue(200, "[]");
        await harness.Sdk.CompanyService.ListReceiptDestinationsAsync(RequestOptions.WithAccessToken("customer-a"));
        await harness.Sdk.CompanyService.ListReceiptDestinationsAsync(RequestOptions.WithAccessToken("customer-b"));
        await harness.Sdk.CompanyService.ListReceiptDestinationsAsync();
        Assert.Equal(new[] { "Bearer customer-a", "Bearer customer-b", "Bearer test-key" }, harness.Requests.Select(r => r.Authorization));
    }

    [Fact]
    public async Task CallWithoutAccessTokenOrApiKeyFailsBeforeSendingAnything()
    {
        using var harness = new SdkHarness();
        using var sdk = new CheqiSdk(new Config.CheqiSdkOptions { Environment = Config.CheqiApiEnvironment.Sandbox, HttpClient = harness.Client });
        var exception = await Assert.ThrowsAsync<CheqiApiException>(() => sdk.CompanyService.ListReceiptDestinationsAsync());
        Assert.Equal(CheqiApiException.ErrorCodes.AUTHENTICATION_FAILED, exception.ErrorCode);
        Assert.Empty(harness.Requests);
    }

    [Fact]
    public async Task PerCallMaxRetriesOverridesConfiguredRetries()
    {
        using var harness = new SdkHarness(retries: true);
        harness.Enqueue(503, "{}");
        await Assert.ThrowsAsync<CheqiApiException>(() => harness.Sdk.MatchingService.MatchCustomerAsync(
            SdkHarness.Identification(), new RequestOptions { MaxRetries = 0 }));
        Assert.Single(harness.Requests);
    }

    [Fact]
    public async Task PerCallTimeoutIsReportedAsTimeout()
    {
        using var harness = new SdkHarness();
        harness.Enqueue(async (_, ct) =>
        {
            await Task.Delay(TimeSpan.FromSeconds(10), ct);
            return new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.OK);
        });
        var exception = await Assert.ThrowsAsync<CheqiApiException>(() => harness.Sdk.CompanyService.ListReceiptDestinationsAsync(
            new RequestOptions { Timeout = TimeSpan.FromMilliseconds(50) }));
        Assert.Equal(CheqiApiException.ErrorCodes.TIMEOUT_ERROR, exception.ErrorCode);
    }

    [Fact]
    public void RequestOptionsRejectBlankTokensAndHideTokenInToString()
    {
        Assert.Throws<ArgumentException>(() => RequestOptions.WithAccessToken(" "));
        Assert.Throws<ArgumentOutOfRangeException>(() => new RequestOptions { MaxRetries = -1 });
        Assert.DoesNotContain("secret-token", RequestOptions.WithAccessToken("secret-token").ToString());
        Assert.Equal(RequestOptions.WithAccessToken("a"), RequestOptions.WithAccessToken("a"));
    }
}
