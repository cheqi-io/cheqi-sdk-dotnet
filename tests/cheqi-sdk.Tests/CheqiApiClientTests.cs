using System.Net;
using System.Text;
using Cheqi.Sdk.Config;
using Cheqi.Sdk.Http;
using Cheqi.Sdk.Http.Exceptions;
using Cheqi.Sdk.Models;
using Cheqi.Sdk.Tests.TestSupport;
using Microsoft.Extensions.Logging;
using Xunit;

namespace Cheqi.Sdk.Tests;

public sealed class CheqiApiClientTests
{
    [Fact]
    public async Task GenerateReceiptTemplateAsync_OnHttpError_DoesNotLogResponseBody()
    {
        var loggerProvider = new ListLoggerProvider();
        using var loggerFactory = LoggerFactory.Create(builder => builder.AddProvider(loggerProvider));

        var config = CheqiSdkConfig.BuilderInstance()
            .CustomApiEndpoint("https://example.test")
            .ApiKey("sk_test_example")
            .Build();

        using var httpClient = new HttpClient(new StaticResponseHandler(
            new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("{\"message\":\"customer@example.com should not appear\"}", Encoding.UTF8, "application/json")
            }))
        {
            BaseAddress = new Uri("https://example.test")
        };

        var client = new CheqiApiClient(config, loggerFactory.CreateLogger<CheqiApiClient>(), httpClient);

        await Assert.ThrowsAsync<CheqiApiException>(() =>
            client.GenerateReceiptTemplateAsync(new ReceiptTemplateRequest(), cancellationToken: CancellationToken.None));

        var combinedLogs = string.Join("\n", loggerProvider.Messages);
        Assert.DoesNotContain("customer@example.com", combinedLogs);
        Assert.DoesNotContain("should not appear", combinedLogs);
        Assert.Contains("Request failed:", combinedLogs);
    }

    [Fact]
    public void Dispose_DoesNotDisposeInjectedHttpClient()
    {
        var handler = new TrackingHandler();
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://example.test")
        };

        var config = CheqiSdkConfig.BuilderInstance()
            .CustomApiEndpoint("https://example.test")
            .ApiKey("sk_test_example")
            .Build();

        var client = new CheqiApiClient(config, httpClient: httpClient);
        client.Dispose();

        Assert.False(handler.Disposed);
        httpClient.Dispose();
        Assert.True(handler.Disposed);
    }

    private sealed class StaticResponseHandler : HttpMessageHandler
    {
        private readonly HttpResponseMessage _response;

        public StaticResponseHandler(HttpResponseMessage response)
        {
            _response = response;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(_response);
        }
    }

    private sealed class TrackingHandler : HttpMessageHandler
    {
        public bool Disposed { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK));
        }

        protected override void Dispose(bool disposing)
        {
            Disposed = true;
            base.Dispose(disposing);
        }
    }
}
