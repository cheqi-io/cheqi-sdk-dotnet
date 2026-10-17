using Cheqi.Sdk.Http;
using Cheqi.Sdk.Models.Generated;

namespace Cheqi.Sdk.OAuth;

/// <summary>
/// Obtains short-lived Company access tokens with the OAuth client-credentials grant.
/// </summary>
/// <remarks>
/// Each token is bound to one Grant, so request one per connected company. The SDK does not cache or
/// refresh tokens: store each token with its <c>ExpiresIn</c> in your own token store, request a new one
/// before it expires, and pass it per call with <see cref="RequestOptions.WithAccessToken"/>.
/// <code>
/// var token = await sdk.OAuthService.RequestAccessTokenAsync(new ClientCredentialsTokenRequest
/// {
///     ClientId = clientId,
///     ClientSecret = clientSecret,
///     GrantId = grantId,
///     Scope = "receipt:issue"
/// });
/// </code>
/// </remarks>
public sealed class OAuthService
{
    private readonly ICheqiApiClient _api;

    public OAuthService(ICheqiApiClient apiClient)
        => _api = apiClient ?? throw new ArgumentNullException(nameof(apiClient));

    /// <summary>Requests an access token for one Grant. <c>GrantType</c> defaults to <c>client_credentials</c>.</summary>
    public Task<OAuthTokenExchangeResponse> RequestAccessTokenAsync(ClientCredentialsTokenRequest request,
        RequestOptions? options = null, CancellationToken cancellationToken = default)
        => _api.RequestAccessTokenAsync(request, options, cancellationToken);
}
