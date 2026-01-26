using Cheqi.Sdk.CreditNote;
using Cheqi.Sdk.Models;
using Cheqi.Sdk.Models.Company;

namespace Cheqi.Sdk.Http;

/// <summary>
/// Interface for the Cheqi API client.
/// </summary>
public interface ICheqiApiClient : IDisposable
{
    /// <summary>
    /// Resolves recipients based on identification details.
    /// </summary>
    /// <param name="identificationDetails">The identification details for matching.</param>
    /// <param name="accessToken">Optional OAuth access token.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The recipient resolution response.</returns>
    Task<RecipientResolutionResponse> ResolveRecipientsAsync(
        IdentificationDetails identificationDetails,
        string? accessToken = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Matches a customer based on identification details (alias for ResolveRecipientsAsync).
    /// </summary>
    /// <param name="identificationDetails">The identification details for matching.</param>
    /// <param name="accessToken">Optional OAuth access token.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The recipient resolution response.</returns>
    Task<RecipientResolutionResponse> MatchCustomerAsync(
        IdentificationDetails identificationDetails,
        string? accessToken = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates a receipt template from the provided request.
    /// </summary>
    /// <param name="request">The receipt template request.</param>
    /// <param name="accessToken">Optional OAuth access token.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The generated receipt as JSON.</returns>
    Task<string> GenerateReceiptTemplateAsync(
        ReceiptTemplateRequest request,
        string? accessToken = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Submits encrypted receipts to the API.
    /// </summary>
    /// <param name="request">The encrypted receipts request.</param>
    /// <param name="accessToken">Optional OAuth access token.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The receipt creation response.</returns>
    Task<ReceiptCreatedResponse> SubmitEncryptedReceiptsAsync(
        EncryptedReceiptsRequest request,
        string? accessToken = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Generates a credit note template from the provided request.
    /// </summary>
    /// <param name="request">The credit note template request.</param>
    /// <param name="accessToken">Optional OAuth access token.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The generated credit note as JSON.</returns>
    Task<string> GenerateCreditNoteTemplateAsync(
        CreditNoteTemplateRequest request,
        string? accessToken = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Submits encrypted credit notes to the API.
    /// </summary>
    /// <param name="request">The encrypted credit notes request.</param>
    /// <param name="accessToken">Optional OAuth access token.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The credit note creation response.</returns>
    Task<CreditNoteCreatedResponse> SubmitEncryptedCreditNotesAsync(
        EncryptedCreditNotesRequest request,
        string? accessToken = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Provisions a new company.
    /// </summary>
    /// <param name="request">The company provisioning request.</param>
    /// <param name="accessToken">Optional OAuth access token.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The provisioning response with credentials.</returns>
    Task<ProvisionCompanyResponse> ProvisionCompanyAsync(
        ProvisionCompanyRequest request,
        string? accessToken = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new store for a company.
    /// </summary>
    /// <param name="companyId">The company ID.</param>
    /// <param name="request">The store creation request.</param>
    /// <param name="accessToken">Optional OAuth access token.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created store.</returns>
    Task<Store> CreateStoreAsync(
        string companyId,
        CreateStoreRequest request,
        string? accessToken = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all stores for a company.
    /// </summary>
    /// <param name="companyId">The company ID.</param>
    /// <param name="accessToken">Optional OAuth access token.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of stores.</returns>
    Task<List<Store>> GetStoresAsync(
        string companyId,
        string? accessToken = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a specific store.
    /// </summary>
    /// <param name="companyId">The company ID.</param>
    /// <param name="storeId">The store ID.</param>
    /// <param name="accessToken">Optional OAuth access token.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The store details.</returns>
    Task<Store> GetStoreAsync(
        string companyId,
        string storeId,
        string? accessToken = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates a store.
    /// </summary>
    /// <param name="companyId">The company ID.</param>
    /// <param name="storeId">The store ID.</param>
    /// <param name="request">The update request.</param>
    /// <param name="accessToken">Optional OAuth access token.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated store.</returns>
    Task<Store> UpdateStoreAsync(
        string companyId,
        string storeId,
        CreateStoreRequest request,
        string? accessToken = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a store.
    /// </summary>
    /// <param name="companyId">The company ID.</param>
    /// <param name="storeId">The store ID.</param>
    /// <param name="accessToken">Optional OAuth access token.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task DeleteStoreAsync(
        string companyId,
        string storeId,
        string? accessToken = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Activates a store.
    /// </summary>
    /// <param name="companyId">The company ID.</param>
    /// <param name="storeId">The store ID.</param>
    /// <param name="accessToken">Optional OAuth access token.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The activated store.</returns>
    Task<Store> ActivateStoreAsync(
        string companyId,
        string storeId,
        string? accessToken = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deactivates a store.
    /// </summary>
    /// <param name="companyId">The company ID.</param>
    /// <param name="storeId">The store ID.</param>
    /// <param name="accessToken">Optional OAuth access token.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The deactivated store.</returns>
    Task<Store> DeactivateStoreAsync(
        string companyId,
        string storeId,
        string? accessToken = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtains an OAuth access token using client credentials.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The access token.</returns>
    Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a receipt via email to a customer.
    /// </summary>
    /// <param name="customerEmail">Customer email address.</param>
    /// <param name="purchaseReceipt">The purchase receipt to send.</param>
    /// <param name="accessToken">Optional OAuth access token.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task SendReceiptViaEmailAsync(
        string customerEmail,
        Models.Ubl.PurchaseReceipt purchaseReceipt,
        string? accessToken = null,
        CancellationToken cancellationToken = default);
}
