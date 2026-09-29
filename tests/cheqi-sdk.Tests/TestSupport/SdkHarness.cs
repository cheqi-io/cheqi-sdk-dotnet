using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Cheqi.Sdk.Config;
using Cheqi.Sdk.Models.Generated;
using Cheqi.Sdk.Serialization;

namespace Cheqi.Sdk.Tests.TestSupport;

internal sealed class SdkHarness : IDisposable
{
    private readonly StubHandler _handler;
    private readonly HttpClient _http;
    public CheqiSdk Sdk { get; }
    public HttpClient Client => _http;
    public List<(string Method, string Path, string Body, string? Authorization)> Requests { get; } = new();
    private readonly Queue<Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>>> _responses = new();
    public SdkHarness(bool retries = false)
    {
        _handler = new StubHandler(async (request, ct) =>
        {
            Requests.Add((request.Method.Method, request.RequestUri!.PathAndQuery,
                request.Content == null ? "" : await request.Content.ReadAsStringAsync(ct), request.Headers.Authorization?.ToString()));
            if (_responses.Count == 0) throw new InvalidOperationException("Unexpected HTTP request");
            return await _responses.Dequeue()(request, ct);
        });
        _http = new HttpClient(retries ? new Http.RetryHandler(_handler, 2, TimeSpan.Zero) : _handler);
        Sdk = new CheqiSdk(new CheqiSdkOptions { Environment = CheqiApiEnvironment.Sandbox, ApiKey = "test-key", HttpClient = _http });
    }
    public void Enqueue(int status, string body, int? retryAfter = null)
        => _responses.Enqueue((_, _) =>
        {
            var response = new HttpResponseMessage((HttpStatusCode)status) { Content = new StringContent(body, Encoding.UTF8, "application/json") };
            response.Headers.Add("X-Correlation-ID", "correlation-test");
            if (retryAfter.HasValue) response.Headers.RetryAfter = new System.Net.Http.Headers.RetryConditionHeaderValue(TimeSpan.FromSeconds(retryAfter.Value));
            return Task.FromResult(response);
        });
    public void Fail(Exception exception) => _responses.Enqueue((_, _) => Task.FromException<HttpResponseMessage>(exception));
    public void Enqueue(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> action) => _responses.Enqueue(action);
    public static string Match(string route = "DIGITAL", RSA? rsa = null) => JsonSerializer.Serialize(new
    {
        routeFound = true, matchId = "match-original", deliveryRouteType = route,
        recipients = rsa == null ? Array.Empty<object>() : new object[] { new { id = "device-1", publicKey = SdkHarness.Pem("PUBLIC KEY", rsa.ExportSubjectPublicKeyInfo()) }, new { id = "device-2", publicKey = SdkHarness.Pem("PUBLIC KEY", rsa.ExportSubjectPublicKeyInfo()) } }
    });
    public const string Submitted = "{\"cheqiReceiptId\":\"receipt-1\",\"matchId\":\"match-original\",\"status\":\"PENDING\",\"createdAt\":\"2026-09-29T10:00:00Z\"}";
    public const string DownloadAccepted = "{\"cheqiReceiptId\":\"download-receipt\",\"createdAt\":\"2026-09-29T10:00:00Z\",\"expiresAt\":\"2026-10-01T10:00:00Z\"}";
    public static string Pem(string label, byte[] bytes) => new(PemEncoding.Write(label, bytes));
    public static IdentificationDetails Identification() => new() { PaymentType = PaymentType.CARDPAYMENT, PairingCode = "private-locator" };
    public static ReceiptPayload Payload() => new("POS-001", DateTimeOffset.Parse("2026-09-29T10:00:00Z"), "EUR",
        10m, 10m, 2.1m, 12.1m, true, new List<Product> { new("Coffee", "coffee-1", 1, UnitCode.C62, 10m, 10m, 12.1m) });
    public static string Decrypt(EncryptedReceiptPayload payload, RSA rsa)
    {
        var key = rsa.Decrypt(Convert.FromBase64String(payload.EncryptedAesKey), RSAEncryptionPadding.OaepSHA256);
        return new Decryption.AesDecryptor().Decrypt(payload.EncryptedContent, key);
    }
    public static T Read<T>(string json) => JsonSerializer.Deserialize<T>(json, CheqiJson.CreateOptions())!;
    public void Dispose() { Sdk.Dispose(); _http.Dispose(); }
    private sealed class StubHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> _send;
        public StubHandler(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> send) => _send = send;
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) => _send(request, cancellationToken);
    }
}
