using Cheqi.Sdk.Http;
using Cheqi.Sdk.Models.Company;
using Microsoft.Extensions.Logging;

namespace Cheqi.Sdk.Services;

/// <summary>
/// Service for company provisioning.
/// </summary>
public sealed class CompanyService
{
    private readonly ICheqiApiClient _apiClient;
    private readonly ILogger? _logger;

    public CompanyService(ICheqiApiClient apiClient, ILogger? logger = null)
    {
        _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
        _logger = logger;
    }

    /// <summary>
    /// Provisions a new company and returns credentials for API access.
    /// </summary>
    /// <param name="request">The company provisioning request.</param>
    /// <param name="accessToken">Optional access token.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The provisioning response with company details and credentials.</returns>
    public async Task<ProvisionCompanyResponse> ProvisionCompanyAsync(
        ProvisionCompanyRequest request,
        string? accessToken = null,
        CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));
        if (request.Company == null)
            throw new ArgumentException("Company details are required", nameof(request));
        if (string.IsNullOrEmpty(request.Company.Name))
            throw new ArgumentException("Company name is required", nameof(request));

        _logger?.LogDebug("Provisioning company: {CompanyName}", request.Company.Name);

        var response = await _apiClient.ProvisionCompanyAsync(request, accessToken, cancellationToken);

        if (response.Success)
        {
            _logger?.LogInformation(
                "Company {CompanyName} provisioned successfully with ID {CompanyId}",
                request.Company.Name,
                response.Company?.Id);
        }
        else
        {
            _logger?.LogWarning(
                "Company provisioning failed for {CompanyName}: {Message}",
                request.Company.Name,
                response.Message);
        }

        return response;
    }

    /// <summary>
    /// Provisions a new company with minimal required information.
    /// </summary>
    /// <param name="company">The company details.</param>
    /// <param name="adminEmail">Optional admin email for notifications.</param>
    /// <param name="sandbox">Whether to create sandbox credentials.</param>
    /// <param name="accessToken">Optional access token.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The provisioning response.</returns>
    public Task<ProvisionCompanyResponse> ProvisionCompanyAsync(
        Company company,
        string? adminEmail = null,
        bool sandbox = false,
        string? accessToken = null,
        CancellationToken cancellationToken = default)
    {
        var request = new ProvisionCompanyRequest
        {
            Company = company,
            AdminEmail = adminEmail,
            Sandbox = sandbox
        };

        return ProvisionCompanyAsync(request, accessToken, cancellationToken);
    }
}
