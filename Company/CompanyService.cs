using Cheqi.Sdk.Http;

namespace Cheqi.Sdk.Company;

/// <summary>Company membership operations.</summary>
public sealed class CompanyService
{
    private readonly ICheqiApiClient _api;

    public CompanyService(ICheqiApiClient apiClient)
        => _api = apiClient ?? throw new ArgumentNullException(nameof(apiClient));

    /// <summary>Sends invitations. A successful response has no body.</summary>
    public Task InviteUsersAsync(Guid companyId, List<string> emails, string? accessToken = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(emails);
        if (emails.Count == 0) throw new ArgumentException("At least one email is required", nameof(emails));
        return _api.InviteUsersAsync(companyId, emails, accessToken, cancellationToken);
    }
}
