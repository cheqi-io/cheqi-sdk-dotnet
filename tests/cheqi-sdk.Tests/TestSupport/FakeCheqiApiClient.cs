using Cheqi.Sdk.CreditNote;
using Cheqi.Sdk.Http;
using Cheqi.Sdk.Models;
using Cheqi.Sdk.Models.Company;
using EncryptedReceiptsRequest = Cheqi.Sdk.Models.Generated.EncryptedReceiptsRequest;
using RecipientResolutionResponse = Cheqi.Sdk.Models.Generated.RecipientResolutionResponse;
using IdentificationDetails = Cheqi.Sdk.Models.Generated.IdentificationDetails;
using ReceiptCreatedResponse = Cheqi.Sdk.Models.Generated.ReceiptCreatedResponse;

namespace Cheqi.Sdk.Tests.TestSupport;

internal sealed class FakeCheqiApiClient : ICheqiApiClient
{
    public Func<IdentificationDetails, string?, CancellationToken, Task<RecipientResolutionResponse>> MatchCustomerAsyncHandler { get; set; } =
        (_, _, _) => Task.FromResult(new RecipientResolutionResponse { CustomerFound = false });

    public Func<ReceiptTemplateRequest, string?, CancellationToken, Task<string>> GenerateReceiptTemplateAsyncHandler { get; set; } =
        (_, _, _) => Task.FromResult("{}");

    public Func<EncryptedReceiptsRequest, string?, CancellationToken, Task<ReceiptCreatedResponse>> SubmitEncryptedReceiptsAsyncHandler { get; set; } =
        (_, _, _) => Task.FromResult(new ReceiptCreatedResponse("cheqi-receipt", DateTimeOffset.UtcNow, "template-hash"));

    public Func<CreditNoteTemplateRequest, string?, CancellationToken, Task<string>> GenerateCreditNoteTemplateAsyncHandler { get; set; } =
        (_, _, _) => Task.FromResult("{\"id\":\"credit-note\"}");

    public Func<EncryptedCreditNotesRequest, string?, CancellationToken, Task<CreditNoteCreatedResponse>> SubmitEncryptedCreditNotesAsyncHandler { get; set; } =
        (_, _, _) => Task.FromResult(new CreditNoteCreatedResponse { Success = true, CreditNoteId = "credit-note-id", RecipientCount = 1 });

    public Func<string, Cheqi.Sdk.Models.Ubl.PurchaseReceipt, string?, CancellationToken, Task> SendReceiptViaEmailAsyncHandler { get; set; } =
        (_, _, _, _) => Task.CompletedTask;

    public Task<RecipientResolutionResponse> ResolveRecipientsAsync(
        IdentificationDetails identificationDetails,
        string? accessToken = null,
        CancellationToken cancellationToken = default) =>
        MatchCustomerAsyncHandler(identificationDetails, accessToken, cancellationToken);

    public Task<RecipientResolutionResponse> MatchCustomerAsync(
        IdentificationDetails identificationDetails,
        string? accessToken = null,
        CancellationToken cancellationToken = default) =>
        MatchCustomerAsyncHandler(identificationDetails, accessToken, cancellationToken);

    public Task<string> GenerateReceiptTemplateAsync(
        ReceiptTemplateRequest request,
        string? accessToken = null,
        CancellationToken cancellationToken = default) =>
        GenerateReceiptTemplateAsyncHandler(request, accessToken, cancellationToken);

    public Task<ReceiptCreatedResponse> SubmitEncryptedReceiptsAsync(
        EncryptedReceiptsRequest request,
        string? accessToken = null,
        CancellationToken cancellationToken = default) =>
        SubmitEncryptedReceiptsAsyncHandler(request, accessToken, cancellationToken);

    public Task<string> GenerateCreditNoteTemplateAsync(
        CreditNoteTemplateRequest request,
        string? accessToken = null,
        CancellationToken cancellationToken = default) =>
        GenerateCreditNoteTemplateAsyncHandler(request, accessToken, cancellationToken);

    public Task<CreditNoteCreatedResponse> SubmitEncryptedCreditNotesAsync(
        EncryptedCreditNotesRequest request,
        string? accessToken = null,
        CancellationToken cancellationToken = default) =>
        SubmitEncryptedCreditNotesAsyncHandler(request, accessToken, cancellationToken);

    public Task<ProvisionCompanyResponse> ProvisionCompanyAsync(
        ProvisionCompanyRequest request,
        string? accessToken = null,
        CancellationToken cancellationToken = default) =>
        throw new NotImplementedException();

    public Task<Store> CreateStoreAsync(
        string companyId,
        CreateStoreRequest request,
        string? accessToken = null,
        CancellationToken cancellationToken = default) =>
        throw new NotImplementedException();

    public Task<List<Store>> GetStoresAsync(
        string companyId,
        string? accessToken = null,
        CancellationToken cancellationToken = default) =>
        throw new NotImplementedException();

    public Task<Store> GetStoreAsync(
        string companyId,
        string storeId,
        string? accessToken = null,
        CancellationToken cancellationToken = default) =>
        throw new NotImplementedException();

    public Task<Store> UpdateStoreAsync(
        string companyId,
        string storeId,
        CreateStoreRequest request,
        string? accessToken = null,
        CancellationToken cancellationToken = default) =>
        throw new NotImplementedException();

    public Task DeleteStoreAsync(
        string companyId,
        string storeId,
        string? accessToken = null,
        CancellationToken cancellationToken = default) =>
        throw new NotImplementedException();

    public Task<Store> ActivateStoreAsync(
        string companyId,
        string storeId,
        string? accessToken = null,
        CancellationToken cancellationToken = default) =>
        throw new NotImplementedException();

    public Task<Store> DeactivateStoreAsync(
        string companyId,
        string storeId,
        string? accessToken = null,
        CancellationToken cancellationToken = default) =>
        throw new NotImplementedException();

    public Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default) =>
        throw new NotImplementedException();

    public Task SendReceiptViaEmailAsync(
        string customerEmail,
        Cheqi.Sdk.Models.Ubl.PurchaseReceipt purchaseReceipt,
        string? accessToken = null,
        CancellationToken cancellationToken = default) =>
        SendReceiptViaEmailAsyncHandler(customerEmail, purchaseReceipt, accessToken, cancellationToken);

    public void Dispose()
    {
    }
}
