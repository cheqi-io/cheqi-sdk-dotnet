using Cheqi.Sdk.Models.Generated;

using Cheqi.Sdk.Serialization;
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
/// <remarks>
/// Each call authenticates with <see cref="RequestOptions.AccessToken"/> when present, otherwise with the
/// configured API key. The client holds no tokens of its own.
/// </remarks>
public sealed class CheqiApiClient : ICheqiApiClient
{
    private readonly HttpClient _httpClient;
    private readonly CheqiSdkConfig _config;
    private readonly ILogger? _logger;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly bool _ownsHttpClient;
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
                // Timeouts are applied per call, so RequestOptions.Timeout can exceed the configured default.
                Timeout = System.Threading.Timeout.InfiniteTimeSpan
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
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("CheqiSDK/3.0");
        }
    }

    public async Task<OAuthTokenExchangeResponse> RequestAccessTokenAsync(
        ClientCredentialsTokenRequest request,
        RequestOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (string.IsNullOrWhiteSpace(request.ClientId) || string.IsNullOrWhiteSpace(request.ClientSecret) || request.GrantId == null)
            throw new ArgumentException("Token request requires ClientId, ClientSecret and GrantId", nameof(request));
        request.GrantType ??= ClientCredentialsTokenRequest.GrantTypeEnum.ClientCredentials;

        // Client credentials travel in the body; this call carries no bearer credential.
        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, Endpoints.OAuthToken);
        httpRequest.Content = CreateJsonContent(request);
        httpRequest.Options.Set(RetryHandler.DisableRetries, true);
        var token = await SendAsync<OAuthTokenExchangeResponse>(httpRequest, options ?? RequestOptions.Default, cancellationToken).ConfigureAwait(false);
        if (string.IsNullOrWhiteSpace(token?.AccessToken))
            throw new CheqiApiException("Token response did not include an access token", 0, CheqiApiException.ErrorCodes.INVALID_RESPONSE, null);
        return token;
    }

    public async Task<RecipientResolutionResponse> ResolveRecipientsAsync(
        IdentificationDetails identificationDetails,
        RequestOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        options ??= RequestOptions.Default;
        using var request = new HttpRequestMessage(HttpMethod.Post, Endpoints.RecipientResolve);
        Authorize(request, options);
        request.Content = CreateJsonContent(identificationDetails);

        return await SendAsync<RecipientResolutionResponse>(request, options, cancellationToken).ConfigureAwait(false);
    }

    public Task<RecipientResolutionResponse> MatchCustomerAsync(
        IdentificationDetails identificationDetails,
        RequestOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        // Alias for ResolveRecipientsAsync - same functionality, different name for consistency
        return ResolveRecipientsAsync(identificationDetails, options, cancellationToken);
    }

    public async Task<MatchStatusResponse> GetMatchAsync(string matchId, RequestOptions? options = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(matchId)) throw new ArgumentException("matchId is required", nameof(matchId));
        options ??= RequestOptions.Default;
        using var request = new HttpRequestMessage(HttpMethod.Get, Endpoints.MatchStatus(matchId));
        Authorize(request, options);
        return await SendAsync<MatchStatusResponse>(request, options, cancellationToken).ConfigureAwait(false);
    }

    public Task<ReceiptSubmissionResponse> SubmitEncryptedReceiptAsync(EncryptedReceiptEnvelope request, RequestOptions? options = null, CancellationToken cancellationToken = default)
        => PostAsync<ReceiptSubmissionResponse>(Endpoints.ReceiptEncrypted, request, options, cancellationToken);

    public Task<ReceiptSubmissionResponse> SubmitEncryptedCreditNoteAsync(EncryptedCreditNoteEnvelope request, RequestOptions? options = null, CancellationToken cancellationToken = default)
        => PostAsync<ReceiptSubmissionResponse>(Endpoints.CreditNoteEncrypted, request, options, cancellationToken);

    public Task<ClientReceiptDownloadResponse> UploadEncryptedDownloadReceiptAsync(ClientReceiptDownloadRequest request, RequestOptions? options = null, CancellationToken cancellationToken = default)
        => PostAsync<ClientReceiptDownloadResponse>(Endpoints.ReceiptDownload, request, options, cancellationToken);

    private async Task<T> PostAsync<T>(string path, object body, RequestOptions? options, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(body);
        options ??= RequestOptions.Default;
        using var request = new HttpRequestMessage(HttpMethod.Post, path);
        Authorize(request, options);
        request.Content = CreateJsonContent(body);
        if (path is Endpoints.ReceiptEncrypted or Endpoints.CreditNoteEncrypted)
            request.Options.Set(RetryHandler.DisableRetries, true);
        return await SendAsync<T>(request, options, cancellationToken).ConfigureAwait(false);
    }

    public async Task<StoreDTO> CreateStoreAsync(
        Guid companyId,
        CreateStoreRequest createRequest,
        RequestOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        options ??= RequestOptions.Default;
        using var request = new HttpRequestMessage(HttpMethod.Post, Endpoints.CompanyStores(companyId));
        Authorize(request, options);
        request.Content = CreateJsonContent(createRequest);

        return await SendAsync<StoreDTO>(request, options, cancellationToken).ConfigureAwait(false);
    }

    public async Task<InviteEmployeesResponse> InviteUsersAsync(
        Guid companyId,
        List<string> emails,
        bool? resendPending = null,
        RequestOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        options ??= RequestOptions.Default;
        using var request = new HttpRequestMessage(HttpMethod.Post, Endpoints.CompanyInviteEmployees(companyId));
        Authorize(request, options);
        var body = new InviteEmployeeRequest(emails);
        if (resendPending.HasValue) body.ResendPending = resendPending.Value;
        request.Content = CreateJsonContent(body);
        return await SendAsync<InviteEmployeesResponse>(request, options, cancellationToken).ConfigureAwait(false);
    }

    public async Task<List<DestinationResponse>> ListReceiptDestinationsAsync(
        RequestOptions? options = null, CancellationToken cancellationToken = default)
    {
        options ??= RequestOptions.Default;
        using var request = new HttpRequestMessage(HttpMethod.Get, Endpoints.ReceiptDestinations);
        Authorize(request, options);
        return await SendAsync<List<DestinationResponse>>(request, options, cancellationToken).ConfigureAwait(false);
    }

    public async Task<RegisterResponse> RegisterReceiptDestinationAsync(
        RegisterRequest destination, RequestOptions? options = null, CancellationToken cancellationToken = default)
    {
        options ??= RequestOptions.Default;
        using var request = new HttpRequestMessage(HttpMethod.Post, Endpoints.ReceiptDestinations);
        Authorize(request, options);
        request.Content = CreateJsonContent(destination);
        return await SendAsync<RegisterResponse>(request, options, cancellationToken).ConfigureAwait(false);
    }

    public async Task<List<WebhookReceiptEnvelope>> GetPendingReceiptsAsync(
        Guid destinationId, RequestOptions? options = null, CancellationToken cancellationToken = default)
    {
        options ??= RequestOptions.Default;
        using var request = new HttpRequestMessage(HttpMethod.Get, Endpoints.ReceiptDestinationQueue(destinationId));
        Authorize(request, options);
        return await SendAsync<List<WebhookReceiptEnvelope>>(request, options, cancellationToken).ConfigureAwait(false);
    }

    public async Task AcknowledgeReceiptsAsync(
        Guid destinationId, List<string> receiptIds, RequestOptions? options = null, CancellationToken cancellationToken = default)
    {
        options ??= RequestOptions.Default;
        using var request = new HttpRequestMessage(HttpMethod.Post, Endpoints.ReceiptDestinationAcknowledge(destinationId));
        Authorize(request, options);
        request.Content = CreateJsonContent(new AcknowledgeRequest { ReceiptIds = receiptIds });
        await SendAsync(request, options, cancellationToken).ConfigureAwait(false);
    }

    public async Task DeactivateReceiptDestinationAsync(
        Guid destinationId, RequestOptions? options = null, CancellationToken cancellationToken = default)
    {
        options ??= RequestOptions.Default;
        using var request = new HttpRequestMessage(HttpMethod.Delete, Endpoints.ReceiptDestination(destinationId));
        Authorize(request, options);
        await SendAsync(request, options, cancellationToken).ConfigureAwait(false);
    }

    public async Task<WebhookDTO> UpdateWebhookSubscriptionUrlAsync(
        Guid subscriptionId, string notificationUrl, RequestOptions? options = null, CancellationToken cancellationToken = default)
    {
        options ??= RequestOptions.Default;
        using var request = new HttpRequestMessage(HttpMethod.Patch, Endpoints.WebhookSubscription(subscriptionId));
        Authorize(request, options);
        request.Content = CreateJsonContent(new UpdateSubscriptionRequest { NotificationUrl = notificationUrl });
        return await SendAsync<WebhookDTO>(request, options, cancellationToken).ConfigureAwait(false);
    }

    public async Task<List<StoreDTO>> GetStoresAsync(
        Guid companyId,
        bool? activeOnly = null,
        RequestOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        options ??= RequestOptions.Default;
        var path = Endpoints.CompanyStores(companyId) + (activeOnly.HasValue ? "?activeOnly=" + activeOnly.Value.ToString().ToLowerInvariant() : "");
        using var request = new HttpRequestMessage(HttpMethod.Get, path);
        Authorize(request, options);

        return await SendAsync<List<StoreDTO>>(request, options, cancellationToken).ConfigureAwait(false);
    }

    public async Task<StoreDTO> GetStoreAsync(
        Guid companyId,
        Guid storeId,
        RequestOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        options ??= RequestOptions.Default;
        using var request = new HttpRequestMessage(HttpMethod.Get, Endpoints.CompanyStore(companyId, storeId));
        Authorize(request, options);

        return await SendAsync<StoreDTO>(request, options, cancellationToken).ConfigureAwait(false);
    }

    public async Task<StoreDTO> UpdateStoreAsync(
        Guid companyId,
        Guid storeId,
        CreateStoreRequest updateRequest,
        RequestOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        options ??= RequestOptions.Default;
        using var request = new HttpRequestMessage(HttpMethod.Put, Endpoints.CompanyStore(companyId, storeId));
        Authorize(request, options);
        request.Content = CreateJsonContent(updateRequest);

        return await SendAsync<StoreDTO>(request, options, cancellationToken).ConfigureAwait(false);
    }

    public async Task DeleteStoreAsync(
        Guid companyId,
        Guid storeId,
        RequestOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        options ??= RequestOptions.Default;
        using var request = new HttpRequestMessage(HttpMethod.Delete, Endpoints.CompanyStore(companyId, storeId));
        Authorize(request, options);

        await SendAsync(request, options, cancellationToken).ConfigureAwait(false);
    }

    public async Task ActivateStoreAsync(
        Guid companyId,
        Guid storeId,
        RequestOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        options ??= RequestOptions.Default;
        using var request = new HttpRequestMessage(HttpMethod.Post, Endpoints.CompanyStoreActivate(companyId, storeId));
        Authorize(request, options);

        await SendAsync(request, options, cancellationToken).ConfigureAwait(false);
    }

    public async Task DeactivateStoreAsync(
        Guid companyId,
        Guid storeId,
        RequestOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        options ??= RequestOptions.Default;
        using var request = new HttpRequestMessage(HttpMethod.Post, Endpoints.CompanyStoreDeactivate(companyId, storeId));
        Authorize(request, options);

        await SendAsync(request, options, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>Uses the per-call access token when present, otherwise the configured API key.</summary>
    private void Authorize(HttpRequestMessage request, RequestOptions options)
    {
        var token = options.AccessToken ?? _config.ApiKey;
        if (string.IsNullOrWhiteSpace(token))
            throw new CheqiApiException("No credentials for this call: pass RequestOptions.WithAccessToken(...) or configure ApiKey",
                0, CheqiApiException.ErrorCodes.AUTHENTICATION_FAILED, null);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    private StringContent CreateJsonContent<T>(T data)
    {
        var json = JsonSerializer.Serialize(data, _jsonOptions);
        return new StringContent(json, Encoding.UTF8, "application/json");
    }

    private async Task<T> SendAsync<T>(HttpRequestMessage request, RequestOptions options, CancellationToken cancellationToken)
    {
        using var response = await SendTransportAsync(request, options, cancellationToken).ConfigureAwait(false);
        return await new ResponseHandler(_logger, _jsonOptions).HandleJsonResponseAsync<T>(response, "API request", cancellationToken).ConfigureAwait(false);
    }

    private async Task SendAsync(HttpRequestMessage request, RequestOptions options, CancellationToken cancellationToken)
    {
        using var response = await SendTransportAsync(request, options, cancellationToken).ConfigureAwait(false);
        await new ResponseHandler(_logger, _jsonOptions).HandleVoidResponseAsync(response, "API request", cancellationToken).ConfigureAwait(false);
    }

    private async Task<HttpResponseMessage> SendTransportAsync(HttpRequestMessage request, RequestOptions options, CancellationToken cancellationToken)
    {
        if (options.MaxRetries is int maxRetries) request.Options.Set(RetryHandler.MaxRetriesOverride, maxRetries);
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        timeout.CancelAfter(options.Timeout ?? _config.Timeout);
        try { return await _httpClient.SendAsync(request, timeout.Token).ConfigureAwait(false); }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) { throw; }
        catch (OperationCanceledException exception)
        { throw new CheqiApiException("API request timed out", exception, 0, CheqiApiException.ErrorCodes.TIMEOUT_ERROR, null); }
        catch (HttpRequestException exception)
        { throw new CheqiApiException("API request failed", exception, 0, CheqiApiException.ErrorCodes.NETWORK_ERROR, null); }
    }

    public async Task SendReceiptViaEmailAsync(
        string customerEmail,
        Cheqi.Sdk.Models.Generated.CheqiReceipt cheqiReceipt,
        RequestOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        options ??= RequestOptions.Default;
        var emailRequest = new Cheqi.Sdk.Models.Generated.EmailReceiptRequest(customerEmail, cheqiReceipt);

        using var request = new HttpRequestMessage(HttpMethod.Post, Endpoints.ReceiptEmail);
        Authorize(request, options);
        request.Content = CreateJsonContent(emailRequest);

        await SendAsync(request, options, cancellationToken).ConfigureAwait(false);
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        if (_ownsHttpClient)
        {
            _httpClient.Dispose();
        }
    }

}
