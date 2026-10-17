using Cheqi.Sdk.Http;
using Cheqi.Sdk.Models.Generated;
using Microsoft.Extensions.Logging;

namespace Cheqi.Sdk.Company;

/// <summary>Asynchronous store management using generated wire models and typed identifiers.</summary>
public sealed class StoreService
{
    private readonly ICheqiApiClient _api;
    public StoreService(ICheqiApiClient apiClient, ILogger<StoreService>? logger = null)
        => _api = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
    public Task<StoreDTO> CreateStoreAsync(Guid companyId, CreateStoreRequest request, RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _api.CreateStoreAsync(companyId, request, options, cancellationToken);
    public Task<List<StoreDTO>> GetStoresAsync(Guid companyId, bool? activeOnly = null, RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _api.GetStoresAsync(companyId, activeOnly, options, cancellationToken);
    public Task<List<StoreDTO>> GetActiveStoresAsync(Guid companyId, RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _api.GetStoresAsync(companyId, activeOnly: true, options, cancellationToken);
    public Task<StoreDTO> GetStoreAsync(Guid companyId, Guid storeId, RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _api.GetStoreAsync(companyId, storeId, options, cancellationToken);
    public Task<StoreDTO> UpdateStoreAsync(Guid companyId, Guid storeId, CreateStoreRequest request, RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _api.UpdateStoreAsync(companyId, storeId, request, options, cancellationToken);
    public Task DeleteStoreAsync(Guid companyId, Guid storeId, RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _api.DeleteStoreAsync(companyId, storeId, options, cancellationToken);
    public Task ActivateStoreAsync(Guid companyId, Guid storeId, RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _api.ActivateStoreAsync(companyId, storeId, options, cancellationToken);
    public Task DeactivateStoreAsync(Guid companyId, Guid storeId, RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _api.DeactivateStoreAsync(companyId, storeId, options, cancellationToken);
}
