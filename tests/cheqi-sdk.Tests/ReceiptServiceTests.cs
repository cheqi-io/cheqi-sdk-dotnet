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
using ReceiptTemplateResponse = Cheqi.Sdk.Models.Generated.ReceiptTemplateResponse;
using CheqiReceipt = Cheqi.Sdk.Models.Generated.CheqiReceipt;

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
            return Task.FromResult(new ReceiptTemplateResponse());
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
        CheqiReceipt? capturedReceipt = null;
        apiClient.GenerateReceiptTemplateAsyncHandler = (_, _, _) =>
            Task.FromResult(new ReceiptTemplateResponse { Cheqi = new CheqiReceipt { DocumentNumber = "INV-001" } });
        apiClient.SendReceiptViaEmailAsyncHandler = (email, receipt, _, _) =>
        {
            capturedEmail = email;
            capturedReceipt = receipt;
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
        Assert.NotNull(capturedReceipt);
        Assert.Equal("INV-001", capturedReceipt!.DocumentNumber);
    }

    [Fact]
    public async Task ProcessCompleteReceiptAsync_WhenCustomerFound_SubmitsPerRecipientEncryptedReceipts()
    {
        var apiClient = new FakeCheqiApiClient();
        EncryptedReceiptsRequest? capturedRequest = null;
        string? capturedToken = null;
        ReceiptTemplateGenerationRequestSeen? seen = null;

        var matchResponse = new RecipientResolutionResponse();
        matchResponse.RouteFound = true;
        matchResponse.MatchId = "match-123";
        matchResponse.BuyerCountryCode = "NL";
        matchResponse.Recipients = new List<MatchedRecipient> { TestData.CreateRecipient() };

        apiClient.MatchCustomerAsyncHandler = (_, _, _) => Task.FromResult(matchResponse);
        apiClient.GenerateReceiptTemplateAsyncHandler = (genReq, _, _) =>
        {
            seen = new ReceiptTemplateGenerationRequestSeen(genReq.BuyerCountryCode, genReq.TaxesApplied, genReq.Formats.Count);
            return Task.FromResult(new ReceiptTemplateResponse { Cheqi = new CheqiReceipt { DocumentNumber = "INV-001" } });
        };
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

        Assert.True(result.IsSuccess());
        Assert.Equal(Cheqi.Sdk.Receipt.DeliveryStatus.DELIVERED_DIGITAL, result.DeliveryStatus);
        Assert.NotNull(capturedRequest);
        Assert.Equal("match-123", capturedRequest!.MatchId);
        Assert.False(string.IsNullOrEmpty(capturedRequest.TemplateHash));
        Assert.Single(capturedRequest.EncryptedReceiptRequests);
        Assert.Equal("access-token", capturedToken);

        // VAT context enrichment: buyerCountryCode came from the match response, taxesApplied inferred true.
        Assert.NotNull(seen);
        Assert.Equal("NL", seen!.BuyerCountryCode);
        Assert.True(seen.TaxesApplied);
        Assert.Equal(1, seen.FormatCount);
    }

    private sealed record ReceiptTemplateGenerationRequestSeen(string BuyerCountryCode, bool TaxesApplied, int FormatCount);

    [Fact]
    public async Task SendEncryptedReceiptsAsync_WhenCustomerWasNotFound_ThrowsValidationError()
    {
        var service = CreateService(new FakeCheqiApiClient());

        var dto = new EncryptedReceiptRequest(
            recipientId: "recipient-1",
            encryptedReceipt: "cipher",
            encryptedSymmetricKey: "wrapped",
            publicKey: "key");

        var notFoundResponse = new RecipientResolutionResponse();
        notFoundResponse.RouteFound = false;

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
