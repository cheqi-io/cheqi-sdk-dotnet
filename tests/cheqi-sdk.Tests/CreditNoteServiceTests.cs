using Cheqi.Sdk.CreditNote;
using Cheqi.Sdk.Encryption;
using Cheqi.Sdk.Exceptions;
using Cheqi.Sdk.Matching;
using Cheqi.Sdk.Models;
using Cheqi.Sdk.Tests.TestSupport;
using Xunit;

namespace Cheqi.Sdk.Tests;

public sealed class CreditNoteServiceTests
{
    [Fact]
    public async Task ProcessCompleteCreditNoteAsync_RequiresCheqiReceiptId()
    {
        var service = CreateService(new FakeCheqiApiClient());
        var details = TestData.PairingCodeIdentification();

        var exception = await Assert.ThrowsAsync<CheqiSDKException>(() => service.ProcessCompleteCreditNoteAsync(
            details,
            TestData.CreateCreditNoteTemplateRequest(),
            "token"));

        Assert.Equal(CheqiSDKException.ErrorCodes.VALIDATION_ERROR, exception.ErrorCode);
        Assert.Contains("cheqiReceiptId", exception.Message);
    }

    [Fact]
    public async Task ProcessCompleteCreditNoteAsync_WhenCustomerNotFoundAndNoEmail_ReturnsNoRecipients()
    {
        var apiClient = new FakeCheqiApiClient();
        var templateCallCount = 0;
        apiClient.GenerateCreditNoteTemplateAsyncHandler = (_, _, _) =>
        {
            templateCallCount++;
            return Task.FromResult("{\"id\":\"credit-note\"}");
        };

        var service = CreateService(apiClient);
        var result = await service.ProcessCompleteCreditNoteAsync(
            TestData.ReceiptIdIdentification("receipt-123"),
            TestData.CreateCreditNoteTemplateRequest(),
            "token");

        Assert.False(result.IsSuccess);
        Assert.False(result.RecipientsFound);
        Assert.Equal(0, templateCallCount);
    }

    [Fact]
    public async Task ProcessCompleteCreditNoteAsync_WhenCustomerFound_SubmitsEncryptedCreditNotes()
    {
        var apiClient = new FakeCheqiApiClient();
        EncryptedCreditNotesRequest? capturedRequest = null;
        string? capturedToken = null;
        apiClient.MatchCustomerAsyncHandler = (_, _, _) => Task.FromResult(TestData.CustomerFoundResponse("match-credit"));
        apiClient.GenerateCreditNoteTemplateAsyncHandler = (_, _, _) => Task.FromResult("{\"note\":\"credit\"}");
        apiClient.SubmitEncryptedCreditNotesAsyncHandler = (request, token, _) =>
        {
            capturedRequest = request;
            capturedToken = token;
            return Task.FromResult(new CreditNoteCreatedResponse
            {
                Success = true,
                CreditNoteId = "cn-123",
                RecipientCount = 1
            });
        };

        var service = CreateService(apiClient);
        var result = await service.ProcessCompleteCreditNoteAsync(
            TestData.ReceiptIdIdentification("receipt-123"),
            TestData.CreateCreditNoteTemplateRequest(),
            "oauth-token");

        Assert.True(result.IsSuccess);
        Assert.Equal("cn-123", result.CreditNoteId);
        Assert.Equal(1, result.DeliveryCount);
        Assert.NotNull(capturedRequest);
        Assert.Equal("match-credit", capturedRequest!.MatchId);
        Assert.Equal("receipt-123", capturedRequest.ParentCheqiReceiptId);
        Assert.Single(capturedRequest.EncryptedCreditNotes!);
        Assert.Equal("oauth-token", capturedToken);
    }

    [Fact]
    public async Task GenerateCreditNoteTemplateAsync_RequiresCreditNoteId()
    {
        var service = CreateService(new FakeCheqiApiClient());
        var invalidRequest = CreditNoteTemplateRequest.Builder()
            .DocumentNumber("")
            .OriginatorDocumentReference("INV-001")
            .IssueDate(DateTimeOffset.UtcNow)
            .Currency("EUR")
            .CreditNoteSubTotal(-10m)
            .TotalBeforeTax(-10m)
            .TotalTaxAmount(-2.1m)
            .TotalAmount(-12.1m)
            .AddProduct("Brand", "Name", 10m, -10m, 21.0, -2.1m, -12.1m, -1.0)
            .AddTax(21.0, "VAT", -2.1m)
            .Build();

        var exception = await Assert.ThrowsAsync<CheqiSDKException>(() => service.GenerateCreditNoteTemplateAsync(invalidRequest, "token"));

        Assert.Equal(CheqiSDKException.ErrorCodes.VALIDATION_ERROR, exception.ErrorCode);
    }

    private static CreditNoteService CreateService(FakeCheqiApiClient apiClient)
    {
        var encryptionService = new EncryptionService();
        var matchingService = new MatchingService(apiClient);
        return new CreditNoteService(apiClient, encryptionService, matchingService);
    }
}
