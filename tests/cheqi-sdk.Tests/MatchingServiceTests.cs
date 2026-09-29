using Cheqi.Sdk.Models.Generated;
using Cheqi.Sdk.Tests.TestSupport;
using Xunit;

namespace Cheqi.Sdk.Tests;

public sealed class MatchingServiceTests
{
    [Fact]
    public async Task EmptyIdentificationIsRejectedBeforeHttp()
    {
        using var harness = new SdkHarness();
        await Assert.ThrowsAsync<ArgumentException>(() => harness.Sdk.MatchingService.MatchCustomerAsync(new IdentificationDetails()));
        Assert.Empty(harness.Requests);
    }

    [Fact]
    public async Task PaymentOnlyAnonymousCustomerCanResolveDownloadRoute()
    {
        using var harness = new SdkHarness();
        harness.Enqueue(200, SdkHarness.Match("DOWNLOAD_FALLBACK"));
        var response = await harness.Sdk.MatchingService.MatchCustomerAsync(new IdentificationDetails { PaymentType = PaymentType.CASH });
        Assert.Equal(RecipientResolutionResponse.DeliveryRouteTypeEnum.DOWNLOADFALLBACK, response.DeliveryRouteType);
        Assert.Contains("\"paymentType\":\"CASH\"", harness.Requests.Single().Body);
    }
}
