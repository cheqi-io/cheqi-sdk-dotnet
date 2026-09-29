using Cheqi.Sdk.Models.Generated;

namespace Cheqi.Sdk.Http;

/// <summary>Asynchronous HTTP boundary for handwritten SDK services.</summary>
public interface ICheqiApiClient : IDisposable
{
    Task<RecipientResolutionResponse> ResolveRecipientsAsync(IdentificationDetails identificationDetails, string? accessToken = null, CancellationToken cancellationToken = default);
    Task<RecipientResolutionResponse> MatchCustomerAsync(IdentificationDetails identificationDetails, string? accessToken = null, CancellationToken cancellationToken = default);
    Task<MatchStatusResponse> GetMatchAsync(string matchId, string? accessToken = null, CancellationToken cancellationToken = default);
    Task<ReceiptSubmissionResponse> SubmitEncryptedReceiptAsync(EncryptedReceiptEnvelope request, string? accessToken = null, CancellationToken cancellationToken = default);
    Task<ReceiptSubmissionResponse> SubmitEncryptedCreditNoteAsync(EncryptedCreditNoteEnvelope request, string? accessToken = null, CancellationToken cancellationToken = default);
    Task<ClientReceiptDownloadResponse> UploadEncryptedDownloadReceiptAsync(ClientReceiptDownloadRequest request, string? accessToken = null, CancellationToken cancellationToken = default);
    Task SendReceiptViaEmailAsync(string customerEmail, CheqiReceipt receipt, string? accessToken = null, CancellationToken cancellationToken = default);
    Task<StoreDTO> CreateStoreAsync(Guid companyId, CreateStoreRequest request, string? accessToken = null, CancellationToken cancellationToken = default);
    Task<List<StoreDTO>> GetStoresAsync(Guid companyId, string? accessToken = null, CancellationToken cancellationToken = default, bool? activeOnly = null);
    Task<StoreDTO> GetStoreAsync(Guid companyId, Guid storeId, string? accessToken = null, CancellationToken cancellationToken = default);
    Task<StoreDTO> UpdateStoreAsync(Guid companyId, Guid storeId, CreateStoreRequest request, string? accessToken = null, CancellationToken cancellationToken = default);
    Task DeleteStoreAsync(Guid companyId, Guid storeId, string? accessToken = null, CancellationToken cancellationToken = default);
    Task ActivateStoreAsync(Guid companyId, Guid storeId, string? accessToken = null, CancellationToken cancellationToken = default);
    Task DeactivateStoreAsync(Guid companyId, Guid storeId, string? accessToken = null, CancellationToken cancellationToken = default);
    Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default);
}
