using Cheqi.Sdk.Encryption;
using Cheqi.Sdk.Exceptions;
using Cheqi.Sdk.Matching;
using Cheqi.Sdk.Models;
using Cheqi.Sdk.Receipt;
using MatchedRecipient = Cheqi.Sdk.Models.Generated.MatchedRecipient;
using EncryptedReceiptRequest = Cheqi.Sdk.Models.Generated.EncryptedReceiptRequest;
using Cheqi.Sdk.Tests.TestSupport;
using Cheqi.Sdk.Utils;
using Xunit;
using EncryptedReceiptsRequest = Cheqi.Sdk.Models.Generated.EncryptedReceiptsRequest;
using RecipientResolutionResponse = Cheqi.Sdk.Models.Generated.RecipientResolutionResponse;
using ReceiptCreatedResponse = Cheqi.Sdk.Models.Generated.ReceiptCreatedResponse;

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
        Assert.Equal(Cheqi.Sdk.Receipt.DeliveryStatus.DELIVERED_EMAIL, result.DeliveryStatus);
        Assert.Equal("sdk@example.com", capturedEmail);
    }

    [Fact]
    public async Task ProcessCompleteReceiptAsync_WhenCustomerFound_SubmitsEncryptedReceiptsWithCanonicalTemplateHash()
    {
        var apiClient = new FakeCheqiApiClient();
        EncryptedReceiptsRequest? capturedRequest = null;
        string? capturedToken = null;

        var matchResponse = new RecipientResolutionResponse();
        matchResponse.CustomerFound = true;
        matchResponse.MatchId = "match-123";
        matchResponse.Recipients = new List<MatchedRecipient> { TestData.CreateRecipient() };

        apiClient.MatchCustomerAsyncHandler = (_, _, _) => Task.FromResult(matchResponse);
        apiClient.GenerateReceiptTemplateAsyncHandler = (_, _, _) => Task.FromResult("{\"z\":2,\"a\":1}");
        apiClient.SubmitEncryptedReceiptsAsyncHandler = (request, token, _) =>
        {
            capturedRequest = request;
            capturedToken = token;
            var resp = new ReceiptCreatedResponse();
            resp.CheqiReceiptId = "receipt-123";
            resp.CreatedAt = DateTimeOffset.Parse("2026-03-15T10:00:00Z");
            resp.TemplateHash = request.TemplateHash;
            return Task.FromResult(resp);
        };

        var service = CreateService(apiClient);
        var result = await service.ProcessCompleteReceiptAsync(TestData.PairingCodeIdentification(), TestData.CreateReceiptTemplateRequest(), "access-token");

        var expectedCanonicalJson = new RFC8785Canonicalizer().Canonicalize("{\"z\":2,\"a\":1}");
        var expectedHash = HashUtils.Sha256Hex(expectedCanonicalJson);

        Assert.True(result.IsSuccess());
        Assert.Equal(Cheqi.Sdk.Receipt.DeliveryStatus.DELIVERED_DIGITAL, result.DeliveryStatus);
        Assert.Equal(expectedCanonicalJson, result.CanonicalJson);
        Assert.Equal(expectedHash, result.TemplateHash);
        Assert.NotNull(capturedRequest);
        Assert.Equal("match-123", capturedRequest!.MatchId);
        Assert.Equal(expectedHash, capturedRequest.TemplateHash);
        Assert.Single(capturedRequest.EncryptedReceiptRequests);
        Assert.Equal("access-token", capturedToken);
    }

    [Fact]
    public async Task SendEncryptedReceiptsAsync_WhenCustomerWasNotFound_ThrowsValidationError()
    {
        var service = CreateService(new FakeCheqiApiClient());

        var dto = new EncryptedReceiptRequest();
        dto.RecipientId = "recipient-1";
        dto.PublicKey = "key";
        dto.EncryptedReceipt = "cipher";
        dto.EncryptedSymmetricKey = "wrapped";

        var notFoundResponse = new RecipientResolutionResponse();
        notFoundResponse.CustomerFound = false;

        var exception = await Assert.ThrowsAsync<CheqiSDKException>(() => service.SendEncryptedReceiptsAsync(
            new HashSet<EncryptedReceiptRequest> { dto },
            "template-hash",
            notFoundResponse,
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
