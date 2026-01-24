using Cheqi.Sdk.Http;
using Cheqi.Sdk.Models.Company;
using Microsoft.Extensions.Logging;

namespace Cheqi.Sdk.Services;

/// <summary>
/// Service for managing company stores.
/// </summary>
public sealed class StoreService
{
    private readonly ICheqiApiClient _apiClient;
    private readonly ILogger? _logger;

    public StoreService(ICheqiApiClient apiClient, ILogger? logger = null)
    {
        _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
        _logger = logger;
    }

    /// <summary>
    /// Creates a new store for a company.
    /// </summary>
    /// <param name="companyId">The company ID.</param>
    /// <param name="request">The store creation request.</param>
    /// <param name="accessToken">Optional access token.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created store.</returns>
    public async Task<Store> CreateStoreAsync(
        string companyId,
        CreateStoreRequest request,
        string? accessToken = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(companyId))
            throw new ArgumentException("Company ID is required", nameof(companyId));
        if (request == null)
            throw new ArgumentNullException(nameof(request));
        if (string.IsNullOrEmpty(request.Name))
            throw new ArgumentException("Store name is required", nameof(request));

        _logger?.LogDebug("Creating store {StoreName} for company {CompanyId}", request.Name, companyId);

        var store = await _apiClient.CreateStoreAsync(companyId, request, accessToken, cancellationToken);

        _logger?.LogInformation("Store {StoreName} created with ID {StoreId}", store.Name, store.Id);

        return store;
    }

    /// <summary>
    /// Gets all stores for a company.
    /// </summary>
    /// <param name="companyId">The company ID.</param>
    /// <param name="accessToken">Optional access token.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>List of stores.</returns>
    public async Task<List<Store>> GetStoresAsync(
        string companyId,
        string? accessToken = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(companyId))
            throw new ArgumentException("Company ID is required", nameof(companyId));

        _logger?.LogDebug("Getting stores for company {CompanyId}", companyId);

        var stores = await _apiClient.GetStoresAsync(companyId, accessToken, cancellationToken);

        _logger?.LogDebug("Found {Count} stores for company {CompanyId}", stores.Count, companyId);

        return stores;
    }

    /// <summary>
    /// Gets a specific store.
    /// </summary>
    /// <param name="companyId">The company ID.</param>
    /// <param name="storeId">The store ID.</param>
    /// <param name="accessToken">Optional access token.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The store details.</returns>
    public async Task<Store> GetStoreAsync(
        string companyId,
        string storeId,
        string? accessToken = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(companyId))
            throw new ArgumentException("Company ID is required", nameof(companyId));
        if (string.IsNullOrEmpty(storeId))
            throw new ArgumentException("Store ID is required", nameof(storeId));

        _logger?.LogDebug("Getting store {StoreId} for company {CompanyId}", storeId, companyId);

        return await _apiClient.GetStoreAsync(companyId, storeId, accessToken, cancellationToken);
    }

    /// <summary>
    /// Updates a store.
    /// </summary>
    /// <param name="companyId">The company ID.</param>
    /// <param name="storeId">The store ID.</param>
    /// <param name="request">The update request.</param>
    /// <param name="accessToken">Optional access token.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated store.</returns>
    public async Task<Store> UpdateStoreAsync(
        string companyId,
        string storeId,
        CreateStoreRequest request,
        string? accessToken = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(companyId))
            throw new ArgumentException("Company ID is required", nameof(companyId));
        if (string.IsNullOrEmpty(storeId))
            throw new ArgumentException("Store ID is required", nameof(storeId));
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        _logger?.LogDebug("Updating store {StoreId} for company {CompanyId}", storeId, companyId);

        var store = await _apiClient.UpdateStoreAsync(companyId, storeId, request, accessToken, cancellationToken);

        _logger?.LogInformation("Store {StoreId} updated successfully", storeId);

        return store;
    }

    /// <summary>
    /// Deletes a store.
    /// </summary>
    /// <param name="companyId">The company ID.</param>
    /// <param name="storeId">The store ID.</param>
    /// <param name="accessToken">Optional access token.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task DeleteStoreAsync(
        string companyId,
        string storeId,
        string? accessToken = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(companyId))
            throw new ArgumentException("Company ID is required", nameof(companyId));
        if (string.IsNullOrEmpty(storeId))
            throw new ArgumentException("Store ID is required", nameof(storeId));

        _logger?.LogDebug("Deleting store {StoreId} for company {CompanyId}", storeId, companyId);

        await _apiClient.DeleteStoreAsync(companyId, storeId, accessToken, cancellationToken);

        _logger?.LogInformation("Store {StoreId} deleted successfully", storeId);
    }

    /// <summary>
    /// Activates a store.
    /// </summary>
    /// <param name="companyId">The company ID.</param>
    /// <param name="storeId">The store ID.</param>
    /// <param name="accessToken">Optional access token.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The activated store.</returns>
    public async Task<Store> ActivateStoreAsync(
        string companyId,
        string storeId,
        string? accessToken = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(companyId))
            throw new ArgumentException("Company ID is required", nameof(companyId));
        if (string.IsNullOrEmpty(storeId))
            throw new ArgumentException("Store ID is required", nameof(storeId));

        _logger?.LogDebug("Activating store {StoreId} for company {CompanyId}", storeId, companyId);

        var store = await _apiClient.ActivateStoreAsync(companyId, storeId, accessToken, cancellationToken);

        _logger?.LogInformation("Store {StoreId} activated successfully", storeId);

        return store;
    }

    /// <summary>
    /// Deactivates a store.
    /// </summary>
    /// <param name="companyId">The company ID.</param>
    /// <param name="storeId">The store ID.</param>
    /// <param name="accessToken">Optional access token.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The deactivated store.</returns>
    public async Task<Store> DeactivateStoreAsync(
        string companyId,
        string storeId,
        string? accessToken = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(companyId))
            throw new ArgumentException("Company ID is required", nameof(companyId));
        if (string.IsNullOrEmpty(storeId))
            throw new ArgumentException("Store ID is required", nameof(storeId));

        _logger?.LogDebug("Deactivating store {StoreId} for company {CompanyId}", storeId, companyId);

        var store = await _apiClient.DeactivateStoreAsync(companyId, storeId, accessToken, cancellationToken);

        _logger?.LogInformation("Store {StoreId} deactivated successfully", storeId);

        return store;
    }
}
