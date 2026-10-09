using Cheqi.Sdk.Http;
using Cheqi.Sdk.Models.Generated;

namespace Cheqi.Sdk.Company;

/// <summary>Webhook subscription management.</summary>
public sealed class WebhookService
{
    private readonly ICheqiApiClient _api;

    public WebhookService(ICheqiApiClient apiClient)
        => _api = apiClient ?? throw new ArgumentNullException(nameof(apiClient));

    public Task<WebhookDTO> UpdateSubscriptionUrlAsync(Guid subscriptionId, string notificationUrl, string? accessToken = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(notificationUrl)) throw new ArgumentException("Notification URL is required", nameof(notificationUrl));
        return _api.UpdateWebhookSubscriptionUrlAsync(subscriptionId, notificationUrl, accessToken, cancellationToken);
    }
}
