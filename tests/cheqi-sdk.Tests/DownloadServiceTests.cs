using Cheqi.Sdk.Config;
using Cheqi.Sdk.Download;
using Cheqi.Sdk.Models.Generated;
using Cheqi.Sdk.Decryption;
using Xunit;

namespace Cheqi.Sdk.Tests;

public sealed class DownloadServiceTests
{
    [Fact]
    public void LinksUseIndependentSecureCredentialsAndEnvironmentOrigins()
    {
        var service = new DownloadService();
        var production = service.GenerateDownloadLink(CheqiApiEnvironment.Production);
        var sandbox = service.GenerateDownloadLink(CheqiApiEnvironment.Sandbox);
        Assert.StartsWith(DownloadService.ProductionBaseUrl + "/", production.Url);
        Assert.StartsWith(DownloadService.SandboxBaseUrl + "/", sandbox.Url);
        Assert.Matches("^[A-Za-z0-9_-]{22}$", production.DownloadId);
        Assert.Matches("^[A-Za-z0-9_-]{43}$", production.ContentKey);
        Assert.NotEqual(production.DownloadId, sandbox.DownloadId);
        Assert.NotEqual(production.ContentKey, sandbox.ContentKey);
        Assert.Equal(production.ContentKey, service.ParseDownloadUrl(production.Url).ContentKey);
        Assert.DoesNotContain(production.ContentKey, production.ToString());
    }

    [Fact]
    public void EnvelopesRoundTripAndRejectTamperedCiphertext()
    {
        var service = new DownloadService();
        var link = service.GenerateDownloadLink("http://localhost:3000/receipts/");
        var envelope = new ReceiptEnvelope(link.DownloadId,
            new() { ["CHEQI"] = new("{\"totalAmount\":12.1}", ReceiptEnvelopeDocument.MediaTypeEnum.ApplicationJson) }, 1, "receipt-payload-v1", Guid.NewGuid());
        var ciphertext = service.EncryptDownloadEnvelope(envelope, link.ContentKey);
        var result = service.DecryptDownloadEnvelope(ciphertext, link.ContentKey);
        Assert.Equal(envelope.ReceiptUuid, result.ReceiptUuid);
        Assert.Equal(envelope.Documents["CHEQI"].Content, result.Documents["CHEQI"].Content);
        var data = Convert.FromBase64String(ciphertext);
        data[^1] ^= 1;
        Assert.Throws<DecryptionException>(() => service.DecryptDownloadEnvelope(Convert.ToBase64String(data), link.ContentKey));
    }

    [Theory]
    [InlineData("https://receipt.cheqi.io/short#abc")]
    [InlineData("https://receipt.cheqi.io/abcdefghijklmnopqrstuv")]
    [InlineData("javascript:alert(1)")]
    public void InvalidLinksAreRejected(string url) => Assert.Throws<ArgumentException>(() => new DownloadService().ParseDownloadUrl(url));
    [Fact]
    public void DecryptsJavaSdkKnownAes256GcmVector()
    {
        const string ciphertext = "AAECAwQFBgcICQoLPCC1c6CUq0noIvLiwZ0xCaHspX2+LxoudzfHqT8Nb9F0fcuS27Iwog/ZU8/t6V5dgjYQ6Ayz0alW+EQ7ItLZzIJZpR+6oVImeTrPHI77YZpu77sRHww9Ssff6qkLhJuh6Yo8hcEouG7LaVfWak8DT4pXUpvT9RW3fc9n/a/WXj6LfzD2hOfmYzjReJEN6nzyBsTSF0hGbdwYV5ZWFVhKIWkRZmjW";
        var plaintext = new AesDecryptor().Decrypt(ciphertext, Enumerable.Range(0, 32).Select(value => (byte)value).ToArray());
        using var document = System.Text.Json.JsonDocument.Parse(plaintext);
        Assert.Equal("INTEROP", document.RootElement.GetProperty("cheqiReceiptId").GetString());
        Assert.Equal("00000000-0000-0000-0000-000000000001", document.RootElement.GetProperty("receiptUuid").GetString());
    }
}
