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
    public Task<StoreDTO> CreateStoreAsync(Guid companyId, CreateStoreRequest request, string? accessToken = null, CancellationToken cancellationToken = default)
        => _api.CreateStoreAsync(companyId, request, accessToken, cancellationToken);
    public Task<List<StoreDTO>> GetStoresAsync(Guid companyId, string? accessToken = null, CancellationToken cancellationToken = default, bool? activeOnly = null)
        => _api.GetStoresAsync(companyId, accessToken, cancellationToken, activeOnly);
    public Task<List<StoreDTO>> GetActiveStoresAsync(Guid companyId, string? accessToken = null, CancellationToken cancellationToken = default)
        => _api.GetStoresAsync(companyId, accessToken, cancellationToken, activeOnly: true);
    public Task<StoreDTO> GetStoreAsync(Guid companyId, Guid storeId, string? accessToken = null, CancellationToken cancellationToken = default)
        => _api.GetStoreAsync(companyId, storeId, accessToken, cancellationToken);
    public Task<StoreDTO> UpdateStoreAsync(Guid companyId, Guid storeId, CreateStoreRequest request, string? accessToken = null, CancellationToken cancellationToken = default)
        => _api.UpdateStoreAsync(companyId, storeId, request, accessToken, cancellationToken);
    public Task DeleteStoreAsync(Guid companyId, Guid storeId, string? accessToken = null, CancellationToken cancellationToken = default)
        => _api.DeleteStoreAsync(companyId, storeId, accessToken, cancellationToken);
    public Task ActivateStoreAsync(Guid companyId, Guid storeId, string? accessToken = null, CancellationToken cancellationToken = default)
        => _api.ActivateStoreAsync(companyId, storeId, accessToken, cancellationToken);
    public Task DeactivateStoreAsync(Guid companyId, Guid storeId, string? accessToken = null, CancellationToken cancellationToken = default)
        => _api.DeactivateStoreAsync(companyId, storeId, accessToken, cancellationToken);
}
