using Cheqi.Sdk.Exceptions;
using Cheqi.Sdk.Http;
using Cheqi.Sdk.Models.Company;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Cheqi.Sdk.Company;

/// <summary>
/// Service for store/location management operations.
///
/// Enables management of stores (locations) for a company. Requires WRITE_STORES scope.
///
/// Example usage:
/// <code>
/// var storeRequest = CreateStoreRequest.Builder()
///     .StoreName("My Store Amsterdam")
///     .StoreCode("STORE-001")
///     .Address(Address.Builder()
///         .StreetName("Kalverstraat 1")
///         .CityName("Amsterdam")
///         .PostalZone("1012NX")
///         .CountryIsoCode("NL")
///         .Build())
///     .PhoneNumber("+31201234567")
///     .Email("amsterdam@mystore.nl")
///     .OpeningHours("Mon-Sat: 09:00-18:00")
///     .Build();
///
/// var store = await sdk.StoreService
///     .CreateStoreAsync(companyId, storeRequest, accessToken);
///
/// // List all stores
/// var stores = await sdk.StoreService
///     .GetStoresAsync(companyId, accessToken);
/// </code>
/// </summary>
public class StoreService
{
    private readonly ILogger<StoreService> _logger;
    private readonly ICheqiApiClient _apiClient;

    public StoreService(ICheqiApiClient apiClient, ILogger<StoreService>? logger = null)
    {
        _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
        _logger = logger ?? NullLogger<StoreService>.Instance;
        _logger.LogInformation("StoreService initialized successfully");
    }

    /// <summary>
    /// Creates a new store/location for a company.
    /// </summary>
    /// <param name="companyId">The company ID to create the store for</param>
    /// <param name="request">Store creation request with store details</param>
    /// <param name="accessToken">OAuth access token with WRITE_STORES scope</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created Store with ID and timestamps</returns>
    /// <exception cref="CheqiSDKException">if creation fails</exception>
    public async Task<Store> CreateStoreAsync(
        string companyId,
        CreateStoreRequest request,
        string accessToken,
        CancellationToken cancellationToken = default)
    {
        ValidateCompanyId(companyId);
        ValidateAccessToken(accessToken);

        if (request == null)
        {
            throw new CheqiSDKException(
                "Store request cannot be null",
                CheqiSDKException.ErrorCodes.VALIDATION_ERROR,
                400,
                null);
        }

        if (!request.IsValid())
        {
            throw new CheqiSDKException(
                $"Invalid store request: {string.Join(", ", request.GetValidationErrors())}",
                CheqiSDKException.ErrorCodes.VALIDATION_ERROR,
                400,
                null);
        }

        _logger.LogInformation("Creating store '{StoreName}' for company {CompanyId}", request.StoreName, companyId);

        try
        {
            var store = await _apiClient.CreateStoreAsync(companyId, request, accessToken, cancellationToken);
            _logger.LogInformation("Store created successfully: storeId={StoreId}", store.Id);
            return store;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to create store: {Message}", e.Message);
            throw new CheqiSDKException($"Failed to create store: {e.Message}", e);
        }
    }

    /// <summary>
    /// Gets all stores for a company.
    /// </summary>
    /// <param name="companyId">The company ID</param>
    /// <param name="accessToken">OAuth access token</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of stores</returns>
    /// <exception cref="CheqiSDKException">if retrieval fails</exception>
    public async Task<List<Store>> GetStoresAsync(
        string companyId,
        string accessToken,
        CancellationToken cancellationToken = default)
    {
        ValidateCompanyId(companyId);
        ValidateAccessToken(accessToken);

        _logger.LogDebug("Getting stores for company {CompanyId}", companyId);

        try
        {
            return await _apiClient.GetStoresAsync(companyId, accessToken, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to get stores: {Message}", e.Message);
            throw new CheqiSDKException($"Failed to get stores: {e.Message}", e);
        }
    }

    /// <summary>
    /// Gets active stores for a company.
    /// </summary>
    /// <param name="companyId">The company ID</param>
    /// <param name="accessToken">OAuth access token</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of active stores</returns>
    /// <exception cref="CheqiSDKException">if retrieval fails</exception>
    public async Task<List<Store>> GetActiveStoresAsync(
        string companyId,
        string accessToken,
        CancellationToken cancellationToken = default)
    {
        ValidateCompanyId(companyId);
        ValidateAccessToken(accessToken);

        _logger.LogDebug("Getting active stores for company {CompanyId}", companyId);

        try
        {
            var allStores = await _apiClient.GetStoresAsync(companyId, accessToken, cancellationToken);
            return allStores.Where(s => s.Active).ToList();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to get active stores: {Message}", e.Message);
            throw new CheqiSDKException($"Failed to get active stores: {e.Message}", e);
        }
    }

    /// <summary>
    /// Gets a specific store by ID.
    /// </summary>
    /// <param name="companyId">The company ID</param>
    /// <param name="storeId">The store ID</param>
    /// <param name="accessToken">OAuth access token</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Store details</returns>
    /// <exception cref="CheqiSDKException">if retrieval fails</exception>
    public async Task<Store> GetStoreAsync(
        string companyId,
        string storeId,
        string accessToken,
        CancellationToken cancellationToken = default)
    {
        ValidateCompanyId(companyId);
        ValidateStoreId(storeId);
        ValidateAccessToken(accessToken);

        _logger.LogDebug("Getting store {StoreId} for company {CompanyId}", storeId, companyId);

        try
        {
            return await _apiClient.GetStoreAsync(companyId, storeId, accessToken, cancellationToken);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to get store: {Message}", e.Message);
            throw new CheqiSDKException($"Failed to get store: {e.Message}", e);
        }
    }

    /// <summary>
    /// Updates a store.
    /// </summary>
    /// <param name="companyId">The company ID</param>
    /// <param name="storeId">The store ID to update</param>
    /// <param name="request">Updated store details</param>
    /// <param name="accessToken">OAuth access token with WRITE_STORES scope</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Updated store</returns>
    /// <exception cref="CheqiSDKException">if update fails</exception>
    public async Task<Store> UpdateStoreAsync(
        string companyId,
        string storeId,
        CreateStoreRequest request,
        string accessToken,
        CancellationToken cancellationToken = default)
    {
        ValidateCompanyId(companyId);
        ValidateStoreId(storeId);
        ValidateAccessToken(accessToken);

        if (request == null)
        {
            throw new CheqiSDKException(
                "Store request cannot be null",
                CheqiSDKException.ErrorCodes.VALIDATION_ERROR,
                400,
                null);
        }

        _logger.LogInformation("Updating store {StoreId} for company {CompanyId}", storeId, companyId);

        try
        {
            var store = await _apiClient.UpdateStoreAsync(companyId, storeId, request, accessToken, cancellationToken);
            _logger.LogInformation("Store updated successfully: storeId={StoreId}", store.Id);
            return store;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to update store: {Message}", e.Message);
            throw new CheqiSDKException($"Failed to update store: {e.Message}", e);
        }
    }

    /// <summary>
    /// Deletes a store.
    /// </summary>
    /// <param name="companyId">The company ID</param>
    /// <param name="storeId">The store ID to delete</param>
    /// <param name="accessToken">OAuth access token with WRITE_STORES scope</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <exception cref="CheqiSDKException">if deletion fails</exception>
    public async Task DeleteStoreAsync(
        string companyId,
        string storeId,
        string accessToken,
        CancellationToken cancellationToken = default)
    {
        ValidateCompanyId(companyId);
        ValidateStoreId(storeId);
        ValidateAccessToken(accessToken);

        _logger.LogInformation("Deleting store {StoreId} for company {CompanyId}", storeId, companyId);

        try
        {
            await _apiClient.DeleteStoreAsync(companyId, storeId, accessToken, cancellationToken);
            _logger.LogInformation("Store deleted successfully: storeId={StoreId}", storeId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to delete store: {Message}", e.Message);
            throw new CheqiSDKException($"Failed to delete store: {e.Message}", e);
        }
    }

    /// <summary>
    /// Activates a store.
    /// </summary>
    /// <param name="companyId">The company ID</param>
    /// <param name="storeId">The store ID to activate</param>
    /// <param name="accessToken">OAuth access token with WRITE_STORES scope</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <exception cref="CheqiSDKException">if activation fails</exception>
    public async Task ActivateStoreAsync(
        string companyId,
        string storeId,
        string accessToken,
        CancellationToken cancellationToken = default)
    {
        ValidateCompanyId(companyId);
        ValidateStoreId(storeId);
        ValidateAccessToken(accessToken);

        _logger.LogInformation("Activating store {StoreId} for company {CompanyId}", storeId, companyId);

        try
        {
            await _apiClient.ActivateStoreAsync(companyId, storeId, accessToken, cancellationToken);
            _logger.LogInformation("Store activated successfully: storeId={StoreId}", storeId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to activate store: {Message}", e.Message);
            throw new CheqiSDKException($"Failed to activate store: {e.Message}", e);
        }
    }

    /// <summary>
    /// Deactivates a store.
    /// </summary>
    /// <param name="companyId">The company ID</param>
    /// <param name="storeId">The store ID to deactivate</param>
    /// <param name="accessToken">OAuth access token with WRITE_STORES scope</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <exception cref="CheqiSDKException">if deactivation fails</exception>
    public async Task DeactivateStoreAsync(
        string companyId,
        string storeId,
        string accessToken,
        CancellationToken cancellationToken = default)
    {
        ValidateCompanyId(companyId);
        ValidateStoreId(storeId);
        ValidateAccessToken(accessToken);

        _logger.LogInformation("Deactivating store {StoreId} for company {CompanyId}", storeId, companyId);

        try
        {
            await _apiClient.DeactivateStoreAsync(companyId, storeId, accessToken, cancellationToken);
            _logger.LogInformation("Store deactivated successfully: storeId={StoreId}", storeId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to deactivate store: {Message}", e.Message);
            throw new CheqiSDKException($"Failed to deactivate store: {e.Message}", e);
        }
    }

    // ============================================
    // Java-style Synchronous Methods (cross-platform consistency)
    // ============================================

    public Store CreateStore(string companyId, CreateStoreRequest request, string accessToken)
        => CreateStoreAsync(companyId, request, accessToken).GetAwaiter().GetResult();

    public List<Store> GetStores(string companyId, string accessToken)
        => GetStoresAsync(companyId, accessToken).GetAwaiter().GetResult();

    public List<Store> GetActiveStores(string companyId, string accessToken)
        => GetActiveStoresAsync(companyId, accessToken).GetAwaiter().GetResult();

    public Store GetStore(string companyId, string storeId, string accessToken)
        => GetStoreAsync(companyId, storeId, accessToken).GetAwaiter().GetResult();

    public Store UpdateStore(string companyId, string storeId, CreateStoreRequest request, string accessToken)
        => UpdateStoreAsync(companyId, storeId, request, accessToken).GetAwaiter().GetResult();

    public void DeleteStore(string companyId, string storeId, string accessToken)
        => DeleteStoreAsync(companyId, storeId, accessToken).GetAwaiter().GetResult();

    public void ActivateStore(string companyId, string storeId, string accessToken)
        => ActivateStoreAsync(companyId, storeId, accessToken).GetAwaiter().GetResult();

    public void DeactivateStore(string companyId, string storeId, string accessToken)
        => DeactivateStoreAsync(companyId, storeId, accessToken).GetAwaiter().GetResult();

    // ============================================
    // Validation Methods
    // ============================================

    private void ValidateCompanyId(string companyId)
    {
        if (string.IsNullOrWhiteSpace(companyId))
        {
            throw new CheqiSDKException(
                "Company ID cannot be null or empty",
                CheqiSDKException.ErrorCodes.VALIDATION_ERROR,
                400,
                null);
        }
    }

    private void ValidateStoreId(string storeId)
    {
        if (string.IsNullOrWhiteSpace(storeId))
        {
            throw new CheqiSDKException(
                "Store ID cannot be null or empty",
                CheqiSDKException.ErrorCodes.VALIDATION_ERROR,
                400,
                null);
        }
    }

    private void ValidateAccessToken(string accessToken)
    {
        if (string.IsNullOrWhiteSpace(accessToken))
        {
            throw new CheqiSDKException(
                "Access token cannot be null or empty",
                CheqiSDKException.ErrorCodes.AUTHENTICATION_FAILED,
                401,
                null);
        }
    }
}
