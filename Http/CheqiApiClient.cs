using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Cheqi.Sdk.Config;
using Cheqi.Sdk.CreditNote;
using Cheqi.Sdk.Models;
using Cheqi.Sdk.Models.Company;
using Microsoft.Extensions.Logging;

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
    private string? _cachedAccessToken;
    private DateTimeOffset _tokenExpiry = DateTimeOffset.MinValue;
    private readonly SemaphoreSlim _tokenLock = new(1, 1);
    private bool _disposed;

    public CheqiApiClient(CheqiSdkConfig config, ILogger? logger = null, HttpClient? httpClient = null)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _logger = logger;

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        };

        if (httpClient != null)
        {
            _httpClient = httpClient;
        }
        else
        {
            var handler = new RetryHandler(_config.MaxRetries, logger: _logger);
            _httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri(_config.ApiEndpoint),
                Timeout = _config.Timeout
            };
        }

        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<RecipientResolutionResponse> ResolveRecipientsAsync(
        IdentificationDetails identificationDetails,
        string? accessToken = null,
        CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, Endpoints.RecipientResolve);
        await SetAuthorizationHeaderAsync(request, accessToken, cancellationToken);
        request.Content = CreateJsonContent(identificationDetails);

        var response = await SendAsync<RecipientResolutionResponse>(request, cancellationToken);
        return response;
    }

    public async Task<string> GenerateReceiptTemplateAsync(
        ReceiptTemplateRequest templateRequest,
        string? accessToken = null,
        CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, Endpoints.ReceiptTemplate);
        await SetAuthorizationHeaderAsync(request, accessToken, cancellationToken);
        request.Content = CreateJsonContent(templateRequest);

        var response = await SendRawAsync(request, cancellationToken);
        return response;
    }

    public async Task<ReceiptCreatedResponse> SubmitEncryptedReceiptsAsync(
        EncryptedReceiptRequestDto encryptedRequest,
        string? accessToken = null,
        CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, Endpoints.ReceiptEncrypted);
        await SetAuthorizationHeaderAsync(request, accessToken, cancellationToken);
        request.Content = CreateJsonContent(encryptedRequest);

        var response = await SendAsync<ReceiptCreatedResponse>(request, cancellationToken);
        return response;
    }

    public async Task<string> GenerateCreditNoteTemplateAsync(
        CreditNoteTemplateRequest templateRequest,
        string? accessToken = null,
        CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, Endpoints.CreditNoteTemplate);
        await SetAuthorizationHeaderAsync(request, accessToken, cancellationToken);
        request.Content = CreateJsonContent(templateRequest);

        var response = await SendRawAsync(request, cancellationToken);
        return response;
    }

    public async Task<CreditNoteCreatedResponse> SubmitEncryptedCreditNotesAsync(
        EncryptedCreditNoteRequestDto encryptedRequest,
        string? accessToken = null,
        CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, Endpoints.CreditNoteEncrypted);
        await SetAuthorizationHeaderAsync(request, accessToken, cancellationToken);
        request.Content = CreateJsonContent(encryptedRequest);

        var response = await SendAsync<CreditNoteCreatedResponse>(request, cancellationToken);
        return response;
    }

    public async Task<ProvisionCompanyResponse> ProvisionCompanyAsync(
        ProvisionCompanyRequest provisionRequest,
        string? accessToken = null,
        CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, Endpoints.CompanyProvision);
        await SetAuthorizationHeaderAsync(request, accessToken, cancellationToken);
        request.Content = CreateJsonContent(provisionRequest);

        var response = await SendAsync<ProvisionCompanyResponse>(request, cancellationToken);
        return response;
    }

    public async Task<Store> CreateStoreAsync(
        string companyId,
        CreateStoreRequest createRequest,
        string? accessToken = null,
        CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, Endpoints.CompanyStores(companyId));
        await SetAuthorizationHeaderAsync(request, accessToken, cancellationToken);
        request.Content = CreateJsonContent(createRequest);

        var response = await SendAsync<Store>(request, cancellationToken);
        return response;
    }

    public async Task<List<Store>> GetStoresAsync(
        string companyId,
        string? accessToken = null,
        CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, Endpoints.CompanyStores(companyId));
        await SetAuthorizationHeaderAsync(request, accessToken, cancellationToken);

        var response = await SendAsync<List<Store>>(request, cancellationToken);
        return response;
    }

    public async Task<Store> GetStoreAsync(
        string companyId,
        string storeId,
        string? accessToken = null,
        CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, Endpoints.CompanyStore(companyId, storeId));
        await SetAuthorizationHeaderAsync(request, accessToken, cancellationToken);

        var response = await SendAsync<Store>(request, cancellationToken);
        return response;
    }

    public async Task<Store> UpdateStoreAsync(
        string companyId,
        string storeId,
        CreateStoreRequest updateRequest,
        string? accessToken = null,
        CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, Endpoints.CompanyStore(companyId, storeId));
        await SetAuthorizationHeaderAsync(request, accessToken, cancellationToken);
        request.Content = CreateJsonContent(updateRequest);

        var response = await SendAsync<Store>(request, cancellationToken);
        return response;
    }

    public async Task DeleteStoreAsync(
        string companyId,
        string storeId,
        string? accessToken = null,
        CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, Endpoints.CompanyStore(companyId, storeId));
        await SetAuthorizationHeaderAsync(request, accessToken, cancellationToken);

        await SendAsync(request, cancellationToken);
    }

    public async Task<Store> ActivateStoreAsync(
        string companyId,
        string storeId,
        string? accessToken = null,
        CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, Endpoints.CompanyStoreActivate(companyId, storeId));
        await SetAuthorizationHeaderAsync(request, accessToken, cancellationToken);

        var response = await SendAsync<Store>(request, cancellationToken);
        return response;
    }

    public async Task<Store> DeactivateStoreAsync(
        string companyId,
        string storeId,
        string? accessToken = null,
        CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, Endpoints.CompanyStoreDeactivate(companyId, storeId));
        await SetAuthorizationHeaderAsync(request, accessToken, cancellationToken);

        var response = await SendAsync<Store>(request, cancellationToken);
        return response;
    }

    public async Task<string> GetAccessTokenAsync(CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(_config.ClientId) || string.IsNullOrEmpty(_config.ClientSecret))
        {
            throw new InvalidOperationException("Client credentials not configured");
        }

        await _tokenLock.WaitAsync(cancellationToken);
        try
        {
            // Check if we have a valid cached token
            if (_cachedAccessToken != null && DateTimeOffset.UtcNow < _tokenExpiry)
            {
                return _cachedAccessToken;
            }

            // Request new token
            var request = new HttpRequestMessage(HttpMethod.Post, Endpoints.OAuthToken);
            var formData = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                new KeyValuePair<string, string>("client_id", _config.ClientId),
                new KeyValuePair<string, string>("client_secret", _config.ClientSecret)
            });
            request.Content = formData;

            using var response = await _httpClient.SendAsync(request, cancellationToken);
            var content = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new CheqiApiException(
                    $"Failed to obtain access token: {content}",
                    response.StatusCode,
                    content);
            }

            var tokenResponse = JsonSerializer.Deserialize<OAuthTokenResponse>(content, _jsonOptions);
            if (tokenResponse == null || string.IsNullOrEmpty(tokenResponse.AccessToken))
            {
                throw new CheqiApiException("Invalid token response", HttpStatusCode.InternalServerError);
            }

            _cachedAccessToken = tokenResponse.AccessToken;
            // Expire 5 minutes before actual expiry to be safe
            _tokenExpiry = DateTimeOffset.UtcNow.AddSeconds(tokenResponse.ExpiresIn - 300);

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
            var token = await GetAccessTokenAsync(cancellationToken);
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
        var json = await SendRawAsync(request, cancellationToken);
        var result = JsonSerializer.Deserialize<T>(json, _jsonOptions);
        return result ?? throw new CheqiApiException("Failed to deserialize response", HttpStatusCode.InternalServerError);
    }

    private async Task SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        _logger?.LogDebug("Sending {Method} request to {Url}", request.Method, request.RequestUri);

        using var response = await _httpClient.SendAsync(request, cancellationToken);
        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            _logger?.LogError("Request failed: {StatusCode} - {Content}", response.StatusCode, content);
            throw new CheqiApiException(
                $"Request failed with status {response.StatusCode}",
                response.StatusCode,
                content,
                request.RequestUri?.ToString());
        }
    }

    private async Task<string> SendRawAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        _logger?.LogDebug("Sending {Method} request to {Url}", request.Method, request.RequestUri);

        using var response = await _httpClient.SendAsync(request, cancellationToken);
        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            _logger?.LogError("Request failed: {StatusCode} - {Content}", response.StatusCode, content);
            throw new CheqiApiException(
                $"Request failed with status {response.StatusCode}",
                response.StatusCode,
                content,
                request.RequestUri?.ToString());
        }

        return content;
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _httpClient.Dispose();
        _tokenLock.Dispose();
    }

    private sealed class OAuthTokenResponse
    {
        public string? AccessToken { get; set; }
        public string? TokenType { get; set; }
        public int ExpiresIn { get; set; }
    }
}
