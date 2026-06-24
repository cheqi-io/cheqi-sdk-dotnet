using Cheqi.Sdk.CreditNote;
using Cheqi.Sdk.Http;
using Cheqi.Sdk.Models;
using Cheqi.Sdk.Models.Company;
using EncryptedReceiptsRequest = Cheqi.Sdk.Models.Generated.EncryptedReceiptsRequest;
using RecipientResolutionResponse = Cheqi.Sdk.Models.Generated.RecipientResolutionResponse;
using IdentificationDetails = Cheqi.Sdk.Models.Generated.IdentificationDetails;
using ReceiptCreatedResponse = Cheqi.Sdk.Models.Generated.ReceiptCreatedResponse;
using ReceiptTemplateGenerationRequest = Cheqi.Sdk.Models.Generated.ReceiptTemplateGenerationRequest;
using ReceiptTemplateResponse = Cheqi.Sdk.Models.Generated.ReceiptTemplateResponse;
using CheqiReceipt = Cheqi.Sdk.Models.Generated.CheqiReceipt;
using CreditNoteTemplateGenerationRequest = Cheqi.Sdk.Models.Generated.CreditNoteTemplateGenerationRequest;
using CreditNoteTemplateResponse = Cheqi.Sdk.Models.Generated.CreditNoteTemplateResponse;

namespace Cheqi.Sdk.Tests.TestSupport;

internal sealed class FakeCheqiApiClient : ICheqiApiClient
{
    public Func<IdentificationDetails, string?, CancellationToken, Task<RecipientResolutionResponse>> MatchCustomerAsyncHandler { get; set; } =
        (_, _, _) => Task.FromResult(new RecipientResolutionResponse { RouteFound = false });

    public Func<ReceiptTemplateGenerationRequest, string?, CancellationToken, Task<ReceiptTemplateResponse>> GenerateReceiptTemplateAsyncHandler { get; set; } =
        (_, _, _) => Task.FromResult(new ReceiptTemplateResponse());

    public Func<EncryptedReceiptsRequest, string?, CancellationToken, Task<ReceiptCreatedResponse>> SubmitEncryptedReceiptsAsyncHandler { get; set; } =
        (_, _, _) => Task.FromResult(new ReceiptCreatedResponse("cheqi-receipt", DateTimeOffset.UtcNow, "template-hash"));

    public Func<CreditNoteTemplateGenerationRequest, string?, CancellationToken, Task<CreditNoteTemplateResponse>> GenerateCreditNoteTemplateAsyncHandler { get; set; } =
        (_, _, _) => Task.FromResult(new CreditNoteTemplateResponse());

    public Func<EncryptedCreditNotesRequest, string?, CancellationToken, Task<CreditNoteCreatedResponse>> SubmitEncryptedCreditNotesAsyncHandler { get; set; } =
        (_, _, _) => Task.FromResult(new CreditNoteCreatedResponse { Success = true, CreditNoteId = "credit-note-id", RecipientCount = 1 });

    public Func<string, CheqiReceipt, string?, CancellationToken, Task> SendReceiptViaEmailAsyncHandler { get; set; } =
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

    public Task<ReceiptTemplateResponse> GenerateReceiptTemplateAsync(
        ReceiptTemplateGenerationRequest request,
        string? accessToken = null,
        CancellationToken cancellationToken = default) =>
        GenerateReceiptTemplateAsyncHandler(request, accessToken, cancellationToken);

    public Task<ReceiptCreatedResponse> SubmitEncryptedReceiptsAsync(
        EncryptedReceiptsRequest request,
        string? accessToken = null,
        CancellationToken cancellationToken = default) =>
        SubmitEncryptedReceiptsAsyncHandler(request, accessToken, cancellationToken);

    public Task<CreditNoteTemplateResponse> GenerateCreditNoteTemplateAsync(
        CreditNoteTemplateGenerationRequest request,
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
        CheqiReceipt cheqiReceipt,
        string? accessToken = null,
        CancellationToken cancellationToken = default) =>
        SendReceiptViaEmailAsyncHandler(customerEmail, cheqiReceipt, accessToken, cancellationToken);

    public void Dispose()
    {
    }
}
