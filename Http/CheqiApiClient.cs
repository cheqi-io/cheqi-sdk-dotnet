using Cheqi.Sdk.Models.Generated;

using Cheqi.Sdk.Serialization;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Cheqi.Sdk.Config;

using Cheqi.Sdk.Http.Exceptions;


using Microsoft.Extensions.Logging;
using RecipientResolutionResponse = Cheqi.Sdk.Models.Generated.RecipientResolutionResponse;
using IdentificationDetails = Cheqi.Sdk.Models.Generated.IdentificationDetails;

namespace Cheqi.Sdk.Http;

/// <summary>
/// HTTP client implementation for the Cheqi API.
/// </summary>
public sealed class CheqiApiClient : ICheqiApiClient
{
    private readonly HttpClient _httpClient;
    private readonly CheqiSdkConfig _config;
    private readonly ILogger? _logger;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly bool _ownsHttpClient;
    private string? _cachedAccessToken;
    private DateTimeOffset _tokenExpiry = DateTimeOffset.MinValue;
    private readonly SemaphoreSlim _tokenLock = new(1, 1);
    private bool _disposed;

    public CheqiApiClient(CheqiSdkConfig config, ILogger? logger = null, HttpClient? httpClient = null)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _logger = logger;

        _jsonOptions = CheqiJson.CreateOptions();

        if (httpClient != null)
        {
            _httpClient = httpClient;
            _ownsHttpClient = false;
        }
        else
        {
            var handler = new RetryHandler(_config.MaxRetries, logger: _logger);
            _httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri(_config.ApiEndpoint),
                Timeout = _config.Timeout
            };
            _ownsHttpClient = true;
        }

        if (_httpClient.BaseAddress == null)
        {
            _httpClient.BaseAddress = new Uri(_config.ApiEndpoint);
        }

        if (!_httpClient.DefaultRequestHeaders.Accept.Any(h => h.MediaType == "application/json"))
        {
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        if (!_httpClient.DefaultRequestHeaders.UserAgent.Any())
        {
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("CheqiSDK/1.0");
        }
    }

    public async Task<RecipientResolutionResponse> ResolveRecipientsAsync(
        IdentificationDetails identificationDetails,
        string? accessToken = null,
        CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, Endpoints.RecipientResolve);
        await SetAuthorizationHeaderAsync(request, accessToken, cancellationToken).ConfigureAwait(false);
        request.Content = CreateJsonContent(identificationDetails);

        var response = await SendAsync<RecipientResolutionResponse>(request, cancellationToken).ConfigureAwait(false);
        return response;
    }

    public Task<RecipientResolutionResponse> MatchCustomerAsync(
        IdentificationDetails identificationDetails,
        string? accessToken = null,
        CancellationToken cancellationToken = default)
    {
        // Alias for ResolveRecipientsAsync - same functionality, different name for consistency
        return ResolveRecipientsAsync(identificationDetails, accessToken, cancellationToken);
    }

    public async Task<MatchStatusResponse> GetMatchAsync(string matchId, string? accessToken = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(matchId)) throw new ArgumentException("matchId is required", nameof(matchId));
        using var request = new HttpRequestMessage(HttpMethod.Get, Endpoints.MatchStatus(matchId));
        await SetAuthorizationHeaderAsync(request, accessToken, cancellationToken).ConfigureAwait(false);
        return await SendAsync<MatchStatusResponse>(request, cancellationToken).ConfigureAwait(false);
    }

    public Task<ReceiptSubmissionResponse> SubmitEncryptedReceiptAsync(EncryptedReceiptEnvelope request, string? accessToken = null, CancellationToken cancellationToken = default)
        => PostAsync<ReceiptSubmissionResponse>(Endpoints.ReceiptEncrypted, request, accessToken, cancellationToken);

    public Task<ReceiptSubmissionResponse> SubmitEncryptedCreditNoteAsync(EncryptedCreditNoteEnvelope request, string? accessToken = null, CancellationToken cancellationToken = default)
        => PostAsync<ReceiptSubmissionResponse>(Endpoints.CreditNoteEncrypted, request, accessToken, cancellationToken);

    public Task<ClientReceiptDownloadResponse> UploadEncryptedDownloadReceiptAsync(ClientReceiptDownloadRequest request, string? accessToken = null, CancellationToken cancellationToken = default)
        => PostAsync<ClientReceiptDownloadResponse>(Endpoints.ReceiptDownload, request, accessToken, cancellationToken);

    private async Task<T> PostAsync<T>(string path, object body, string? accessToken, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(body);
        using var request = new HttpRequestMessage(HttpMethod.Post, path);
        await SetAuthorizationHeaderAsync(request, accessToken, cancellationToken).ConfigureAwait(false);
        request.Content = CreateJsonContent(body);
        if (path is Endpoints.ReceiptEncrypted or Endpoints.CreditNoteEncrypted)
            request.Options.Set(RetryHandler.DisableRetries, true);
        return await SendAsync<T>(request, cancellationToken).ConfigureAwait(false);
    }

    public async Task<StoreDTO> CreateStoreAsync(
        Guid companyId,
        CreateStoreRequest createRequest,
        string? accessToken = null,
        CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, Endpoints.CompanyStores(companyId));
        await SetAuthorizationHeaderAsync(request, accessToken, cancellationToken).ConfigureAwait(false);
        request.Content = CreateJsonContent(createRequest);

        var response = await SendAsync<StoreDTO>(request, cancellationToken).ConfigureAwait(false);
        return response;
    }

    public async Task<InviteEmployeesResponse> InviteUsersAsync(
        Guid companyId,
        List<string> emails,
        string? accessToken = null,
        CancellationToken cancellationToken = default,
        bool? resendPending = null)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, Endpoints.CompanyInviteEmployees(companyId));
        await SetAuthorizationHeaderAsync(request, accessToken, cancellationToken).ConfigureAwait(false);
        var body = new InviteEmployeeRequest(emails);
        if (resendPending.HasValue) body.ResendPending = resendPending.Value;
        request.Content = CreateJsonContent(body);
        return await SendAsync<InviteEmployeesResponse>(request, cancellationToken).ConfigureAwait(false);
    }

    public async Task<List<DestinationResponse>> ListReceiptDestinationsAsync(
        string? accessToken = null, CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, Endpoints.ReceiptDestinations);
        await SetAuthorizationHeaderAsync(request, accessToken, cancellationToken).ConfigureAwait(false);
        return await SendAsync<List<DestinationResponse>>(request, cancellationToken).ConfigureAwait(false);
    }

    public async Task<RegisterResponse> RegisterReceiptDestinationAsync(
        RegisterRequest destination, string? accessToken = null, CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, Endpoints.ReceiptDestinations);
        await SetAuthorizationHeaderAsync(request, accessToken, cancellationToken).ConfigureAwait(false);
        request.Content = CreateJsonContent(destination);
        return await SendAsync<RegisterResponse>(request, cancellationToken).ConfigureAwait(false);
    }

    public async Task<List<WebhookReceiptEnvelope>> GetPendingReceiptsAsync(
        Guid destinationId, string? accessToken = null, CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, Endpoints.ReceiptDestinationQueue(destinationId));
        await SetAuthorizationHeaderAsync(request, accessToken, cancellationToken).ConfigureAwait(false);
        return await SendAsync<List<WebhookReceiptEnvelope>>(request, cancellationToken).ConfigureAwait(false);
    }

    public async Task AcknowledgeReceiptsAsync(
        Guid destinationId, List<string> receiptIds, string? accessToken = null, CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, Endpoints.ReceiptDestinationAcknowledge(destinationId));
        await SetAuthorizationHeaderAsync(request, accessToken, cancellationToken).ConfigureAwait(false);
        request.Content = CreateJsonContent(new AcknowledgeRequest { ReceiptIds = receiptIds });
        await SendAsync(request, cancellationToken).ConfigureAwait(false);
    }

    public async Task DeactivateReceiptDestinationAsync(
        Guid destinationId, string? accessToken = null, CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Delete, Endpoints.ReceiptDestination(destinationId));
        await SetAuthorizationHeaderAsync(request, accessToken, cancellationToken).ConfigureAwait(false);
        await SendAsync(request, cancellationToken).ConfigureAwait(false);
    }

    public async Task<WebhookDTO> UpdateWebhookSubscriptionUrlAsync(
        Guid subscriptionId, string notificationUrl, string? accessToken = null, CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Patch, Endpoints.WebhookSubscription(subscriptionId));
        await SetAuthorizationHeaderAsync(request, accessToken, cancellationToken).ConfigureAwait(false);
        request.Content = CreateJsonContent(new UpdateSubscriptionRequest { NotificationUrl = notificationUrl });
        return await SendAsync<WebhookDTO>(request, cancellationToken).ConfigureAwait(false);
    }

    public async Task<List<StoreDTO>> GetStoresAsync(
        Guid companyId,
        string? accessToken = null,
        CancellationToken cancellationToken = default,
        bool? activeOnly = null)
    {
        var path = Endpoints.CompanyStores(companyId) + (activeOnly.HasValue ? "?activeOnly=" + activeOnly.Value.ToString().ToLowerInvariant() : "");
        using var request = new HttpRequestMessage(HttpMethod.Get, path);
        await SetAuthorizationHeaderAsync(request, accessToken, cancellationToken).ConfigureAwait(false);

        var response = await SendAsync<List<StoreDTO>>(request, cancellationToken).ConfigureAwait(false);
        return response;
    }

    public async Task<StoreDTO> GetStoreAsync(
        Guid companyId,
        Guid storeId,
        string? accessToken = null,
        CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, Endpoints.CompanyStore(companyId, storeId));
        await SetAuthorizationHeaderAsync(request, accessToken, cancellationToken).ConfigureAwait(false);

        var response = await SendAsync<StoreDTO>(request, cancellationToken).ConfigureAwait(false);
        return response;
    }

    public async Task<StoreDTO> UpdateStoreAsync(
        Guid companyId,
        Guid storeId,
        CreateStoreRequest updateRequest,
        string? accessToken = null,
        CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Put, Endpoints.CompanyStore(companyId, storeId));
        await SetAuthorizationHeaderAsync(request, accessToken, cancellationToken).ConfigureAwait(false);
        request.Content = CreateJsonContent(updateRequest);

        var response = await SendAsync<StoreDTO>(request, cancellationToken).ConfigureAwait(false);
        return response;
    }

    public async Task DeleteStoreAsync(
        Guid companyId,
        Guid storeId,
        string? accessToken = null,
        CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Delete, Endpoints.CompanyStore(companyId, storeId));
        await SetAuthorizationHeaderAsync(request, accessToken, cancellationToken).ConfigureAwait(false);

        await SendAsync(request, cancellationToken).ConfigureAwait(false);
    }

    public async Task ActivateStoreAsync(
        Guid companyId,
        Guid storeId,
        string? accessToken = null,
        CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, Endpoints.CompanyStoreActivate(companyId, storeId));
        await SetAuthorizationHeaderAsync(request, accessToken, cancellationToken).ConfigureAwait(false);

        await SendAsync(request, cancellationToken).ConfigureAwait(false);
    }

    public async Task DeactivateStoreAsync(
        Guid companyId,
        Guid storeId,
        string? accessToken = null,
        CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Post, Endpoints.CompanyStoreDeactivate(companyId, storeId));
        await SetAuthorizationHeaderAsync(request, accessToken, cancellationToken).ConfigureAwait(false);

        await SendAsync(request, cancellationToken).ConfigureAwait(false);
    }

    public async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(_config.ClientId) || string.IsNullOrEmpty(_config.ClientSecret))
        {
            throw new InvalidOperationException("Client credentials not configured");
        }

        await _tokenLock.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            // Check if we have a valid cached token
            if (_cachedAccessToken != null && DateTimeOffset.UtcNow < _tokenExpiry)
            {
                return _cachedAccessToken;
            }

            // Request new token
            using var request = new HttpRequestMessage(HttpMethod.Post, Endpoints.OAuthToken);
            request.Content = CreateJsonContent(new ClientCredentialsTokenRequest
            {
                GrantType = ClientCredentialsTokenRequest.GrantTypeEnum.ClientCredentials,
                ClientId = _config.ClientId,
                ClientSecret = _config.ClientSecret
            });
            var tokenResponse = await SendAsync<OAuthTokenExchangeResponse>(request, cancellationToken).ConfigureAwait(false);
            if (string.IsNullOrWhiteSpace(tokenResponse.AccessToken) || tokenResponse.ExpiresIn is null or <= 0)
                throw new CheqiApiException("Invalid token response", 200, CheqiApiException.ErrorCodes.INVALID_RESPONSE, null);

            _cachedAccessToken = tokenResponse.AccessToken;
            // Expire 5 minutes before actual expiry to be safe
            _tokenExpiry = DateTimeOffset.UtcNow.AddSeconds(Math.Max(1, tokenResponse.ExpiresIn.Value - Math.Min(300, tokenResponse.ExpiresIn.Value / 10)));

            return _cachedAccessToken;
        }
        finally
        {
            _tokenLock.Release();
        }
    }

    private async Task SetAuthorizationHeaderAsync(
        HttpRequestMessage request,
        string? accessToken,
        CancellationToken cancellationToken)
    {
        if (accessToken != null && string.IsNullOrWhiteSpace(accessToken)) throw new ArgumentException("accessToken cannot be empty", nameof(accessToken));
        if (!string.IsNullOrEmpty(accessToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }
        else if (!string.IsNullOrEmpty(_config.ApiKey))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _config.ApiKey);
        }
        else if (!string.IsNullOrEmpty(_config.ClientId) && !string.IsNullOrEmpty(_config.ClientSecret))
        {
            var token = await GetAccessTokenAsync(cancellationToken).ConfigureAwait(false);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        else
        {
            throw new InvalidOperationException("No authentication method configured");
        }
    }

    private StringContent CreateJsonContent<T>(T data)
    {
        var json = JsonSerializer.Serialize(data, _jsonOptions);
        return new StringContent(json, Encoding.UTF8, "application/json");
    }

    private async Task<T> SendAsync<T>(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        using var response = await SendTransportAsync(request, cancellationToken).ConfigureAwait(false);
        return await new ResponseHandler(_logger, _jsonOptions).HandleJsonResponseAsync<T>(response, "API request", cancellationToken).ConfigureAwait(false);
    }

    private async Task SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        using var response = await SendTransportAsync(request, cancellationToken).ConfigureAwait(false);
        await new ResponseHandler(_logger, _jsonOptions).HandleVoidResponseAsync(response, "API request", cancellationToken).ConfigureAwait(false);
    }

    private async Task<HttpResponseMessage> SendTransportAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        try { return await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false); }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) { throw; }
        catch (OperationCanceledException exception)
        { throw new CheqiApiException("API request timed out", exception, 0, CheqiApiException.ErrorCodes.TIMEOUT_ERROR, null); }
        catch (HttpRequestException exception)
        { throw new CheqiApiException("API request failed", exception, 0, CheqiApiException.ErrorCodes.NETWORK_ERROR, null); }
    }

    public async Task SendReceiptViaEmailAsync(
        string customerEmail,
        Cheqi.Sdk.Models.Generated.CheqiReceipt cheqiReceipt,
        string? accessToken = null,
        CancellationToken cancellationToken = default)
    {
        var emailRequest = new Cheqi.Sdk.Models.Generated.EmailReceiptRequest(customerEmail, cheqiReceipt);

        using var request = new HttpRequestMessage(HttpMethod.Post, Endpoints.ReceiptEmail);
        await SetAuthorizationHeaderAsync(request, accessToken, cancellationToken).ConfigureAwait(false);
        request.Content = CreateJsonContent(emailRequest);

        await SendAsync(request, cancellationToken).ConfigureAwait(false);
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        if (_ownsHttpClient)
        {
            _httpClient.Dispose();
        }
        _tokenLock.Dispose();
    }

}
