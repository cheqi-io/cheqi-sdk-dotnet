using Cheqi.Sdk.Http.Exceptions;
using Cheqi.Sdk.Matching;
using Cheqi.Sdk.Models;
using Cheqi.Sdk.Models.Enums;
using Cheqi.Sdk.Tests.TestSupport;
using Xunit;

namespace Cheqi.Sdk.Tests;

public sealed class MatchingServiceTests
{
    [Fact]
    public async Task MatchCustomerAsync_WithNoIdentifiers_ThrowsArgumentException()
    {
        var apiClient = new FakeCheqiApiClient();
        var service = new MatchingService(apiClient);
        var details = new IdentificationDetails(PaymentType.CARD_PAYMENT, null, null, null, null, null, null);

        await Assert.ThrowsAsync<ArgumentException>(() => service.MatchCustomerAsync(details, "token"));
    }

    [Fact]
    public async Task MatchCustomerAsync_WithPairingCode_UsesApiClient()
    {
        var apiClient = new FakeCheqiApiClient();
        IdentificationDetails? capturedDetails = null;
        string? capturedToken = null;
        var expected = TestData.CustomerFoundResponse();

        apiClient.MatchCustomerAsyncHandler = (details, token, _) =>
        {
            capturedDetails = details;
            capturedToken = token;
            return Task.FromResult(expected);
        };

        var service = new MatchingService(apiClient);
        var response = await service.MatchCustomerAsync(TestData.PairingCodeIdentification("PAIR-999"), "access-token");

        Assert.Same(expected, response);
        Assert.Equal("PAIR-999", capturedDetails?.PairingCode);
        Assert.Equal("access-token", capturedToken);
    }

    [Fact]
    public async Task MatchCustomerAsync_WithApiKeyPath_PassesNullAccessToken()
    {
        var apiClient = new FakeCheqiApiClient();
        string? capturedToken = "not-null";
        apiClient.MatchCustomerAsyncHandler = (_, token, _) =>
        {
            capturedToken = token;
            return Task.FromResult(RecipientResolutionResponse.CustomerNotFound());
        };

        var service = new MatchingService(apiClient);
        await service.MatchCustomerAsync(TestData.EmailIdentification());

        Assert.Null(capturedToken);
    }

    [Fact]
    public async Task MatchCustomerAsync_WrapsUnexpectedErrorsAsCheqiApiException()
    {
        var apiClient = new FakeCheqiApiClient
        {
            MatchCustomerAsyncHandler = (_, _, _) => throw new InvalidOperationException("boom")
        };

        var service = new MatchingService(apiClient);
        var exception = await Assert.ThrowsAsync<CheqiApiException>(() => service.MatchCustomerAsync(TestData.EmailIdentification(), "token"));

        Assert.Equal(CheqiApiException.ErrorCodes.UNKNOWN_ERROR, exception.ErrorCode);
        Assert.Contains("Customer matching failed", exception.Message);
        Assert.IsType<InvalidOperationException>(exception.InnerException);
    }
}
