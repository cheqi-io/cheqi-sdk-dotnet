using Cheqi.Sdk.Encryption;
using Cheqi.Sdk.Exceptions;
using Cheqi.Sdk.Matching;
using Cheqi.Sdk.Models;
using Cheqi.Sdk.Models.Ubl;
using Cheqi.Sdk.Receipt;
using Cheqi.Sdk.Tests.TestSupport;
using Cheqi.Sdk.Utils;
using Xunit;

namespace Cheqi.Sdk.Tests;

public sealed class ReceiptServiceTests
{
    [Fact]
    public async Task ProcessCompleteReceiptAsync_WhenCustomerNotFoundAndNoEmail_SkipsTemplateGeneration()
    {
        var apiClient = new FakeCheqiApiClient();
        var templateCallCount = 0;
        apiClient.GenerateReceiptTemplateAsyncHandler = (_, _, _) =>
        {
            templateCallCount++;
            return Task.FromResult("{}");
        };

        var service = CreateService(apiClient);
        var result = await service.ProcessCompleteReceiptAsync(TestData.PairingCodeIdentification(), TestData.CreateReceiptTemplateRequest(), "token");

        Assert.True(result.IsCustomerNotFound());
        Assert.Equal(0, templateCallCount);
    }

    [Fact]
    public async Task ProcessCompleteReceiptAsync_WhenCustomerNotFoundButEmailPresent_SendsViaEmail()
    {
        var apiClient = new FakeCheqiApiClient();
        string? capturedEmail = null;
        apiClient.SendReceiptViaEmailAsyncHandler = (email, _, _, _) =>
        {
            capturedEmail = email;
            return Task.CompletedTask;
        };

        var service = CreateService(apiClient);
        var result = await service.ProcessCompleteReceiptAsync(
            TestData.EmailIdentification("sdk@example.com"),
            TestData.CreateReceiptTemplateRequest(),
            "token");

        Assert.True(result.IsSuccess());
        Assert.Equal(DeliveryStatus.DELIVERED_EMAIL, result.DeliveryStatus);
        Assert.Equal("sdk@example.com", capturedEmail);
    }

    [Fact]
    public async Task ProcessCompleteReceiptAsync_WhenCustomerFound_SubmitsEncryptedReceiptsWithCanonicalTemplateHash()
    {
        var apiClient = new FakeCheqiApiClient();
        EncryptedReceiptsRequest? capturedRequest = null;
        string? capturedToken = null;
        apiClient.MatchCustomerAsyncHandler = (_, _, _) => Task.FromResult(
            RecipientResolutionResponse.Builder()
                .CustomerFound(true)
                .MatchId("match-123")
                .Recipients(new List<Recipient> { TestData.CreateRecipient() })
                .PaymentMeans(new List<PaymentMeans>
                {
                    new() { PaymentMeansDescription = new List<string> { "Card payment" } }
                })
                .Build());
        apiClient.GenerateReceiptTemplateAsyncHandler = (_, _, _) => Task.FromResult("{\"z\":2,\"a\":1}");
        apiClient.SubmitEncryptedReceiptsAsyncHandler = (request, token, _) =>
        {
            capturedRequest = request;
            capturedToken = token;
            return Task.FromResult(new ReceiptCreatedResponse("receipt-123", DateTimeOffset.Parse("2026-03-15T10:00:00Z"), request.templateHash));
        };

        var service = CreateService(apiClient);
        var result = await service.ProcessCompleteReceiptAsync(TestData.PairingCodeIdentification(), TestData.CreateReceiptTemplateRequest(), "access-token");

        var expectedCanonicalJson = new RFC8785Canonicalizer().Canonicalize(
            "{\"z\":2,\"a\":1,\"paymentMeans\":[{\"paymentMeansDescription\":[\"Card payment\"]}]}");
        var expectedHash = HashUtils.Sha256Hex(expectedCanonicalJson);

        Assert.True(result.IsSuccess());
        Assert.Equal(DeliveryStatus.DELIVERED_DIGITAL, result.DeliveryStatus);
        Assert.Equal(expectedCanonicalJson, result.CanonicalJson);
        Assert.Equal(expectedHash, result.TemplateHash);
        Assert.NotNull(capturedRequest);
        Assert.Equal("match-123", capturedRequest!.matchId);
        Assert.Equal(expectedHash, capturedRequest.templateHash);
        Assert.Single(capturedRequest.encryptedReceipts);
        Assert.Equal("access-token", capturedToken);
    }

    [Fact]
    public async Task SendEncryptedReceiptsAsync_WhenCustomerWasNotFound_ThrowsValidationError()
    {
        var service = CreateService(new FakeCheqiApiClient());

        var exception = await Assert.ThrowsAsync<CheqiSDKException>(() => service.SendEncryptedReceiptsAsync(
            new HashSet<EncryptedReceiptRequestDto>
            {
                EncryptedReceiptRequestDto.Builder()
                    .RecipientId("recipient-1")
                    .ReceiverType(Models.Enums.ReceiverType.DEVICE)
                    .PublicKey("key")
                    .EncryptedReceipt("cipher")
                    .EncryptedSymmetricKey("wrapped")
                    .Build()
            },
            "template-hash",
            RecipientResolutionResponse.CustomerNotFound(),
            "token"));

        Assert.Equal(CheqiSDKException.ErrorCodes.CUSTOMER_NOT_FOUND, exception.ErrorCode);
    }

    private static ReceiptService CreateService(FakeCheqiApiClient apiClient)
    {
        var encryptionService = new EncryptionService();
        var matchingService = new MatchingService(apiClient);
        return new ReceiptService(apiClient, encryptionService, matchingService);
    }
}
