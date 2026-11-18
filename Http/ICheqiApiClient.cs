using Cheqi.Sdk.Models.Generated;

namespace Cheqi.Sdk.Http;

/// <summary>
/// Asynchronous HTTP boundary for handwritten SDK services.
/// Every call takes optional <see cref="RequestOptions"/>; <c>null</c> means <see cref="RequestOptions.Default"/>.
/// </summary>
public interface ICheqiApiClient : IDisposable
{
    /// <summary>Exchanges client credentials for a short-lived Company token bound to one Grant.</summary>
    Task<OAuthTokenExchangeResponse> RequestAccessTokenAsync(ClientCredentialsTokenRequest request, RequestOptions? options = null, CancellationToken cancellationToken = default);
    Task<RecipientResolutionResponse> ResolveRecipientsAsync(IdentificationDetails identificationDetails, RequestOptions? options = null, CancellationToken cancellationToken = default);
    Task<RecipientResolutionResponse> MatchCustomerAsync(IdentificationDetails identificationDetails, RequestOptions? options = null, CancellationToken cancellationToken = default);
    Task<MatchStatusResponse> GetMatchAsync(string matchId, RequestOptions? options = null, CancellationToken cancellationToken = default);
    Task<ReceiptSubmissionResponse> SubmitEncryptedReceiptAsync(EncryptedReceiptEnvelope request, RequestOptions? options = null, CancellationToken cancellationToken = default);
    Task<ReceiptSubmissionResponse> SubmitEncryptedCreditNoteAsync(EncryptedCreditNoteEnvelope request, RequestOptions? options = null, CancellationToken cancellationToken = default);
    Task<ClientReceiptDownloadResponse> UploadEncryptedDownloadReceiptAsync(ClientReceiptDownloadRequest request, RequestOptions? options = null, CancellationToken cancellationToken = default);
    Task SendReceiptViaEmailAsync(string customerEmail, CheqiReceipt receipt, RequestOptions? options = null, CancellationToken cancellationToken = default);
    Task<StoreDTO> CreateStoreAsync(Guid companyId, CreateStoreRequest request, RequestOptions? options = null, CancellationToken cancellationToken = default);
    Task<InviteEmployeesResponse> InviteUsersAsync(Guid companyId, List<string> emails, bool? resendPending = null, RequestOptions? options = null, CancellationToken cancellationToken = default);
    Task<List<DestinationResponse>> ListReceiptDestinationsAsync(RequestOptions? options = null, CancellationToken cancellationToken = default);
    Task<RegisterResponse> RegisterReceiptDestinationAsync(RegisterRequest request, RequestOptions? options = null, CancellationToken cancellationToken = default);
    Task<List<WebhookReceiptEnvelope>> GetPendingReceiptsAsync(Guid destinationId, RequestOptions? options = null, CancellationToken cancellationToken = default);
    Task AcknowledgeReceiptsAsync(Guid destinationId, List<string> receiptIds, RequestOptions? options = null, CancellationToken cancellationToken = default);
    Task DeactivateReceiptDestinationAsync(Guid destinationId, RequestOptions? options = null, CancellationToken cancellationToken = default);
    Task<WebhookDTO> UpdateWebhookSubscriptionUrlAsync(Guid subscriptionId, string notificationUrl, RequestOptions? options = null, CancellationToken cancellationToken = default);
    Task<List<StoreDTO>> GetStoresAsync(Guid companyId, bool? activeOnly = null, RequestOptions? options = null, CancellationToken cancellationToken = default);
    Task<StoreDTO> GetStoreAsync(Guid companyId, Guid storeId, RequestOptions? options = null, CancellationToken cancellationToken = default);
    Task<StoreDTO> UpdateStoreAsync(Guid companyId, Guid storeId, CreateStoreRequest request, RequestOptions? options = null, CancellationToken cancellationToken = default);
    Task DeleteStoreAsync(Guid companyId, Guid storeId, RequestOptions? options = null, CancellationToken cancellationToken = default);
    Task ActivateStoreAsync(Guid companyId, Guid storeId, RequestOptions? options = null, CancellationToken cancellationToken = default);
    Task DeactivateStoreAsync(Guid companyId, Guid storeId, RequestOptions? options = null, CancellationToken cancellationToken = default);
}
