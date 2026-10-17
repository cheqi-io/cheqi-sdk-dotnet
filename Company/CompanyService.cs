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
    public Task<InviteEmployeesResponse> InviteUsersAsync(Guid companyId, List<string> emails, bool? resendPending = null,
        RequestOptions? options = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(emails);
        if (emails.Count == 0) throw new ArgumentException("At least one email is required", nameof(emails));
        return _api.InviteUsersAsync(companyId, emails, resendPending, options, cancellationToken);
    }

    public Task<List<DestinationResponse>> ListReceiptDestinationsAsync(RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _api.ListReceiptDestinationsAsync(options, cancellationToken);

    public Task<RegisterResponse> RegisterReceiptDestinationAsync(RegisterRequest request, RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _api.RegisterReceiptDestinationAsync(request, options, cancellationToken);

    public Task<List<WebhookReceiptEnvelope>> GetPendingReceiptsAsync(Guid destinationId, RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _api.GetPendingReceiptsAsync(destinationId, options, cancellationToken);

    public Task AcknowledgeReceiptsAsync(Guid destinationId, List<string> receiptIds, RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _api.AcknowledgeReceiptsAsync(destinationId, receiptIds, options, cancellationToken);

    public Task DeactivateReceiptDestinationAsync(Guid destinationId, RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _api.DeactivateReceiptDestinationAsync(destinationId, options, cancellationToken);
}
