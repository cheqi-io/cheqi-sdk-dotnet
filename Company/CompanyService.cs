using Cheqi.Sdk.Exceptions;
using Cheqi.Sdk.Http;
using Cheqi.Sdk.Models.Company;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Cheqi.Sdk.Company;

/// <summary>
/// Service for company provisioning operations.
///
/// Enables trusted POS systems to provision companies and receive immediate API access
/// without waiting for merchant OAuth approval.
///
/// Example usage:
/// <code>
/// var company = Company.Builder()
///     .CompanyName("My Store BV")
///     .CompanyLegalName("My Store B.V.")
///     .ChamberOfCommerceNumber("12345678")
///     .CompanyEmail("info@mystore.nl")
///     .Address(Address.Builder()
///         .StreetName("Hoofdstraat 123")
///         .CityName("Amsterdam")
///         .PostalZone("1012AB")
///         .CountryIsoCode("NL")
///         .Build())
///     .Build();
///
/// var response = await sdk.CompanyService
///     .ProvisionCompanyAsync(company, "owner@mystore.nl");
///
/// if (response.IsProvisioned) {
///     var accessToken = response.AccessToken;
///     // Use accessToken to create stores and receipts
/// } else if (response.AlreadyExists) {
///     // Company exists, initiate OAuth flow
/// }
/// </code>
/// </summary>
public class CompanyService
{
    private readonly ILogger<CompanyService> _logger;
    private readonly ICheqiApiClient _apiClient;

    public CompanyService(ICheqiApiClient apiClient, ILogger<CompanyService>? logger = null)
    {
        _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));
        _logger = logger ?? NullLogger<CompanyService>.Instance;
        _logger.LogInformation("CompanyService initialized successfully");
    }

    /// <summary>
    /// Provisions a new company for a merchant using API key from SDK config.
    /// Uses Bearer token authentication with the API key configured during SDK initialization.
    ///
    /// This method creates a new company in the Cheqi system and returns OAuth tokens
    /// for immediate API access. Only available for partner-tier API keys.
    /// </summary>
    /// <param name="company">Company details for provisioning</param>
    /// <param name="adminEmail">Email address for the company admin (receives invitation)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>ProvisionCompanyResponse containing companyId and OAuth tokens</returns>
    /// <exception cref="CheqiSDKException">if provisioning fails or validation errors occur</exception>
    public async Task<ProvisionCompanyResponse> ProvisionCompanyAsync(
        Models.Company.Company company,
        string adminEmail,
        CancellationToken cancellationToken = default)
    {
        if (company == null)
        {
            throw new CheqiSDKException(
                "Company cannot be null",
                CheqiSDKException.ErrorCodes.VALIDATION_ERROR,
                400,
                null);
        }

        if (string.IsNullOrWhiteSpace(company.Name))
        {
            throw new CheqiSDKException(
                "Company name is required",
                CheqiSDKException.ErrorCodes.VALIDATION_ERROR,
                400,
                null);
        }

        if (string.IsNullOrWhiteSpace(adminEmail))
        {
            throw new CheqiSDKException(
                "Admin email is required",
                CheqiSDKException.ErrorCodes.VALIDATION_ERROR,
                400,
                null);
        }

        if (!adminEmail.Contains("@"))
        {
            throw new CheqiSDKException(
                "Admin email must be a valid email address",
                CheqiSDKException.ErrorCodes.VALIDATION_ERROR,
                400,
                null);
        }

        _logger.LogInformation("Provisioning company: {CompanyName}", company.Name);

        try
        {
            var request = new ProvisionCompanyRequest
            {
                Company = company,
                AdminEmail = adminEmail
            };

            var response = await _apiClient.ProvisionCompanyAsync(request, null, cancellationToken);

            if (response.Success)
            {
                _logger.LogInformation("Company provisioned successfully: companyId={CompanyId}", response.Company?.Id);
            }
            else
            {
                _logger.LogWarning("Company provisioning failed: {Message}", response.Message);
            }

            return response;
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to provision company: {Message}", e.Message);
            throw new CheqiSDKException($"Failed to provision company: {e.Message}", e);
        }
    }

    /// <summary>
    /// Provisions a new company (synchronous - Java compatibility).
    /// </summary>
    public ProvisionCompanyResponse ProvisionCompany(
        Models.Company.Company company,
        string adminEmail)
    {
        return ProvisionCompanyAsync(company, adminEmail).GetAwaiter().GetResult();
    }
}
