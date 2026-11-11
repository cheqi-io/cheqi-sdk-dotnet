using System.Text.Json;
using Cheqi.Sdk.Http.Exceptions;
using Cheqi.Sdk.Models.Generated;
using Cheqi.Sdk.Serialization;
using Cheqi.Sdk.Tests.TestSupport;
using Xunit;

namespace Cheqi.Sdk.Tests;

public class OAuthRedactionTests
{
    [Theory]
    [InlineData(typeof(ClientCredentialsTokenRequest), "ClientSecret", "client_secret")]
    [InlineData(typeof(ClientApplicationPublicKeyRequest), "ClientSecret", "client_secret")]
    [InlineData(typeof(AuthorizationCodeExchangeRequest), "ClientSecret", "client_secret")]
    [InlineData(typeof(AuthorizationCodeExchangeRequest), "Code", "code")]
    [InlineData(typeof(AuthorizationCodeExchangeRequest), "CodeVerifier", "code_verifier")]
    [InlineData(typeof(OAuthTokenExchangeResponse), "AccessToken", "access_token")]
    public void DiagnosticsRedactButWireSerializationPreservesCredentials(Type type, string property, string wireName)
    {
        const string secret = "dummy-sensitive-value\nsecond-line";
        var model = JsonSerializer.Deserialize("{\"publicKey\":\"public\",\"keyAlgorithm\":\"RSA_2048\",\"client_id\":\"client\",\"" + wireName + "\":" + JsonSerializer.Serialize(secret) + "}", type, CheqiJson.CreateOptions())!;
        Assert.Equal(secret, type.GetProperty(property)!.GetValue(model));
        Assert.DoesNotContain("dummy-sensitive-value", model.ToString());
        Assert.DoesNotContain("second-line", model.ToString());
        using var json = JsonDocument.Parse(JsonSerializer.Serialize(model, type, CheqiJson.CreateOptions()));
        Assert.Equal(secret, json.RootElement.GetProperty(wireName).GetString());
    }

    [Theory]
    [InlineData(400, "{\"message\":\"dummy-echoed-secret\"}")]
    [InlineData(200, "{\"access_token\":\"dummy-echoed-secret\",\"expires_in\":\"dummy-echoed-secret\"}")]
    public async Task TokenErrorsNeverIncludeResponseCredentials(int status, string body)
    {
        using var harness = new SdkHarness();
        harness.Enqueue(status, body);
        var error = await Assert.ThrowsAsync<CheqiApiException>(() => harness.Sdk.OAuthService.RequestAccessTokenAsync(
            new ClientCredentialsTokenRequest { ClientId = "client", ClientSecret = "secret", GrantId = Guid.NewGuid() }));
        Assert.DoesNotContain("dummy-echoed-secret", error.ToString());
        Assert.Null(error.InnerException);
        Assert.Equal(status, error.HttpStatusCode);
        Assert.Equal("correlation-test", error.CorrelationId);
    }
}
