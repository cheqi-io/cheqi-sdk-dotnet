using Cheqi.Sdk.Http;
using Cheqi.Sdk.Models.Generated;

namespace Cheqi.Sdk.Company;

/// <summary>Company membership operations.</summary>
public sealed class CompanyService
{
    private readonly ICheqiApiClient _api;

    public CompanyService(ICheqiApiClient apiClient)
        => _api = apiClient ?? throw new ArgumentNullException(nameof(apiClient));

    /// <summary>Sends invitations and returns their outcomes.</summary>
    public Task<InviteEmployeesResponse> InviteUsersAsync(Guid companyId, List<string> emails, string? accessToken = null, CancellationToken cancellationToken = default, bool? resendPending = null)
    {
        ArgumentNullException.ThrowIfNull(emails);
        if (emails.Count == 0) throw new ArgumentException("At least one email is required", nameof(emails));
        return _api.InviteUsersAsync(companyId, emails, accessToken, cancellationToken, resendPending);
    }

    public Task<List<DestinationResponse>> ListReceiptDestinationsAsync(string? accessToken = null, CancellationToken cancellationToken = default)
        => _api.ListReceiptDestinationsAsync(accessToken, cancellationToken);

    public Task<RegisterResponse> RegisterReceiptDestinationAsync(RegisterRequest request, string? accessToken = null, CancellationToken cancellationToken = default)
        => _api.RegisterReceiptDestinationAsync(request, accessToken, cancellationToken);

    public Task<List<WebhookReceiptEnvelope>> GetPendingReceiptsAsync(Guid destinationId, string? accessToken = null, CancellationToken cancellationToken = default)
        => _api.GetPendingReceiptsAsync(destinationId, accessToken, cancellationToken);

    public Task AcknowledgeReceiptsAsync(Guid destinationId, List<string> receiptIds, string? accessToken = null, CancellationToken cancellationToken = default)
        => _api.AcknowledgeReceiptsAsync(destinationId, receiptIds, accessToken, cancellationToken);

    public Task DeactivateReceiptDestinationAsync(Guid destinationId, string? accessToken = null, CancellationToken cancellationToken = default)
        => _api.DeactivateReceiptDestinationAsync(destinationId, accessToken, cancellationToken);
}
